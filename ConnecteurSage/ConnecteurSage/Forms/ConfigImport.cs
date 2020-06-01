using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnecteurSage.Forms
{
    public partial class ConfigImport : Form
    {
        public ConfigImport()
        {
            InitializeComponent();

            Boolean[] list1 = new Boolean[2];   //List pour activer ou desactiver l'export
            list1[0] = true;
            list1[1] = false;

            string[] list2 = new string[3];     //List des formats d'export
            list2[0] = "";
            list2[1] = "Plat";
            list2[2] = "Véolog";

            string[] list3 = new string[4];     //List des statuts
            list3[0] = "";
            list3[1] = "Saisie";
            list3[2] = "Confirmé";
            list3[3] = "A Préparé";


            // Init les comboBoxs
            for (int i = 0; i < getValuesExport().Length; i++)
            {
                comboBox_import_achats_cmd_active.Items.Add(((list1[i] == true) ? "Activer" : "Désactiver"));
                comboBox_import_achats_dsadv_active.Items.Add(((list1[i] == true) ? "Activer" : "Désactiver"));
                comboBox_import_achats_facture_active.Items.Add(((list1[i] == true) ? "Activer" : "Désactiver"));

                comboBox_import_ventes_cmd_active.Items.Add(((list1[i] == true) ? "Activer" : "Désactiver"));
                comboBox_import_ventes_dsadv_active.Items.Add(((list1[i] == true) ? "Activer" : "Désactiver"));
                comboBox_import_ventes_facture_active.Items.Add(((list1[i] == true) ? "Activer" : "Désactiver"));

                comboBox_import_stock_active.Items.Add(((list1[i] == true) ? "Activer" : "Désactiver"));
            }
            for (int i = 0; i < getFormatExport().Length; i++)
            {
                comboBox_import_achats_cmd_format.Items.Add(list2[i]);
                comboBox_import_achats_dsadv_format.Items.Add(list2[i]);
                comboBox_import_achats_facture_format.Items.Add(list2[i]);

                comboBox_import_ventes_cmd_format.Items.Add(list2[i]);
                comboBox_import_ventes_dsadv_format.Items.Add(list2[i]);
                comboBox_import_ventes_facture_format.Items.Add(list2[i]);

                comboBox_import_stock_format.Items.Add(list2[i]);
            }
            for (int i = 0; i < getStatutExport().GetLength(0); i++)
            {
                comboBox_import_achats_cmd_statut.Items.Add(list3[i]);
                comboBox_import_achats_dsadv_statut.Items.Add(list3[i]);
                comboBox_import_achats_facture_statut.Items.Add(list3[i]);

                comboBox_import_ventes_cmd_statut.Items.Add(list3[i]);
                comboBox_import_ventes_dsadv_statut.Items.Add(list3[i]);
                comboBox_import_ventes_facture_statut.Items.Add(list3[i]);

                comboBox_import_stock_statut.Items.Add(list3[i]);
            }

            Config_Import.ConfigurationSaveLoad settings = new Config_Import.ConfigurationSaveLoad();
            if (settings.isSettings())
            {
                settings.Load();
                Config_Import.Classes.ConfigurationImport configurationImport = settings.configurationImport;

                // Achats
                comboBox_import_achats_cmd_active.Text = ((Boolean.Parse(configurationImport.Doc_Achat.Commande.Activate)) ? "Activer" : "Désactiver");
                comboBox_import_achats_dsadv_active.Text = ((Boolean.Parse(configurationImport.Doc_Achat.DSADV.Activate)) ? "Activer" : "Désactiver");
                comboBox_import_achats_facture_active.Text = ((Boolean.Parse(configurationImport.Doc_Achat.Facture.Activate)) ? "Activer" : "Désactiver");

                comboBox_import_achats_cmd_format.Text = configurationImport.Doc_Achat.Commande.Format;
                comboBox_import_achats_dsadv_format.Text = configurationImport.Doc_Achat.DSADV.Format;
                comboBox_import_achats_facture_format.Text = configurationImport.Doc_Achat.Facture.Format;

                comboBox_import_achats_cmd_statut.Text = getStatutName(configurationImport.Doc_Achat.Commande.Status);
                comboBox_import_achats_dsadv_statut.Text = getStatutName(configurationImport.Doc_Achat.DSADV.Status);
                comboBox_import_achats_facture_statut.Text = getStatutName(configurationImport.Doc_Achat.Facture.Status);

                // Ventes
                comboBox_import_ventes_cmd_active.Text = ((Boolean.Parse(configurationImport.Doc_Ventes.Commande.Activate)) ? "Activer" : "Désactiver");
                comboBox_import_ventes_dsadv_active.Text = ((Boolean.Parse(configurationImport.Doc_Ventes.DSADV.Activate)) ? "Activer" : "Désactiver");
                comboBox_import_ventes_facture_active.Text = ((Boolean.Parse(configurationImport.Doc_Ventes.Facture.Activate)) ? "Activer" : "Désactiver");

                comboBox_import_ventes_cmd_format.Text = configurationImport.Doc_Ventes.Commande.Format;
                comboBox_import_ventes_dsadv_format.Text = configurationImport.Doc_Ventes.DSADV.Format;
                comboBox_import_ventes_facture_format.Text = configurationImport.Doc_Ventes.Facture.Format;

                comboBox_import_ventes_cmd_statut.Text = getStatutName(configurationImport.Doc_Ventes.Commande.Status);
                comboBox_import_ventes_dsadv_statut.Text = getStatutName(configurationImport.Doc_Ventes.DSADV.Status);
                comboBox_import_ventes_facture_statut.Text = getStatutName(configurationImport.Doc_Ventes.Facture.Status);

                // Stock
                comboBox_import_stock_active.Text = ((Boolean.Parse(configurationImport.Doc_Stock.Stock.Activate)) ? "Activer" : "Désactiver");
                comboBox_import_stock_format.Text = configurationImport.Doc_Stock.Stock.Format;
                comboBox_import_stock_statut.Text = getStatutName(configurationImport.Doc_Stock.Stock.Status);
            }

        }

        public static Boolean[] getValuesExport()
        {
            Boolean[] list1 = new Boolean[2];
            list1[0] = true;
            list1[1] = false;

            return list1;
        }
        public static string[] getFormatExport()
        {
            string[] list = new string[3];
            list[0] = "";
            list[1] = "Plat";
            list[2] = "Véolog";
            return list;
        }
        public static string[,] getStatutExport()
        {
            string[,] list = new string[4, 2];
            list[0, 0] = ""; list[0, 1] = "";
            list[1, 0] = "Saisie"; list[1, 1] = "0";   // Order created
            list[2, 0] = "Confirmé"; list[2, 1] = "1";   // Order ready to be send in EDI to Veolog
            list[3, 0] = "A Préparé"; list[3, 1] = "2";   // Order send to Veolog
            return list;
        }
        public static string getStatutValue(string value)
        {
            string result = "";
            for (int x = 0; x < getStatutExport().GetLength(0); x++)
            {
                if (value.Equals(getStatutExport()[x, 0]))
                {
                    result = getStatutExport()[x, 1];
                    break;
                }
            }
            return result;
        }
        public static string getStatutName(string value)
        {
            string result = "";
            for (int x = 0; x < getStatutExport().GetLength(0); x++)
            {
                if (value.Equals(getStatutExport()[x, 1]))
                {
                    result = getStatutExport()[x, 0];
                    break;
                }
            }
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox_import_achats_cmd_active_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_import_achats_cmd_active.Text == "Activer")
            {
                comboBox_import_achats_cmd_format.Enabled = true;
                comboBox_import_achats_cmd_statut.Enabled = true;
            }
            else
            {
                comboBox_import_achats_cmd_format.Enabled = false;
                comboBox_import_achats_cmd_statut.Enabled = false;
            }
        }

        private void comboBox_import_achats_dsadv_active_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_import_achats_dsadv_active.Text == "Activer")
            {
                comboBox_import_achats_dsadv_format.Enabled = true;
                comboBox_import_achats_dsadv_statut.Enabled = true;
            }
            else
            {
                comboBox_import_achats_dsadv_format.Enabled = false;
                comboBox_import_achats_dsadv_statut.Enabled = false;
            }
        }

        private void comboBox_import_achats_facture_active_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_import_achats_facture_active.Text == "Activer")
            {
                comboBox_import_achats_facture_format.Enabled = true;
                comboBox_import_achats_facture_statut.Enabled = true;
            }
            else
            {
                comboBox_import_achats_facture_format.Enabled = false;
                comboBox_import_achats_facture_statut.Enabled = false;
            }
        }

        private void comboBox_import_cmd_active_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_import_ventes_cmd_active.Text == "Activer")
            {
                comboBox_import_ventes_cmd_format.Enabled = true;
                comboBox_import_ventes_cmd_statut.Enabled = true;
            }
            else
            {
                comboBox_import_ventes_cmd_format.Enabled = false;
                comboBox_import_ventes_cmd_statut.Enabled = false;
            }
        }

        private void comboBox_import_dsadv_active_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_import_ventes_dsadv_active.Text == "Activer")
            {
                comboBox_import_ventes_dsadv_format.Enabled = true;
                comboBox_import_ventes_dsadv_statut.Enabled = true;
            }
            else
            {
                comboBox_import_ventes_dsadv_format.Enabled = false;
                comboBox_import_ventes_dsadv_statut.Enabled = false;
            }
        }

        private void comboBox_import_facture_active_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_import_ventes_facture_active.Text == "Activer")
            {
                comboBox_import_ventes_facture_format.Enabled = true;
                comboBox_import_ventes_facture_statut.Enabled = true;
            }
            else
            {
                comboBox_import_ventes_facture_format.Enabled = false;
                comboBox_import_ventes_facture_statut.Enabled = false;
            }
        }

        private void comboBox_import_stock_active_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_import_stock_active.Text == "Activer")
            {
                comboBox_import_stock_format.Enabled = true;
                comboBox_import_stock_statut.Enabled = true;
            }
            else
            {
                comboBox_import_stock_format.Enabled = false;
                comboBox_import_stock_statut.Enabled = false;
            }
        }


        private void enregistrer_config_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox_import_achats_cmd_active.Text) || !string.IsNullOrEmpty(comboBox_import_achats_dsadv_active.Text) || !string.IsNullOrEmpty(comboBox_import_achats_facture_active.Text) ||
                !string.IsNullOrEmpty(comboBox_import_ventes_cmd_active.Text) || !string.IsNullOrEmpty(comboBox_import_ventes_dsadv_active.Text) || !string.IsNullOrEmpty(comboBox_import_ventes_facture_active.Text) ||
                !string.IsNullOrEmpty(comboBox_import_ventes_cmd_active.Text))
            {
                Config_Import.ConfigurationSaveLoad settings = new Config_Import.ConfigurationSaveLoad();
                Config_Import.Classes.ConfigurationImport configurationExport = new Config_Import.Classes.ConfigurationImport(
                    new Config_Import.Classes.Custom_Doc.Doc_Achat(
                        new Config_Import.Classes.Custom_Doc.Custom.Commande(((comboBox_import_achats_cmd_active.Text == "Activer") ? "true" : "false"), comboBox_import_achats_cmd_format.Text, getStatutValue(comboBox_import_achats_cmd_statut.Text)),
                        new Config_Import.Classes.Custom_Doc.Custom.DSADV(((comboBox_import_achats_dsadv_active.Text == "Activer") ? "true" : "false"), comboBox_import_achats_dsadv_format.Text, getStatutValue(comboBox_import_achats_dsadv_statut.Text)),
                        new Config_Import.Classes.Custom_Doc.Custom.Facture(((comboBox_import_achats_facture_active.Text == "Activer") ? "true" : "false"), comboBox_import_achats_facture_format.Text, getStatutValue(comboBox_import_achats_facture_statut.Text))
                        ),
                    new Config_Import.Classes.Custom_Doc.Doc_Stock(
                        new Config_Import.Classes.Custom_Doc.Custom.Stock(((comboBox_import_stock_active.Text == "Activer") ? "true" : "false"), comboBox_import_stock_format.Text, getStatutValue(comboBox_import_stock_statut.Text))
                        ),
                    new Config_Import.Classes.Custom_Doc.Doc_Ventes(
                        new Config_Import.Classes.Custom_Doc.Custom.Commande(((comboBox_import_ventes_cmd_active.Text == "Activer") ? "true" : "false"), comboBox_import_ventes_cmd_format.Text, getStatutValue(comboBox_import_ventes_cmd_statut.Text)),
                        new Config_Import.Classes.Custom_Doc.Custom.DSADV(((comboBox_import_ventes_dsadv_active.Text == "Activer") ? "true" : "false"), comboBox_import_ventes_dsadv_format.Text, getStatutValue(comboBox_import_ventes_dsadv_statut.Text)),
                        new Config_Import.Classes.Custom_Doc.Custom.Facture(((comboBox_import_ventes_facture_active.Text == "Activer") ? "true" :" false"), comboBox_import_ventes_facture_format.Text, getStatutValue(comboBox_import_ventes_facture_statut.Text))
                    )
                );

                settings.configurationImport = configurationExport;
                settings.saveInfo();

                Close();
            }
            else
            {
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Document Achats
                /// 

                // Commande
                if (!string.IsNullOrEmpty(comboBox_import_achats_cmd_active.Text))
                {
                    MessageBox.Show("La saisi import Commande Fournisseur est obligatoire !!");
                }
                if (comboBox_import_achats_cmd_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_achats_cmd_format.Text))
                {
                    MessageBox.Show("Le Format d'Export Commande est obligatoire !!");
                }
                if (comboBox_import_achats_cmd_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_achats_cmd_statut.Text))
                {
                    MessageBox.Show("Le Statut d'Export Commande est obligatoire !!");
                }

                // DSADV
                if (!string.IsNullOrEmpty(comboBox_import_achats_dsadv_active.Text))
                {
                    MessageBox.Show("Export Bon de Livraision est obligatoire !!");
                }
                if (comboBox_import_achats_dsadv_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_achats_dsadv_format.Text))
                {
                    MessageBox.Show("Le Format d'Export Bon de Livraision est obligatoire !!");
                }
                if (comboBox_import_achats_dsadv_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_achats_dsadv_statut.Text))
                {
                    MessageBox.Show("Le Statut d'Export Bon de Livraision est obligatoire !!");
                }

                // Facture
                if (!string.IsNullOrEmpty(comboBox_import_achats_facture_active.Text))
                {
                    MessageBox.Show("D'Export Facture est obligatoire !!");
                }
                if (comboBox_import_achats_facture_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_achats_facture_format.Text))
                {
                    MessageBox.Show("Le Format d'Export Facture est obligatoire !!");
                }
                if (comboBox_import_achats_facture_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_achats_facture_statut.Text))
                {
                    MessageBox.Show("Le Statut d'Export Facture est obligatoire !!");
                }


                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Document Ventes
                /// 

                // Commande
                if (!string.IsNullOrEmpty(comboBox_import_ventes_cmd_active.Text))
                {
                    MessageBox.Show("Export Commande est obligatoire !!");
                }
                if (comboBox_import_ventes_cmd_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_ventes_cmd_format.Text))
                {
                    MessageBox.Show("Le Format d'Export Commande est obligatoire !!");
                }
                if (comboBox_import_ventes_cmd_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_ventes_cmd_statut.Text))
                {
                    MessageBox.Show("Le Statut d'Export Commande est obligatoire !!");
                }

                // DSADV
                if (!string.IsNullOrEmpty(comboBox_import_ventes_dsadv_active.Text))
                {
                    MessageBox.Show("Export Bon de Livraision est obligatoire !!");
                }
                if (comboBox_import_ventes_dsadv_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_ventes_dsadv_format.Text))
                {
                    MessageBox.Show("Le Format d'Export Bon de Livraision est obligatoire !!");
                }
                if (comboBox_import_ventes_dsadv_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_ventes_dsadv_statut.Text))
                {
                    MessageBox.Show("Le Statut d'Export Bon de Livraision est obligatoire !!");
                }

                // Facture
                if (!string.IsNullOrEmpty(comboBox_import_ventes_facture_active.Text))
                {
                    MessageBox.Show("D'Export Facture est obligatoire !!");
                }
                if (comboBox_import_ventes_facture_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_ventes_facture_format.Text))
                {
                    MessageBox.Show("Le Format d'Export Facture est obligatoire !!");
                }
                if (comboBox_import_ventes_facture_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_ventes_facture_statut.Text))
                {
                    MessageBox.Show("Le Statut d'Export Facture est obligatoire !!");
                }

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Stock
                /// 
                if (!string.IsNullOrEmpty(comboBox_import_stock_active.Text))
                {
                    MessageBox.Show("D'Export Stock est obligatoire !!");
                }
                if (comboBox_import_stock_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_stock_format.Text))
                {
                    MessageBox.Show("Le Format d'Export Stock est obligatoire !!");
                }
                if (comboBox_import_stock_active.Text.Equals("Activer") && !string.IsNullOrEmpty(comboBox_import_stock_statut.Text))
                {
                    MessageBox.Show("Le Statut d'Export Stock est obligatoire !!");
                }
            }
        }

        
    }
}
