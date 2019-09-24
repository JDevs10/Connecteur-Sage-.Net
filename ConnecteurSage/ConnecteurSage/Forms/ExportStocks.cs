using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using ConnecteurSage.Classes;
using ConnecteurSage.Helpers;
using System.Threading;
using System.IO;

namespace ConnecteurSage.Forms
{
    public partial class ExportStocks : Form
    {
        public ExportStocks()
        {
            InitializeComponent();
        }

        private Customer customer = new Customer();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void ExportStock()
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text)) //check if the seleted path is empty
                {
                    MessageBox.Show("Le chemin pour l'export du fichier stock liste doit être renseigné !"); //if yes prompt error message
                    return;
                }
                
                List<Stock> s = new List<Stock>(); //creating list type stock
                s = GetStockArticle(); //call function GetStockArticle to get all the products and their stock

                //testing purpose only :begin
                /*Stock s1 = new Stock("product 1", "PROD1", "1234567891234", "59", "LOT-BDF9411123", "5.00000", "0");
                Stock s2 = new Stock("product 2", "PROD2", "4321987654321", "15", "MV32", "1.0000", "1");
                s.Add(s1);
                s.Add(s2);*/
                //testing purpose only :end

                if (s == null) //check if the list is empty or not
                {
                    MessageBox.Show("Failed to obtain value from database : (Maybe failed to connect with database) "); //show failed to connect with database
                }
                else
                {

                    string[] stocklines = new string[s.Count]; //creating array to add output lines for file
                    int i=0;
                    foreach (Stock stockline in s) //reading line per line from the list
                    {
                        stocklines[i] = "L;" + stockline.reference + ";" + stockline.codebarre + ";" + stockline.stock + ";" + stockline.numerolot + ";" + stockline.lotqty + ";" + stockline.lotepuise + ";" + (i + 1); //adding lines into array for file
                        i++; //increment for further adding/reading into the array
                    }

                    string fileName = string.Format(textBox1.Text + @"\" + "stock_{0:yyMMddHHmmss}.csv", DateTime.Now); //creating the file.

                    if (File.Exists(fileName)) //verifying if the file exists else delete and recreate
                    {
                        File.Delete(fileName); //delete file.
                    }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName)) // streaming the file 
                    {
                        foreach (string line in stocklines) //reading line per line from array
                        {
                            file.WriteLine(line); //writing inside the file
                        }
                        file.WriteLine("F" + ";" + i); //writing at the end of file
                    }
                    
                    // *file has been generated at the end of the method using @fileName*
                    
                    /*string myFileData = File.ReadAllText(fileName); //get all content of the created file (need to fix)
                    if (myFileData.EndsWith(Environment.NewLine)) //check if at the end of the has empty return/jump character
                    {
                        File.WriteAllText(@"D:\test_backup.csv", myFileData.TrimEnd(Environment.NewLine.ToCharArray()) ); //remove jump at the end of the file
                    }*/

                    MessageBox.Show("File has been generated at : "+fileName); //show message file has been generated

                }

                Close(); //close the ExportStock window.

            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                MessageBox.Show("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        /*
        * 
        GET ALL LIST OF CUSTOMERS TO ADD INTO THE COMBOLIST
        */
        private List<Customer> GetListClients()
        {
            try
            {
                List<Customer> listClient = new List<Customer>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListClient(), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Customer client = new Customer(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(), reader[16].ToString());
                                listClient.Add(client);
                            }
                        }
                    }
                    return listClient;

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                MessageBox.Show("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

        }


        private List<Stock> GetStockArticle()
        {
            try
            {
                List<Stock> stock_info = new List<Stock>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();//connecting as handler with database

                    OdbcCommand command = new OdbcCommand(QueryHelper.getStockInfo(), connection);//Exécution de la requête permettant de récupérer les articles du dossier
                    {
                        using (IDataReader reader = command.ExecuteReader()) //reading lines fetched.
                        {
                            while (reader.Read())
                            {
                                Stock stock = new Stock(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString());
                                stock_info.Add(stock);
                            }
                        }
                    }
                    return stock_info;
                }
            }
            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                MessageBox.Show("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private void ExportStocks_Load(object sender, EventArgs e)
        {
            
            //adding columns to the grid
            cols_rows.ColumnCount     = 5;  //size of columns
            cols_rows.Columns[0].Name = "libelle";//string[] row1 = new string[] { "Vis de fixation","XVIS","59536109","241.000000" };
            cols_rows.Columns[1].Name = "reference";//cols_rows.Rows.Add(row2);
            cols_rows.Columns[2].Name = "stock";
            cols_rows.Columns[3].Name = "barcode";
            cols_rows.Columns[4].Name = "num. lot";

            cols_rows.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            cols_rows.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            cols_rows.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            cols_rows.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            cols_rows.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            List<Stock> stock_info = new List<Stock>(); //creating stock_info object
            stock_info = GetStockArticle(); //this class function to get all data from database

            if (stock_info == null)
            {
                MessageBox.Show("Failed to obtain value from database : (Maybe failed to connect with database) "); //show failed to connect with database
            }
            else
            {

                //count total
                int total_stock = stock_info.Count; //counting total data
                if (total_stock > 0) //checking if total customers more than zero
                {

                    foreach (Stock stockline in stock_info) //loop each element
                    {
                       cols_rows.Rows.Add(new string[] { stockline.libelle,stockline.reference,stockline.stock,stockline.codebarre,stockline.numerolot }); //add element into the gridview
                    }

                }
                else if (total_stock == 0) // checking if total_customers is null or zero
                {

                    MessageBox.Show("0 Customers retreived from the database."); //show messagebox because zero data received

                }

            }

        }

        private void cols_rows_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void cols_rows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string text = this.cols_rows.CurrentRow.Cells[0].Value.ToString();
            if (text == null)
            {
                MessageBox.Show("The selected item value is = empty");
            }
            else
            {
                MessageBox.Show("The selected item value is = " + text);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void export_stockliste_Click(object sender, EventArgs e)
        {
            ExportStock(); //calling this class' function to export list of stock
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = f.SelectedPath;
            }
        }
        /*
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string text = listBox1.SelectedItem.ToString();
            MessageBox.Show(" The selected item value is = "+text);
        }
        */


    }
}
