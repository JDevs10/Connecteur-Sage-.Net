using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ConnecteurSage.Classes;
using System.Globalization;
using System.Data.Odbc;
using ConnecteurSage.Helpers;

namespace ConnecteurSage.Forms
{
    public partial class importManuel : Form
    {
        private static string filename = "";
        private static List<string> MessageErreur;

        public importManuel()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exportCustomersFileBrowseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "CSV|*.csv";
                //dialog.AddExtension = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (System.IO.Path.GetExtension(dialog.FileName).ToLower() == ".csv")
                    {
                        exportCustomersFilenameTextBox.Text = dialog.FileName;
                        filename = dialog.FileName;
                    }
                    else
                    {
                        exportCustomersFilenameTextBox.Text = string.Empty;
                        MessageBox.Show("Le format de ce fichier doit être de type CSV.");
                    }
                }
            }
        }

        public static string ConvertDate(string date)
        {
            if(date.Length == 8 && IsNumeric(date)) {
            return date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);
            }
            return "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(exportCustomersFilenameTextBox.Text))
            {
                MessageBox.Show("Le chemin du fichier d'import de commande doit être renseigné");
                return;
            }

            try
            {
                Boolean prixDef = false;
                //Boolean insertAdressLivr = false;
                Order order = new Order();

                if(checkBox1.Checked)
                {
                    order.Id = NextNumPiece();
                    goto sautNumPiece;
                }

                order.Id = get_next_num_piece_commande();

                //if (TestSiNumPieceExisteDeja(order.Id))
                //{
                //    order.Id = NextNumPiece();
                //}

                sautNumPiece :

                if (order.Id == "erreur")
                {
                    return;
                }

                if (order.Id == null)
                {
                    MessageBox.Show("Erreur [10] : numéro de piece non valide", "Erreur !!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }



                order.Lines = new List<OrderLine>();
                long pos = 1;
                string[] lines = System.IO.File.ReadAllLines(filename, Encoding.Default);

                if (lines[0].Split(';')[0] == "ORDERS" && lines[0].Split(';').Length == 11)
                {
                    order.NumCommande = lines[0].Split(';')[1];
                    if (order.NumCommande.Length > 10)
                    {
                        MessageBox.Show("Numéro de commande doit être < 10", "Erreur de lecture !!",
                                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (order.NumCommande == "")
                    {
                        MessageBox.Show("Le champ numéro de commande est vide.", "Erreur !!",
                                                                           MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (!IsNumeric(order.NumCommande))
                    {
                        MessageBox.Show("Le champ numéro de commande est invalide.", "Erreur !!",
                                                                           MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    string existe = existeCommande(order.NumCommande);

                    if (existe != null && existe != "erreur")
                    {
                        MessageBox.Show("La commande N° " + order.NumCommande + " existe deja dans la base.\nN° de pièce : "+existe+"", "Erreur !!",
                                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (existe == "erreur")
                    {
                      return;
                    }

                    order.codeClient = lines[0].Split(';')[2];
                    order.codeAcheteur = lines[0].Split(';')[3];
                    order.codeFournisseur = lines[0].Split(';')[4];
                    //order.adresseLivraison = lines[0].Split(';')[7];


                    Client client = getClient(order.codeClient,1);
                    if (client == null)
                    {
                        return;
                    }

                    Client client2 = getClient(order.codeAcheteur,2);
                    if (client2 == null)
                    {
                        return;
                    }


                    if (existeFourniseur(order.codeFournisseur) == null)
                    {
                        return;
                    }

                    order.adresseLivraison = lines[0].Split(';')[7];
                    string[] tab_adress = order.adresseLivraison.Split('.');
                    if (tab_adress.Length != 5)
                    {
                        MessageBox.Show("La forme de l'adresse de livraison est incorrecte, Veuillez respecter la forme suivante :\nNom.Adresse.CodePostal.Ville.Pays", "Erreur !!",
                                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    order.nom_contact = tab_adress[0];
                    order.adresse = tab_adress[1].Replace("'", "''");
                    order.codepostale = tab_adress[2];
                    order.ville = tab_adress[3].Replace("'", "''");
                    order.pays = tab_adress[4];


                    List<AdresseLivraison> listAdress = get_adresse_livraison(new AdresseLivraison(1,client.CT_Num,order.nom_contact, order.adresse, order.codepostale, order.ville, order.pays));

                    
                    // Ajouter ville dans la réference
                    //string[] part = order.adresseLivraison.Split('.');
                    //if (part.Length >= 2)
                    //{
                    order.Reference = order.ville;
                    //}
                    
                    order.deviseCommande = lines[0].Split(';')[8];
                    
                    if (order.deviseCommande != "")
                    {
                    order.deviseCommande = getDevise(order.deviseCommande);
                    }

                    if (order.deviseCommande == "erreur")
                    {
                        return;
                    }


                    if (lines[1].Split(';')[0] == "ORDHD1" && lines[1].Split(';').Length == 5)
                    {
                        
                        if (lines[1].Split(';')[1].Length == 8)
                        {
                            order.DateCommande = ConvertDate(lines[1].Split(';')[1]);
                            order.conditionLivraison = lines[1].Split(';')[2];

                            if (order.conditionLivraison != "")
                            {
                            order.conditionLivraison = get_condition_livraison(order.conditionLivraison);
                            }

                            if (string.IsNullOrEmpty(order.conditionLivraison))
                            {
                                order.conditionLivraison = "1";
                            }

                            if (lines[2].Split(';')[0] == "ORDLIN" && lines[2].Split(';').Length == 23)
                            {
                                decimal total = 0m;
                                foreach (string ligneDuFichier in lines)
                                {

                                    string[] tab = ligneDuFichier.Split(';');

                                    switch (tab[0])
                                    {
                                        case "ORDLIN":
                                            if (tab.Length == 23)
                                            {
                                                OrderLine line = new OrderLine();
                                                line.NumLigne = tab[1];
                                                line.article = getArticle(tab[2]);
                                                
                                                if (line.article == null)
                                                {
                                                    return;
                                                }


                                                if (line.article.AR_Nomencl == "2" || line.article.AR_Nomencl == "3")
                                                {
                                                    line.article.AR_REFCompose = line.article.AR_REF;
                                                }

                                                if (line.article.gamme1 != "0")
                                                {
                                                    line.article.gamme1 = testGamme(0, line.article.AR_REF, line.article.gamme1);
                                                }

                                                if(line.article.gamme2 != "0")
                                                {
                                                line.article.gamme2 = testGamme(1,line.article.AR_REF, line.article.gamme2);
                                                }

                                                line.Quantite = tab[9].Replace(",", ".");
                                                decimal d = Decimal.Parse(line.Quantite, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                                                if (d == 0)
                                                {

                                                    line.Quantite = "1";

                                                }
                                                line.PrixNetHT = tab[14].Replace(",",".");
                                                line.MontantLigne = tab[11];
                                                line.DateLivraison = "'{d " + ConvertDate(tab[21]) + "}'";
                                                if (line.DateLivraison.Length==6)
                                                {
                                                    line.DateLivraison = "Null";
                                                }

                                                //if (line.article.AR_UnitePoids == "2")
                                                //{
                                                //    line.article.AR_POIDSBRUT = Convert.ToString(1000 * d * Decimal.Parse(line.article.AR_POIDSBRUT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                //    line.article.AR_POIDSNET = Convert.ToString(1000 * d * Decimal.Parse(line.article.AR_POIDSNET, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                //}

                                                //if(line.article.AR_UnitePoids == "3")
                                                //{
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(line.article.AR_POIDSBRUT))
                                                    {
                                                        line.article.AR_POIDSBRUT = Convert.ToString(d * Decimal.Parse(line.article.AR_POIDSBRUT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                    }
                                                    if (!string.IsNullOrEmpty(line.article.AR_POIDSNET))
                                                    {
                                                        line.article.AR_POIDSNET = Convert.ToString(d * Decimal.Parse(line.article.AR_POIDSNET, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                    }
                                                } 
                                                catch
                                                {
                                                     DialogResult resultDialog = MessageBox.Show("Erreur de conversion de poids.",
                                                            "Warning Message !!",
                                                            MessageBoxButtons.OKCancel,
                                                            MessageBoxIcon.Warning,
                                                            MessageBoxDefaultButton.Button2);

                                                    if (resultDialog == DialogResult.Cancel)
                                                    {
                                                        return;
                                                    }
                                                }
                                                //}
                                                //}
                                                line.codeAcheteur = tab[4].Replace(" ","");
                                                line.codeFournis = tab[5].Replace(" ", "");
                                                //line.codeFournis = line.codeFournis.Replace(Environment.NewLine, String.Empty);
                                                line.descriptionArticle = tab[8].Replace("'", "''");
                                                total = total + Decimal.Parse(tab[11].Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

                                                decimal prix = Decimal.Parse(line.PrixNetHT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                                                decimal prixSage = Decimal.Parse(line.article.AR_PRIXVEN.Replace(",","."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

                                                if (prix != prixSage)
                                                {
                                                    DialogResult resultDialog = MessageBox.Show("Prix de l'article " + line.article.AR_REF + "(" + tab[2] + ") dans la base est : " + prixSage + "\nIl est différent du prix envoyer par le client : " + prix + ".",
                                                            "Warning Message !!",
                                                            MessageBoxButtons.OKCancel,
                                                            MessageBoxIcon.Warning,
                                                            MessageBoxDefaultButton.Button2);

                                                    if (resultDialog == DialogResult.Cancel)
                                                    {
                                                        return;
                                                    }

                                                    if (resultDialog == DialogResult.OK)
                                                    {
                                                        prixDef = true;
                                                    }
                                                }

                                                order.Lines.Add(line);
                                            }
                                            else
                                            {
                                                MessageBox.Show("Erreur dans la ligne " + pos + " du fichier.", "Erreur de lecture !!",
                                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                                return;
                                            }
                                            break;

                                    }

                                    pos++;

                                }

                                //order.MontantTotal = total;

                               

                                order.DateLivraison = "Null";

                                for (int i = 0; i < order.Lines.Count; i++)
                                {
                                    if (order.Lines[i].DateLivraison.Length == 16)
                                    {
                                        order.DateLivraison = order.Lines[i].DateLivraison;
                                        goto jamp;
                                    }
                                }

                            jamp:

                    

                                if (order.codeClient != "")
                                {
                                    
                                    if(string.IsNullOrEmpty(order.deviseCommande))
                                    {
                                        order.deviseCommande = client.N_Devise;
                                    }

                                    order.StockId = getStockId();
                                    if (string.IsNullOrEmpty(order.StockId))
                                    {
                                        return;
                                    }

                                    if(!prixDef)
                                    {
                                        string Ref = order.Reference + "/" + order.NumCommande;

                                        if (Ref.Length <= 17)
                                        {
                                            order.Reference = Ref ;
                                        }
                                        else
                                        {
                                            int reste = 16 - order.NumCommande.Length;

                                            if (order.Reference.Length > reste)
                                            {
                                                order.Reference = order.Reference.Substring(0, reste) + "/" + order.NumCommande;
                                            }
                                        }
                                    }

                                    

                                    if(prixDef)
                                    {
                                        string pr = "/AP";
                                        string Ref = order.Reference + "/" + order.NumCommande+pr;

                                        if (Ref.Length <= 17)
                                        {
                                            order.Reference = Ref;
                                        }
                                        else
                                        {
                                            int reste = 16 - order.NumCommande.Length - pr.Length ;

                                            if (order.Reference.Length > reste)
                                            {
                                                order.Reference = order.Reference.Substring(0, reste) + "/" + order.NumCommande+pr;
                                            }
                                        }

                                         //if (order.Reference.Length > 13)
                                         //               {
                                         //                   order.Reference = order.Reference.Substring(0,12) + "../AP";
                                         //               }
                                         //               else
                                         //               {
                                         //                   order.Reference = order.Reference+"/AP";
                                         //               }
                                    }

                                    //order.FNT_MONTANTTOTALTAXES = (total * 19.6m) / 100.0m ;
                                    //return;

                                    if(order.Lines.Count == 0)
                                    {
                                        MessageBox.Show("Aucun ligne de commande enregistré.", "Erreur de lecture !!",
                                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        return;
                                    }
                                    MessageErreur = new List<string>();

                                    //###################################################################
                                    //                    TEST ADRESSE DE LIVRAISION
                                    //###################################################################

                                    //if (listAdress.Count != 0)
                                    //{
                                    //    order.adresseLivraison = listAdress[0].Li_no;
                                    //    if (string.IsNullOrEmpty(order.adresseLivraison))
                                    //    {
                                    //        MessageBox.Show("Adresse de livraison invalide");
                                    //        return;
                                    //    }
                                    //}

                                    //if (listAdress.Count == 0)
                                    //{
                                    //    string intitule = client.CT_Num + " " + order.ville;

                                    //    List<string> listIntitules = TestIntituleLivraison(intitule);

                                    //    //if (listIntitules == null && listIntitules.Count == 0)
                                    //    //{
                                    //    //    tabCommandeError.Add(filename.ToString());
                                    //    //    goto goOut;
                                    //    //}

                                    //    int inc = 1;

                                    //incIntitule:

                                    //    if (listIntitules.Count != 0)
                                    //    {
                                    //        for (int i = 0; i < listIntitules.Count; i++)
                                    //        {
                                    //            if (intitule == listIntitules[i])
                                    //            {
                                    //                if (inc != 1)
                                    //                {
                                    //                    intitule = intitule.Substring(0, (intitule.Length - 4));
                                    //                }
                                    //                intitule = intitule + " N°" + inc;
                                    //                inc++;
                                    //                goto incIntitule;
                                    //            }
                                    //        }
                                    //    }
                                    //    if (insert_adresse_livraison(client.CT_Num, new AdresseLivraison("", order.nom_contact, order.adresse, order.codepostale, order.ville, order.pays, order.conditionLivraison, intitule)))
                                    //    {
                                    //        order.adresseLivraison = get_Last_insert_livraison(client.CT_Num);
                                    //        if (string.IsNullOrEmpty(order.adresseLivraison))
                                    //        {
                                    //            return;
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        return;
                                    //    }
                                    //}

                                    //###################################################################
                                    //                   FIN DE TEST ADRESSE DE LIVRAISION
                                    //###################################################################


                                    //if (insertAdressLivr)
                                    //{
                                    //    order.adresseLivraison = get_Last_insert_livraison();
                                    //    if (string.IsNullOrEmpty(order.adresseLivraison))
                                    //    {
                                    //        return;
                                    //    }
                                    //}

                                  


                                    //if (!insertAdressLivr && listAdress.Count == 0)
                                    //{
                                        //order.adresseLivraison = getNumLivraison(client.CT_Num);
                                        //if (string.IsNullOrEmpty(order.adresseLivraison))
                                        //{
                                        //    return;
                                        //}
                                    //}

                                    order.adresseLivraison = getNumLivraison(client.CT_Num);
                                    if (string.IsNullOrEmpty(order.adresseLivraison))
                                    {
                                        return;
                                    }

                                    if (insertCommande(client, order))
                                    {
                                        int nbr=0;

                                        for (int i = 0; i < order.Lines.Count; i++)
                                        {
                                            if (order.Lines[i].article.AR_SuiviStock == "0")
                                            {
                                                order.Lines[i].article.AR_StockId = "0";
                                            }
                                            else
                                            {
                                                order.Lines[i].article.AR_StockId = order.StockId;
                                            }

                                            if (insertCommandeLine(client, order, order.Lines[i]))
                                            {
                                                nbr++;
                                            }
                                        }
                                        string mot="";
                                        for(int i=0;i<MessageErreur.Count;i++)
                                        {
                                            mot = mot + MessageErreur[i] + "\n";
                                        }

                                        if (nbr == 0)
                                        {
                                            deleteCommande(order.NumCommande);
                                        }

                                        //if (nbr != 0)
                                        //{
                                        //    UpdateCommandeTaxes(order.FNT_MONTANTTOTALTAXES.ToString().Replace(",","."),order.Id);
                                        //}
                                        Close();

                                        //// Creer dossier sortie "LOG Directory" --------------------------
                                        //var dirName = string.Format("LogSage(manuelle) {0:dd-MM-yyyy HH.mm.ss}", DateTime.Now);
                                        //string outputFile = System.IO.Path.GetDirectoryName(filename) + @"\" + dirName;
                                        //System.IO.Directory.CreateDirectory(outputFile);
                                        ////deplacer les fichiers csv
                                        //System.IO.File.Move(filename, outputFile + @"\" + System.IO.Path.GetFileName(filename));


                                        MessageBox.Show(""+nbr+"/"+order.Lines.Count+" ligne(s) enregistrée(s).\n"+mot, "Information d'insertion",
                                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Il faut mentionner le code client.", "Erreur de lecture !!",
                                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Erreur dans la troisième ligne du fichier.", "Erreur de lecture !!",
                                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Date de la commande est incorrecte", "Erreur de lecture !!",
                                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur dans la deuxième ligne du fichier.", "Erreur de lecture !!",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("Erreur dans la première ligne du fichier.", "Erreur de lecture !!",
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }


                
            }
            catch (Exception ex) {
                MessageBox.Show(" ERREUR[0]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR",""), "Erreur!!",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public static Client getClient(string id, int flag)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getClient(id), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Client cli = new Client(reader[0].ToString(),reader[1].ToString(),reader[2].ToString(),reader[3].ToString(),reader[4].ToString(),reader[5].ToString(),reader[6].ToString(),reader[7].ToString(),reader[8].ToString(),reader[9].ToString(),reader[10].ToString(),reader[11].ToString());
                                connection.Close();
                                return cli;
                            }
                            else
                            {
                                if(flag==1)
                                {
                                MessageBox.Show("GLN émetteur  " + id + " n'existe pas dans la base sage.", "Erreur !!",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                if(flag==2)
                                {
                                MessageBox.Show("GLN destinataire  " + id + " n'existe pas dans la base sage.", "Erreur !!",
                                                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[1]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", ""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
            }

        }

        public static string getStockId()
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getStockId(), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string id=reader[0].ToString();
                                connection.Close();
                                return id;

                            }
                            else
                            {
                                MessageBox.Show("Il n'y a pas de stock enregistré.", "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[2]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR",""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
            }

        }

        public static string getNumLivraison(string client_num)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getNumLivraison(client_num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {
                                MessageBox.Show("Numero de livraison n'existe pas pour le tier " + client_num + "", "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[3]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR",""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
            }

        }

        public static string get_Last_insert_livraison(string client)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_last_Num_Livraison(client), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {
                                MessageBox.Show("Numero de livraison n'existe pas", "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[3]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
            }

        }

        public static Boolean insertCommande(Client client,Order order)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();

                    OdbcCommand command = new OdbcCommand(QueryHelper.insertCommande(client, order), connection);
                    //MessageBox.Show(command.CommandText);
                    command.ExecuteReader();

                        connection.Close();
                        return true;
                    

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[4]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR",""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

        }

        public static Boolean insertCommandeLine(Client client, Order order, OrderLine orderLine)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.insertLigneCommande(client, order, orderLine), connection);
                    //MessageBox.Show(command.CommandText);
                    //Console.Read();
                    command.ExecuteReader();

                    connection.Close();
                    return true;


                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //MessageBox.Show("Echec d'insertion de la ligne '"+orderLine.NumLigne+"' de la commande "+order.NumCommande+"." + "\n"+ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", ""), "Erreur!!",
                      //                          MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    MessageErreur.Add("Echec d'insertion de la ligne " + orderLine.NumLigne + " de la commande " + order.NumCommande + "." + "\n" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR",""));
                    return false;
                }
            }

        }

        public static Boolean deleteCommande(string NumCommande)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(NumCommande), connection);
                    command.ExecuteReader();

                    connection.Close();
                    return true;


                }
                catch
                {
                  return false;
                }
            }

        }

        public static Boolean UpdateCommandeTaxes(string montantTaxes, string do_piece)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.UpdateCommandeTaxes(montantTaxes, do_piece), connection);
                    command.ExecuteReader();

                    connection.Close();
                    return true;


                }
                catch
                {
                    return false;
                }
            }

        }

        public static Article getArticle(string code_article)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getArticle(code_article), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Article article = new Article(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString());
                                //MessageBox.Show(reader[0].ToString());
                                //MessageBox.Show(article.AR_REF+" gamme1:"+ article.gamme1+" gamme2:"+article.gamme2 );
                                connection.Close();
                                return article;

                            }
                            else
                            {
                                MessageBox.Show("code article "+code_article+" n'existe pas dans la base.", "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[5]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR",""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
            }

        }

        public static string testGamme(int type,string code_article,string gamme)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getGAMME(type,code_article), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            List<string> list=new List<string>();
                            while (reader.Read())
                            {
                                list.Add(reader[0].ToString());
                            }

                            Boolean ok = false;

                            for (int i=0; i < list.Count; i++)
                            {
                                if (gamme == list[i])
                                    ok = true;
                            }


                            if (!ok && list.Count > 0)
                            {
                                return list[0];
                            }

                            return gamme;
                             
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[6]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR",""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return gamme;
                }
            }

        }

        public static string getDevise(string codeIso)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getDevise(codeIso), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[7]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR",""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return "erreur";
                }
            }

        }


        public static string existeCommande(string num)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_NumPiece_Motif(num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string numero = reader[0].ToString();
                                connection.Close();
                                return numero;

                            }
                            else
                            {
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[7]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return "erreur";
                }
            }

        }

        public static string existeFourniseur(string num)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.fournisseurExiste(num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string numero = reader[0].ToString();
                                connection.Close();
                                return numero;

                            }
                            else
                            {
                                MessageBox.Show("Code GLN fournisseur "+num+" n'existe pas.", "Erreur de lecture !!",
                                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[7]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return "erreur";
                }
            }

        }

        public static string MaxNumPiece()
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.MaxNumPiece(), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {
                                return "BC00000";
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[8]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return "erreur";
                }
            }

        }

        public static string NextNumPiece()
        {
            try
            {
                string NumCommande = MaxNumPiece();

                if (NumCommande=="erreur")
                {
                    return "erreur";
                }

                NumCommande = NumCommande.Replace("BC", "");

                if (IsNumeric(NumCommande))
                {
                    int Nombre = int.Parse(NumCommande) + 1;
                    string num = Nombre.ToString();

                    while (num.Length < 5)
                    {
                        num = "0" + num;
                    }

                    NumCommande = "BC" + num;

                    return NumCommande;

                }

                return null;
            }
            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                MessageBox.Show(" ERREUR[9]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""), "Erreur!!",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return "erreur";
            }

            

        }



        public static Boolean IsNumeric(string Nombre)
        {
            try
            {
                int.Parse(Nombre);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string get_next_num_piece_commande()
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_Next_NumPiece_BonCommande(), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {
                                return NextNumPiece();
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[10]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return "erreur";
                }
            }

        }

        public static string get_condition_livraison(string c_mode)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_condition_livraison_indice(c_mode), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[11]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return "erreur";
                }
            }

        }

        public static List<AdresseLivraison> get_adresse_livraison(AdresseLivraison adresse)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    List<AdresseLivraison> list = new List<AdresseLivraison>();
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_adresse_livraison(adresse), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                list.Add(new AdresseLivraison(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(),"",""));
                            }

                            return list;
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[12]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
            }

        }

        public static Boolean insert_adresse_livraison(string client,AdresseLivraison adresse)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();

                    OdbcCommand command = new OdbcCommand(QueryHelper.insert_adresse_livraison(client,adresse), connection);
                    command.ExecuteReader();

                    connection.Close();
                    return true;


                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(" ERREUR[13]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""), "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

        }

        public static Boolean TestSiNumPieceExisteDeja(string num)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.TestSiNumPieceExisteDeja(num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                connection.Close();
                                return true;

                            }
                            else
                            {
                                return false;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[42] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    MessageBox.Show(" ERREUR[55]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""), "Erreur!!",
                                               MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

        }

        public static List<string> TestIntituleLivraison(string Intitule)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    List<string> adresses = new List<string>();
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.TestIntituleLivraison(Intitule), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                adresses.Add(reader[0].ToString());
                            }

                            return adresses;

                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[42] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    MessageBox.Show(DateTime.Now + " : Erreur[42] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return null;
                }
            }

        }
    }
}
