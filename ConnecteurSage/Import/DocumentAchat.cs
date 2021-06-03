using Connexion;
using System;
using Import.Classes;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import
{
    public class DocumentAchat
    {
        private string logFileName_import;
        private List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new;

        public DocumentAchat() { }
        public DocumentAchat(string logFileName_import_, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new_)
        {
            this.recapLinesList_new = recapLinesList_new_;
            this.logFileName_import = logFileName_import_;
        }


        public List<Alert_Mail.Classes.Custom.CustomMailRecapLines> returnAlertLogs()
        {
            return recapLinesList_new;
        }


        public static string[,] insertSupplierOrder(string reference_BLF_doc, Veolog_BCF dh, List<Veolog_BCF_Lines> dl, string fileName, StreamWriter logFileWriter, string logFileName_import, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
        {
            string[,] list_of_cmd_lines = new string[dl.Count, 82];    // new string [x,y]
            string[] list_of_supplier_info = null;
            int position_item = 0;
            DateTime d = DateTime.Now;
            string curr_date = d.ToString("yyyy-MM-dd");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;

            using (OdbcConnection connexion = ConnexionManager.CreateOdbcConnexionSQL()) //connecting to database as handler
            {
                try
                {
                    connexion.Open();
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Connexion ouverte.");

                    //Get ref client CMD, nature_OP_P && total ht
                    string nature_op_p_ = "";
                    string do_totalHT_ = "";
                    string do_totalHTNet_ = "";
                    string do_totalTTC_ = "";
                    string do_NetAPayer_ = "";
                    string do_MontantRegle_ = "";
                    string ref_supplier = null;

                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Récupérer la référence Commande fournisseur livré de la commande " + dh.Ref_Commande_Donneur_Ordre);
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.getCMDSupplierByRef(true, dh.Ref_Commande_Donneur_Ordre));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getCMDSupplierByRef(true, dh.Ref_Commande_Donneur_Ordre), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                dh.Ref_Commande_Fournisseur = reader[0].ToString();
                                nature_op_p_ = reader[1].ToString();
                                do_totalHT_ = reader[2].ToString().Replace(",", ".");
                                do_totalHTNet_ = reader[3].ToString().Replace(",", ".");
                                do_totalTTC_ = reader[4].ToString().Replace(",", ".");
                                do_NetAPayer_ = reader[5].ToString().Replace(",", ".");
                                do_MontantRegle_ = reader[6].ToString().Replace(",", ".");
                                ref_supplier = reader[7].ToString();
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse. ");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", "Aucun BCF trouvé dans la BDD", "insertSupplierOrder() : Aucune reponse", fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    //get Client Reference by Ref
                    logFileWriter.WriteLine("");
                    if (ref_supplier != null)
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.getClientReferenceById_DESADV(true, ref_supplier));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceById_DESADV(true, ref_supplier), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_supplier_info = new string[16];
                                    list_of_supplier_info[0] = reader[0].ToString();      // CT_Num
                                    list_of_supplier_info[1] = reader[1].ToString();      // CA_Num 
                                    list_of_supplier_info[2] = reader[2].ToString();      // CG_NumPrinc
                                    list_of_supplier_info[3] = reader[3].ToString();      // CT_NumPayeur
                                    list_of_supplier_info[4] = reader[4].ToString();      // N_Condition
                                    list_of_supplier_info[5] = reader[5].ToString();      // N_Devise
                                    list_of_supplier_info[6] = reader[6].ToString();      // CT_Langue
                                    list_of_supplier_info[7] = reader[7].ToString();      // DO_NbFacture = CT_Facture
                                    list_of_supplier_info[8] = reader[8].ToString().Replace(',', '.');      // DO_TxEscompte = CT_Taux02
                                    list_of_supplier_info[9] = reader[9].ToString();      // N_CatCompta
                                    list_of_supplier_info[10] = reader[10].ToString();    // CO_No
                                    list_of_supplier_info[11] = reader[11].ToString();    //  DO_Tarif = N_CatTarif
                                    list_of_supplier_info[12] = reader[12].ToString();    //  DO_Expedit = N_Expedition du tier
                                    list_of_supplier_info[13] = reader[13].ToString();    //  CT_Intitule
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse. list_of_fournisseur_info est null");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée. Le fournisseur \"" + ref_supplier + "\" n'existe pas dans Sage", "Le fournisseur \"" + ref_supplier + "\" n'existe pas dans Sage", "insertSupplierOrder() : Aucune reponse", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.getClientDeliveryAddress_DESADV(true, ref_supplier));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientDeliveryAddress_DESADV(true, ref_supplier), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_supplier_info[14] = reader[0].ToString();    // LI_No
                                    list_of_supplier_info[15] = reader[0].ToString();    // cbLI_No
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Adresse de livraison (" + reader[0].ToString() + ") trouvé!");
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    list_of_supplier_info[14] = "0";    // LI_No
                                    list_of_supplier_info[15] = "NULL";    // cbLI_No
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse. ( list_of_supplier_info[14] = 0 && list_of_supplier_info[15] = NULL ) est null");
                                }
                            }
                        }
                    }
                    else
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ********************** Exception Supplier *********************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Le fournisseur : " + ref_supplier + " n'existe pas dans la base!");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : import annulé!");
                        logFileWriter.Flush();
                        connexion.Close(); //disconnect from database
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée. Le fournisseur : " + ref_supplier + " n'existe pas dans Sage!", "Le fournisseur : " + ref_supplier + " n'existe pas dans la BDD", "insertSupplierOrder() : ref_supplier == null", fileName, logFileName_import));
                        return null;
                    }

                    //Get the list of all Taxes (TVA)
                    //So i can calculate the ttc later
                    List<TVA> tvaList = null;
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Récupére tous les tva");
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.getAllTVA(true));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getAllTVA(true), connexion))
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                tvaList = new List<TVA>();
                                tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                while (reader.Read())
                                {
                                    tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                }
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                tvaList = null;
                                logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse. ");
                            }
                        }
                    }


                    if (list_of_supplier_info != null)
                    {
                        //get veolog delivery date and time
                        string year = dh.Date_De_Reception.Substring(0, 4);
                        string month = dh.Date_De_Reception.Substring(4, 2);
                        string day = dh.Date_De_Reception.Substring(6, 2);
                        string hour = dh.Heure_De_Reception.Substring(0, 2);
                        string mins = dh.Heure_De_Reception.Substring(2, 2);
                        string veologDeliveryDate = year + "-" + month + "-" + day;
                        string veologDeliveryTime = hour + ":" + mins + ":00";
                        string veologDeliveryDateTime = veologDeliveryDate + " " + veologDeliveryTime;

                        int counter = 0;

                        foreach (Veolog_BCF_Lines line in dl) //read item by item
                        {
                            string ref_article = "";
                            string name_article = "";
                            string DL_PoidsNet = "0";
                            string DL_PoidsBrut = "0";
                            //string DL_PrixUnitaire_buyPrice = "0";
                            string DL_PrixUnitaire_salePriceHT = "0";
                            string DL_PUTTC = "0";
                            string COLIS_article = "";
                            string PCB_article = "";
                            string COMPLEMENT_article = "";
                            string DL_Taxe1 = "";
                            string DL_CodeTaxe1 = "";
                            string DL_PieceBCF = "";
                            string DL_DateBCF = "";
                            string DL_QteBCF = "";

                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Lire la ligne de l'article.");

                            //get Product Name By Reference
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.getProductNameByReference_BLF(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article));
                            using (OdbcCommand command = new OdbcCommand(QueryHelper.getProductNameByReference_BLF(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                            {
                                using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                {
                                    if (reader.Read()) // If any rows returned
                                    {
                                        ref_article = (reader[0].ToString());                   // get product ref
                                        name_article = (reader[1].ToString());                  // sum up the total_negative variable. - check query
                                        DL_PoidsNet = (reader[2].ToString());                   // get unit weight NET - check query
                                        DL_PoidsBrut = (reader[3].ToString());                  // get unit weight BRUT - check query
                                        DL_PrixUnitaire_salePriceHT = (reader[4].ToString());   // get (Prix de vente) unit price ht - check query
                                        COLIS_article = reader[5].ToString();
                                        PCB_article = reader[6].ToString();
                                        COMPLEMENT_article = reader[7].ToString();
                                        DL_Taxe1 = reader[8].ToString();
                                        DL_CodeTaxe1 = reader[9].ToString();
                                        DL_PieceBCF = reader[10].ToString();
                                        DL_DateBCF = reader[11].ToString();
                                        DL_QteBCF = reader[12].ToString().Replace(",", ".");
                                    }
                                    else// If no rows returned
                                    {
                                        //do nothing.
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse. ");
                                        logFileWriter.Flush();
                                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée. L'article \"" + line.Code_Article + "\" n'existe pas dans le BCF " + dh.Ref_Commande_Donneur_Ordre, "L'article \"" + line.Code_Article + "\" n'existe pas dans la commande " + dh.Ref_Commande_Donneur_Ordre, "", fileName, logFileName_import));
                                        return null;
                                    }
                                }
                            }


                            if (ref_article != "" && name_article != "" && list_of_supplier_info != null)
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Article trouvé.");
                                logFileWriter.WriteLine("");

                                try
                                {
                                    //DL_Ligne
                                    position_item += 1000;

                                    //calculate product ttc
                                    double product_ttc = 0.0;
                                    try
                                    {
                                        logFileWriter.WriteLine("");
                                        if (tvaList != null)
                                        {
                                            TVA tva = null;
                                            if (DL_CodeTaxe1 == null || DL_CodeTaxe1 == "")
                                            {
                                                foreach (TVA tva_ in tvaList)
                                                {
                                                    if (tva_.TA_Code == "C00")
                                                    {
                                                        tva = tva_;
                                                        DL_CodeTaxe1 = tva_.TA_Code;
                                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TA_Code trouvé C00");
                                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA trouvé \"" + tva.TA_Taux + "\"");
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (TVA tva_ in tvaList)
                                                {
                                                    if (tva_.TA_Code == DL_CodeTaxe1)
                                                    {
                                                        tva = tva_;
                                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TA_Code trouvé \"" + tva.TA_Taux + "\"");
                                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA trouvé \"" + tva.TA_Taux + "\"");
                                                        break;
                                                    }
                                                }
                                            }


                                            double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                            double product_20_P = (product_ht * Convert.ToDouble(tva.TA_Taux)) / 100;
                                            product_ttc = product_ht + product_20_P;
                                            DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Prix TTC créé");
                                        }
                                        else
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ******************** Warning TVA ********************");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Liste des tva non trouvée, tous les tva et prix ttc de chaque produit dans ce BL seront 0");

                                            double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                            double product_20_P = (product_ht * 0.0) / 100;
                                            product_ttc = product_ht + product_20_P;
                                            DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Prix TTC créé");
                                        }
                                        logFileWriter.Flush();
                                    }
                                    catch (Exception ex)
                                    {
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ******************** Exception TVA ********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Erreur lors du calcule du prix d'article TTC, message : \n" + ex.Message);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :" + ex.StackTrace);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Import annulée");
                                        logFileWriter.Flush();
                                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                        return null;
                                    }

                                    // input DL_DateBC: 03 - 01 - 2020 00:00:00;
                                    string original_date = DL_DateBCF;
                                    string date = original_date.Split(' ')[0];
                                    string time = original_date.Split(' ')[1];
                                    DL_DateBCF = date.Split('/')[2] + "-" + date.Split('/')[1] + "-" + date.Split('/')[0] + " " + time;
                                    // ouput DL_DateBC: 2020-01-03 00:00:00;

                                    // DESADV prefix will be used to create document
                                    list_of_cmd_lines[counter, 0] = "1"; // DO_Domaine
                                    list_of_cmd_lines[counter, 1] = "13"; //DO_Type
                                    list_of_cmd_lines[counter, 2] = "13"; //DO_DocType
                                    list_of_cmd_lines[counter, 3] = list_of_supplier_info[0]; //CT_NUM
                                    list_of_cmd_lines[counter, 4] = reference_BLF_doc; //DO_Piece
                                    list_of_cmd_lines[counter, 5] = curr_date; //DO_Date
                                    list_of_cmd_lines[counter, 6] = DL_DateBCF; //DL_DateBC
                                    list_of_cmd_lines[counter, 7] = (position_item).ToString(); // DL_Ligne line number 1000,2000
                                    list_of_cmd_lines[counter, 8] = dh.Ref_Commande_Fournisseur; // DO_Ref
                                    list_of_cmd_lines[counter, 9] = ref_article; // AR_Ref
                                    list_of_cmd_lines[counter, 10] = "1"; //DL_Valorise
                                    list_of_cmd_lines[counter, 11] = "1"; //DE_NO
                                    list_of_cmd_lines[counter, 12] = name_article.Replace("'", "''"); // DL_Design
                                    list_of_cmd_lines[counter, 13] = Convert.ToInt16(line.Quantite).ToString().Replace(",", ".");  //line.Quantite_Colis; // DL_Qte
                                    list_of_cmd_lines[counter, 14] = Convert.ToDouble(DL_PoidsNet).ToString().Replace(",", "."); // DL_PoidsNet
                                    if (list_of_cmd_lines[counter, 14].Equals("0")) { list_of_cmd_lines[counter, 14] = "0.000000"; } else if (!list_of_cmd_lines[counter, 14].Contains(".")) { list_of_cmd_lines[counter, 14] = list_of_cmd_lines[counter, 14] + ".000000"; }

                                    list_of_cmd_lines[counter, 15] = Convert.ToDouble(DL_PoidsBrut).ToString().Replace(",", "."); // DL_PoidsBrut
                                    if (list_of_cmd_lines[counter, 15].Equals("0")) { list_of_cmd_lines[counter, 15] = "0.000000"; } else if (!list_of_cmd_lines[counter, 15].Contains(".")) { list_of_cmd_lines[counter, 15] = list_of_cmd_lines[counter, 15] + ".000000"; }

                                    list_of_cmd_lines[counter, 16] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_PrixUnitaire
                                    if (list_of_cmd_lines[counter, 16].Equals("0")) { list_of_cmd_lines[counter, 16] = "0.000000"; } else if (!list_of_cmd_lines[counter, 16].Contains(".")) { list_of_cmd_lines[counter, 16] = list_of_cmd_lines[counter, 16] + ".000000"; }

                                    list_of_cmd_lines[counter, 17] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_PrixRU
                                    if (list_of_cmd_lines[counter, 17].Equals("0")) { list_of_cmd_lines[counter, 17] = "0.000000"; } else if (!list_of_cmd_lines[counter, 17].Contains(".")) { list_of_cmd_lines[counter, 17] = list_of_cmd_lines[counter, 17] + ".000000"; }

                                    list_of_cmd_lines[counter, 18] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_CMUP
                                    list_of_cmd_lines[counter, 19] = "Heure";    //DL_PrixUnitaire.ToString().Replace(",", "."); // EU_Enumere
                                    list_of_cmd_lines[counter, 20] = Convert.ToInt16(line.Quantite).ToString().Replace(",", "."); // EU_Qte; // EU_Qte
                                    if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }

                                    list_of_cmd_lines[counter, 21] = (Convert.ToDouble(line.Quantite) * Convert.ToDouble(DL_PrixUnitaire_salePriceHT)).ToString().Replace(",", "."); //DL_MontantHT
                                    list_of_cmd_lines[counter, 22] = (Convert.ToDouble(line.Quantite) * product_ttc).ToString().Replace(",", "."); //DL_MontantTTC
                                    if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }
                                    if (list_of_cmd_lines[counter, 21].Equals("0")) { list_of_cmd_lines[counter, 21] = "0.000000"; } else if (!list_of_cmd_lines[counter, 21].Contains(".")) { list_of_cmd_lines[counter, 21] = list_of_cmd_lines[counter, 21] + ".0"; }
                                    if (list_of_cmd_lines[counter, 22].Equals("0")) { list_of_cmd_lines[counter, 22] = "0.000000"; } else if (!list_of_cmd_lines[counter, 22].Contains(".")) { list_of_cmd_lines[counter, 22] = list_of_cmd_lines[counter, 22] + ".000000"; }

                                    list_of_cmd_lines[counter, 23] = ""; //PF_Num
                                    list_of_cmd_lines[counter, 24] = "0"; //DL_No
                                    list_of_cmd_lines[counter, 25] = "0"; //DL_FactPoids
                                    list_of_cmd_lines[counter, 26] = "0"; //DL_Escompte
                                    list_of_cmd_lines[counter, 27] = DL_PUTTC; //DL_PUTTC
                                    list_of_cmd_lines[counter, 28] = "0";   //DL_TTC

                                    list_of_cmd_lines[counter, 29] = DL_PieceBCF;   //DL_PieceBC
                                    list_of_cmd_lines[counter, 30] = reference_BLF_doc;   //DL_PieceBL
                                    list_of_cmd_lines[counter, 31] = curr_date;   // DL_DateBL
                                    list_of_cmd_lines[counter, 32] = "0";   //DL_TNomencl
                                    list_of_cmd_lines[counter, 33] = "0";   //DL_TRemPied
                                    list_of_cmd_lines[counter, 34] = "0";   //DL_TRemExep
                                    list_of_cmd_lines[counter, 35] = DL_QteBCF.Replace(",", ".");   //DL_QteBC
                                    list_of_cmd_lines[counter, 36] = Convert.ToInt16(line.Quantite).ToString().Replace(",", ".");   //DL_QteBL
                                    list_of_cmd_lines[counter, 37] = "0.000000";    //DL_Remise01REM_Valeur
                                    list_of_cmd_lines[counter, 38] = "0";           //DL_Remise01REM_Type
                                    list_of_cmd_lines[counter, 39] = "0.000000";    //DL_Remise02REM_Valeur
                                    list_of_cmd_lines[counter, 40] = "0";           //DL_Remise02REM_Type
                                    list_of_cmd_lines[counter, 41] = "0.000000";    //DL_Remise03REM_Valeur
                                    list_of_cmd_lines[counter, 42] = "0";           //DL_Remise03REM_Type
                                    list_of_cmd_lines[counter, 43] = "1";                   //DL_NoRef
                                    list_of_cmd_lines[counter, 44] = "0";                   //DL_TypePL
                                    list_of_cmd_lines[counter, 45] = "0.000000";            //DL_PUDevise
                                    list_of_cmd_lines[counter, 46] = "";                    //CA_Num
                                    list_of_cmd_lines[counter, 47] = "0.000000";            //DL_Frais
                                    list_of_cmd_lines[counter, 48] = "";                    //AC_RefClient
                                    list_of_cmd_lines[counter, 49] = "";                   //DL_PiecePL
                                    list_of_cmd_lines[counter, 50] = "NULL"; //curr_date;                    //DL_DatePL
                                    list_of_cmd_lines[counter, 51] = "0.000000"; //Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");                   //DL_QtePL
                                    list_of_cmd_lines[counter, 52] = "";                    //DL_NoColis
                                    list_of_cmd_lines[counter, 53] = "0";                   //DL_NoLink
                                    list_of_cmd_lines[counter, 54] = "0";                    //CO_No
                                    list_of_cmd_lines[counter, 55] = "0";                //DT_No
                                    list_of_cmd_lines[counter, 56] = "";                    //DL_PieceDE
                                    list_of_cmd_lines[counter, 57] = "NULL";                   //DL_DateDe
                                    list_of_cmd_lines[counter, 58] = "0.000000"; //Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");                    //DL_QteDE
                                    list_of_cmd_lines[counter, 59] = "0";                  //DL_NoSousTotal
                                    list_of_cmd_lines[counter, 60] = "0";                //CA_No
                                    list_of_cmd_lines[counter, 61] = "0.000000";            // DL_PUBC
                                    list_of_cmd_lines[counter, 62] = DL_CodeTaxe1;                  // DL_CodeTaxe1
                                    list_of_cmd_lines[counter, 63] = DL_Taxe1.ToString().Replace(",", ".");         // DL_Taxe1
                                    list_of_cmd_lines[counter, 64] = "0.000000";            // DL_Taxe2
                                    list_of_cmd_lines[counter, 65] = "0.000000";            // DL_Taxe3
                                    list_of_cmd_lines[counter, 66] = "0";                   // DL_TypeTaux1
                                    list_of_cmd_lines[counter, 67] = "0";                   // DL_TypeTaxe1
                                    list_of_cmd_lines[counter, 68] = "0";                   // DL_TypeTaux2
                                    list_of_cmd_lines[counter, 69] = "0";                   // DL_TypeTaxe2
                                    list_of_cmd_lines[counter, 70] = "0";                   // DL_TypeTaux3
                                    list_of_cmd_lines[counter, 71] = "0";                   // DL_TypeTaxe3

                                    list_of_cmd_lines[counter, 72] = "1";                                           // DL_MvtStock
                                    list_of_cmd_lines[counter, 73] = "";                                            // AF_RefFourniss
                                    list_of_cmd_lines[counter, 74] = ((COLIS_article == null || COLIS_article == "") ? "0.0" : COLIS_article).ToString().Replace(",", ".");    // COLIS
                                    list_of_cmd_lines[counter, 75] = ((PCB_article == null || PCB_article == "") ? "0.0" : PCB_article).ToString().Replace(",", ".");      // PCB
                                    list_of_cmd_lines[counter, 76] = COMPLEMENT_article;                            // COMPLEMENT
                                    list_of_cmd_lines[counter, 77] = "";                                            // PourVeolog
                                    list_of_cmd_lines[counter, 78] = "";                                            // DL_PieceOFProd
                                    list_of_cmd_lines[counter, 79] = "";                                            // DL_Operation
                                    list_of_cmd_lines[counter, 80] = veologDeliveryDateTime;    //DO_DateLivr
                                    list_of_cmd_lines[counter, 81] = "0";    //DL_NonLivre

                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Exception : 2D table not working properly.\r\n" + ex.Message);
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ******************** Exception Line ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Le tableau 'BLF' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", "Le tableau 'BLF' à 2 dimensions ne fonctionne pas correctement, message: " + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }
                            }

                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Compter => " + counter);
                            counter++;

                        }
                        // ===== End Foreach =====

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Vérifier si un produit pour 1 = BLF");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertBcfDocument_Veolog(true, "13", reference_BLF_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_supplier_info));

                        //generate document BLF_____. in database.
                        try
                        {
                            OdbcCommand command = new OdbcCommand(QueryHelper.insertBcfDocument_Veolog(true, "13", reference_BLF_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_supplier_info), connexion); //calling the query and parsing the parameters into it
                            command.ExecuteReader(); // executing the query
                        }
                        catch (OdbcException ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ********************** OdbcException *********************");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Message :" + ex.Message);
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :" + ex.StackTrace);
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Import annulée");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }


                        string[,] products_BCF = new string[position_item / 1000, 82]; // create array with enough space

                        //insert documentline into the database with articles having 20 as value @index 2
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : insert documentline into the database with articles having 13 as value @index 2");

                        for (int x = 0; x < list_of_cmd_lines.GetLength(0); x++)
                        {
                            if (list_of_cmd_lines[x, 1] == "13")
                            {
                                for (int y = 0; y < list_of_cmd_lines.GetLength(1); y++)
                                {
                                    products_BCF[x, y] = list_of_cmd_lines[x, y];
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : products_BLF_L[" + x + "," + y + "] = " + products_BCF[x, y]);
                                }

                                //insert the article to documentline in the database
                                try
                                {
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : insert the article " + products_BCF[x, 12] + " (Ref:" + products_BCF[x, 9] + ") to documentline in the database");

                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : requette sql ===> " + QueryHelper.insertBcfDocumentLine_Veolog(true, products_BCF, x));

                                    OdbcCommand command = new OdbcCommand(QueryHelper.insertBcfDocumentLine_Veolog(true, products_BCF, x), connexion);
                                    command.ExecuteReader();
                                }
                                catch (OdbcException ex)
                                {
                                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ********************** OdbcException *********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }

                                //Update docline artile stock
                                try
                                {
                                    bool found_stock = false;
                                    double AS_StockReel = 0.0;
                                    double AS_StockReserve = 0.0;
                                    double AS_StockMontant = 0.0;
                                    double productPrixUnite = Convert.ToDouble(products_BCF[x, 16].Replace('.', ','));
                                    double AS_StockCommande = 0.0;

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : get current stock in F_ARTSTOCK table in the database");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : requette sql ===> " + QueryHelper.getArticleStock(true, products_BCF[x, 9]));
                                    using (OdbcCommand command_ = new OdbcCommand(QueryHelper.getArticleStock(true, products_BCF[x, 9]), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                                    {
                                        using (IDataReader reader = command_.ExecuteReader()) //read rows of the executed query
                                        {
                                            if (reader.Read()) // If any rows returned
                                            {
                                                found_stock = true;
                                                logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Stock trouvé : AS_StockReel (" + reader[0].ToString() + "), AS_StockReserve (" + reader[1].ToString() + "), AS_StockMontant (" + reader[2].ToString() + "), AS_StockCommande (" + reader[3].ToString() + ").");
                                                AS_StockReel = Convert.ToDouble(reader[0].ToString().Replace(".", ","));
                                                AS_StockReserve = Convert.ToDouble(reader[1].ToString().Replace(".", ","));
                                                AS_StockMontant = Convert.ToDouble(reader[2].ToString().Replace(".", ","));
                                                AS_StockCommande = Convert.ToDouble(reader[3].ToString().Replace(".", ","));
                                            }
                                            else// If no rows returned
                                            {
                                                //do nothing.
                                                found_stock = false;
                                                logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse.");
                                                logFileWriter.Flush();
                                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", "Echec à la récupération du stock de l'article " + products_BCF[x, 9], "insertSupplierOrder() : Aucune reponse.", fileName, logFileName_import));
                                                return null;
                                            }
                                        }
                                    }


                                    if (found_stock)
                                    {
                                        //Calculate stock info
                                        double AS_CMUP = AS_StockMontant / AS_StockReel;
                                        double new_AS_StockReel = AS_StockReel + Convert.ToDouble(products_BCF[x, 13].Replace('.', ','));
                                        //double new_AS_StockReserve = AS_StockReserve - Convert.ToDouble(products_BCF[x, 13].Replace('.', ','));
                                        double new_AS_StockCommande = AS_StockCommande - Convert.ToDouble(products_BCF[x, 13].Replace('.', ','));
                                        double new_AS_StockMontant = new_AS_StockReel * productPrixUnite;
                                        double new_AS_CMUP = new_AS_StockMontant / new_AS_StockReel;

                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Calculation...");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : new_AS_StockReel: " + new_AS_StockReel + " = AS_StockReel: " + AS_StockReel + " + products_DESADV[x, 13]: " + Convert.ToDouble(products_BCF[x, 13].Replace('.', ',')));
                                        //logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : new_AS_StockReserve: " + new_AS_StockReserve + " = AS_StockReserve: " + AS_StockReserve + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_BCF[x, 13].Replace('.', ',')));
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : new_AS_StockCommande: " + new_AS_StockCommande + " = AS_StockCommande: " + AS_StockCommande + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_BCF[x, 13].Replace('.', ',')));
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : new_AS_StockMontant: " + new_AS_StockMontant + " = new_AS_StockReel: " + new_AS_StockReel + " X productPrixUnite: " + productPrixUnite);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : new_AS_CMUP: " + new_AS_CMUP + " = new_AS_StockMontant: " + new_AS_StockMontant + " / new_AS_StockReel: " + new_AS_StockReel);

                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : update article " + products_BCF[x, 12] + " (Ref:" + products_BCF[x, 9] + ") stock in F_ARTSTOCK table in the database");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : requette sql ===> " + QueryHelper.updateArticleStockBLF(true, products_BCF[x, 9], new_AS_StockReel, new_AS_StockCommande, new_AS_StockMontant));

                                        OdbcCommand command = new OdbcCommand(QueryHelper.updateArticleStockBLF(true, products_BCF[x, 9], new_AS_StockReel, new_AS_StockCommande, new_AS_StockMontant), connexion);
                                        command.ExecuteReader();
                                    }
                                }
                                catch (OdbcException ex)
                                {
                                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ********************** OdbcException Update F_ARTSTOCK table *********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        //set Veolog date time import
                        try
                        {
                            string delivery_date_veolog = string.Format("{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Ajouter la date de livraision \"" + delivery_date_veolog + "\" de Veolog au BLF \"" + reference_BLF_doc + "\".");

                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.updateVeologDeliveryDate(true, reference_BLF_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre));
                            OdbcCommand command = new OdbcCommand(QueryHelper.updateVeologDeliveryDate(true, reference_BLF_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre), connexion);
                            {
                                using (IDataReader reader = command.ExecuteReader())
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Date de livraison veolog à jour !");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur Veolog Date BCF ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }

                        //update document numbering
                        try
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Mettre à jour la numérotation du document \"" + reference_BLF_doc + "\".");

                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.updateDOC_NumerotationTable(true, ((reference_BLF_doc == "BLF") ? "BLF" : "LF"), reference_BLF_doc));
                            OdbcCommand command = new OdbcCommand(QueryHelper.updateDOC_NumerotationTable(true, ((reference_BLF_doc == "BLF") ? "BLF" : "LF"), reference_BLF_doc), connexion);
                            IDataReader reader = command.ExecuteReader();
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Nouvelle numérotation à jour!");
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }

                        //Delete the BC of the BL
                        try
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Supprimer le Bon de Commande (BCF) \"" + dh.Ref_Commande_Donneur_Ordre + "\".");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre));
                            OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre), connexion);
                            IDataReader reader = command.ExecuteReader();
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Bon de Commande Fournisseur supprimé!");
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ********************** Exception 1 *********************");
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Message :: " + ex.Message);
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :: " + ex.StackTrace);
                    connexion.Close(); //disconnect from database
                    logFileWriter.Flush();
                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                    return null;
                }
            }

            return list_of_cmd_lines;
        }


        public static string[,] insertSupplierOrder_w_Reliquat(string reference_BLF_doc, Veolog_BCF dh, List<Veolog_BCF_Lines> dl, string fileName, StreamWriter logFileWriter, string logFileName_import, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
        {
            string METHODE_NAME = "insertSupplierOrder_w_Reliquat()";
            string[,] list_of_cmd_lines = new string[dl.Count, 82];    // new string [x,y]
            string[] list_of_supplier_info = null;
            int position_item = 0;
            DateTime d = DateTime.Now;
            string curr_date = d.ToString("yyyy-MM-dd");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;

            using (OdbcConnection connexion = ConnexionManager.CreateOdbcConnexionSQL()) //connecting to database as handler
            {
                try
                {
                    // Get all tva
                    // Mapping of delivery date
                    // GET CMD
                    // Check if CMD is not reliquat
                    //      ||=> import the document
                    // if CMD is Reliquat
                    //      ||=> 

                    connexion.Open();
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Connexion ouverte.");

                    //Get the list of all Taxes (TVA)
                    //So i can calculate the ttc later
                    List<TVA> tvaList = null;
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Récupére tous les tva");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getAllTVA(true));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getAllTVA(true), connexion))
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                tvaList = new List<TVA>();
                                tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                while (reader.Read())
                                {
                                    tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                }
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                tvaList = null;
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucune reponse. ");
                            }
                        }
                    }
                    logFileWriter.WriteLine("");
                    logFileWriter.Flush();

                    //get veolog delivery date and time
                    string veologDeliveryDateTime = "";
                    try
                    {
                        string year = dh.Date_De_Reception.Substring(0, 4);
                        string month = dh.Date_De_Reception.Substring(4, 2);
                        string day = dh.Date_De_Reception.Substring(6, 2);
                        string hour = dh.Heure_De_Reception.Substring(0, 2);
                        string mins = dh.Heure_De_Reception.Substring(2, 2);
                        if (dh.Heure_De_Reception != "" && dh.Heure_De_Reception.Length == 4)
                        {
                            hour = dh.Heure_De_Reception.Substring(0, 2);
                            mins = dh.Heure_De_Reception.Substring(2, 2);
                        }
                        string veologDeliveryDate = year + "-" + month + "-" + day;
                        string veologDeliveryTime = hour + ":" + mins + ":00";
                        veologDeliveryDateTime = veologDeliveryDate + " " + veologDeliveryTime;
                    }
                    catch (Exception e)
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ******************** Erreur Date/Heure de livraison ********************");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Message: " + e.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace: " + e.StackTrace);
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison est annulée.", e.Message, e.StackTrace, fileName, logFileName_import));
                        return null;
                    }
                    logFileWriter.Flush();

                    // Get CMD info
                    string nature_op_p_ = "";
                    string do_totalHT_ = "";
                    string do_totalHTNet_ = "";
                    string do_totalTTC_ = "";
                    string do_NetAPayer_ = "";
                    string do_MontantRegle_ = "";
                    string ref_supplier = null;
                    string DO_Reliquat = "0"; // 0 => Non Reliquat | 1 => Un Reliquat
                    int total_lines = 0; // number of lines in the order

                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Récupérer la référence Commande fournisseur livré de la commande " + dh.Ref_Commande_Donneur_Ordre);
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getCMDSupplierByRef(true, dh.Ref_Commande_Donneur_Ordre));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getCMDSupplierByRef(true, dh.Ref_Commande_Donneur_Ordre), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                dh.Ref_Commande_Fournisseur = reader[0].ToString();
                                nature_op_p_ = reader[1].ToString();
                                do_totalHT_ = reader[2].ToString().Replace(",", ".");
                                do_totalHTNet_ = reader[3].ToString().Replace(",", ".");
                                do_totalTTC_ = reader[4].ToString().Replace(",", ".");
                                do_NetAPayer_ = reader[5].ToString().Replace(",", ".");
                                do_MontantRegle_ = reader[6].ToString().Replace(",", ".");
                                ref_supplier = reader[7].ToString();
                                DO_Reliquat = reader[8].ToString();
                                total_lines = (reader[9].ToString() != null && reader[9].ToString() != "null" && reader[9].ToString() != "NULL" && reader[9].ToString() != "" ? Convert.ToInt16(reader[9].ToString()) : 0);
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucune reponse. ");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", "Aucun BCF trouvé dans la BDD", "" + METHODE_NAME + " : Aucune reponse", fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    //get supplier Reference by Ref
                    logFileWriter.WriteLine("");
                    logFileWriter.Flush();
                    if (ref_supplier != null)
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getClientReferenceById_DESADV(true, ref_supplier));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceById_DESADV(true, ref_supplier), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_supplier_info = new string[16];
                                    list_of_supplier_info[0] = reader[0].ToString();      // CT_Num
                                    list_of_supplier_info[1] = reader[1].ToString();      // CA_Num 
                                    list_of_supplier_info[2] = reader[2].ToString();      // CG_NumPrinc
                                    list_of_supplier_info[3] = reader[3].ToString();      // CT_NumPayeur
                                    list_of_supplier_info[4] = reader[4].ToString();      // N_Condition
                                    list_of_supplier_info[5] = reader[5].ToString();      // N_Devise
                                    list_of_supplier_info[6] = reader[6].ToString();      // CT_Langue
                                    list_of_supplier_info[7] = reader[7].ToString();      // DO_NbFacture = CT_Facture
                                    list_of_supplier_info[8] = reader[8].ToString().Replace(',', '.');      // DO_TxEscompte = CT_Taux02
                                    list_of_supplier_info[9] = reader[9].ToString();      // N_CatCompta
                                    list_of_supplier_info[10] = reader[10].ToString();    // CO_No
                                    list_of_supplier_info[11] = reader[11].ToString();    //  DO_Tarif = N_CatTarif
                                    list_of_supplier_info[12] = reader[12].ToString();    //  DO_Expedit = N_Expedition du tier
                                    list_of_supplier_info[13] = reader[13].ToString();    //  CT_Intitule
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucune reponse. list_of_fournisseur_info est null");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée. Le fournisseur \"" + ref_supplier + "\" n'existe pas dans Sage", "Le fournisseur \"" + ref_supplier + "\" n'existe pas dans Sage", "" + METHODE_NAME + " : Aucune reponse", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        // get supplier delivery adress
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getClientDeliveryAddress_DESADV(true, ref_supplier));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientDeliveryAddress_DESADV(true, ref_supplier), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_supplier_info[14] = reader[0].ToString();    // LI_No
                                    list_of_supplier_info[15] = reader[0].ToString();    // cbLI_No
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Adresse de livraison (" + reader[0].ToString() + ") trouvé!");
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    list_of_supplier_info[14] = "0";    // LI_No
                                    list_of_supplier_info[15] = "NULL";    // cbLI_No
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucune reponse. ( list_of_supplier_info[14] = 0 && list_of_supplier_info[15] = NULL ) est null");
                                }
                            }
                        }
                    }
                    else
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ********************** Exception Supplier *********************");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Le fournisseur : " + ref_supplier + " n'existe pas dans la base!");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : import annulé!");
                        logFileWriter.Flush();
                        connexion.Close(); //disconnect from database
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée. Le fournisseur : " + ref_supplier + " n'existe pas dans Sage!", "Le fournisseur : " + ref_supplier + " n'existe pas dans la BDD", "" + METHODE_NAME + " : ref_supplier == null", fileName, logFileName_import));
                        return null;
                    }
                    logFileWriter.WriteLine("");
                    logFileWriter.Flush();


                    if (list_of_supplier_info == null)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ********************** Exception Supplier *********************");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Le fournisseur : " + ref_supplier + " n'existe pas dans la base!");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : import annulé!");
                        logFileWriter.Flush();
                        connexion.Close(); //disconnect from database
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée. Le fournisseur : " + ref_supplier + " n'existe pas dans Sage!", "Le fournisseur : " + ref_supplier + " n'existe pas dans la BDD", "" + METHODE_NAME + " : ref_supplier == null", fileName, logFileName_import));
                        return null;
                    }


                    logFileWriter.Flush();
                    if (DO_Reliquat.Equals("0"))        // BL normal
                    {

                    }
                    else if (DO_Reliquat.Equals("1"))  // BL Reliquat
                    {

                    }

                    bool isNeedReliquat = false;
                    bool isLastBlReliquat = true;
                    List<string> validProducts = new List<string>();
                    List<DocumentAchatLine> produitReliquat = new List<DocumentAchatLine>();
                    int counter = 0;

                    foreach (Veolog_BCF_Lines line in dl) //read item by item
                    {
                        string ref_article = "";
                        string name_article = "";
                        string DL_PoidsNet = "0";
                        string DL_PoidsBrut = "0";
                        //string DL_PrixUnitaire_buyPrice = "0";
                        string DL_PrixUnitaire_salePriceHT = "0";
                        string DL_PUTTC = "0";
                        string COLIS_article = "";
                        string PCB_article = "";
                        string COMPLEMENT_article = "";
                        string DL_Taxe1 = "";
                        string DL_CodeTaxe1 = "";
                        string DL_PieceBCF = "";
                        string DL_DateBCF = "";
                        string DL_QteBCF = "";
                        string DL_QtePLF = ""; //Qte Livrées

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Lire la ligne de l'article.");

                        //get Product Name By Reference
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getProductNameByReference_BLF(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getProductNameByReference_BLF(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    ref_article = (reader[0].ToString());                   // get product ref
                                    name_article = (reader[1].ToString());                  // sum up the total_negative variable. - check query
                                    DL_PoidsNet = (reader[2].ToString());                   // get unit weight NET - check query
                                    DL_PoidsBrut = (reader[3].ToString());                  // get unit weight BRUT - check query
                                    DL_PrixUnitaire_salePriceHT = (reader[4].ToString());   // get (Prix de vente) unit price ht - check query
                                    COLIS_article = reader[5].ToString();
                                    PCB_article = reader[6].ToString();
                                    COMPLEMENT_article = reader[7].ToString();
                                    DL_Taxe1 = reader[8].ToString();
                                    DL_CodeTaxe1 = reader[9].ToString();
                                    DL_PieceBCF = reader[10].ToString();
                                    DL_DateBCF = reader[11].ToString();
                                    DL_QteBCF = reader[12].ToString().Replace(",", ".");
                                    DL_QtePLF = reader[13].ToString().Replace(",", ".");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Info du produit " + line.Code_Article + " trouvé");
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : L'article \"" + line.Code_Article + "\" n'est pas trouvé dans la commande " + dh.Ref_Commande_Donneur_Ordre + ".");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Alors je saute cette article.");
                                    logFileWriter.Flush();
                                    // recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée. L'article \"" + line.Code_Article + "\" n'existe pas dans le BCF " + dh.Ref_Commande_Donneur_Ordre, "L'article \"" + line.Code_Article + "\" n'existe pas dans la commande " + dh.Ref_Commande_Donneur_Ordre, "", fileName, logFileName_import));
                                    continue;
                                }
                            }
                        }


                        // Check if we may get a BLF Reliquat
                        if (DL_QteBCF.Split('.')[0].Equals(line.Quantite))
                        {
                            // continue import normally
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Article => " + line.Code_Article);
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Qte commandé : " + DL_QteBCF.Split('.')[0] + " == Qte livrer : " + line.Quantite);
                            logFileWriter.WriteLine("");
                        }
                        else
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Article => " + line.Code_Article);
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Qte commandé : " + DL_QteBCF.Split('.')[0] + " != Qte livrer : " + line.Quantite);
                            logFileWriter.WriteLine("");

                            DO_Reliquat = "1";
                            isLastBlReliquat = false;
                            DocumentAchatLine doc = new DocumentAchatLine();
                            doc.AR_Ref = ref_article;
                            doc.AR_CODEBARRE = line.Code_Article;
                            doc.DL_Qte = (Convert.ToInt32(DL_QteBCF.Split('.')[0]) - Convert.ToInt32(line.Quantite)).ToString();
                            doc.DL_QtePL = (Convert.ToInt32(DL_QteBCF.Split('.')[0]) - Convert.ToInt32(line.Quantite)).ToString();
                            doc.EU_Qte = (Convert.ToInt32(DL_QteBCF.Split('.')[0]) - Convert.ToInt32(line.Quantite)).ToString();

                            produitReliquat.Add(doc);
                        }


                        if (ref_article != "" && name_article != "" && list_of_supplier_info != null)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Article trouvé.");
                            logFileWriter.WriteLine("");

                            try
                            {
                                //DL_Ligne
                                position_item += 1000;

                                //calculate product ttc
                                double product_ttc = 0.0;
                                try
                                {
                                    logFileWriter.WriteLine("");
                                    if (tvaList != null)
                                    {
                                        TVA tva = null;
                                        if (DL_CodeTaxe1 == null || DL_CodeTaxe1 == "")
                                        {
                                            foreach (TVA tva_ in tvaList)
                                            {
                                                if (tva_.TA_Code == "C00")
                                                {
                                                    tva = tva_;
                                                    DL_CodeTaxe1 = tva_.TA_Code;
                                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : TA_Code trouvé C00");
                                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : TVA trouvé \"" + tva.TA_Taux + "\"");
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (TVA tva_ in tvaList)
                                            {
                                                if (tva_.TA_Code == DL_CodeTaxe1)
                                                {
                                                    tva = tva_;
                                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : TA_Code trouvé \"" + tva.TA_Taux + "\"");
                                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : TVA trouvé \"" + tva.TA_Taux + "\"");
                                                    break;
                                                }
                                            }
                                        }


                                        double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                        double product_20_P = (product_ht * Convert.ToDouble(tva.TA_Taux)) / 100;
                                        product_ttc = product_ht + product_20_P;
                                        DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Prix TTC créé");
                                    }
                                    else
                                    {
                                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ******************** Warning TVA ********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Liste des tva non trouvée, tous les tva et prix ttc de chaque produit dans ce BL seront 0");

                                        double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                        double product_20_P = (product_ht * 0.0) / 100;
                                        product_ttc = product_ht + product_20_P;
                                        DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Prix TTC créé");
                                    }
                                    logFileWriter.Flush();
                                }
                                catch (Exception ex)
                                {
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ******************** Exception TVA ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Erreur lors du calcule du prix d'article TTC, message : \n" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }

                                // input DL_DateBC: 03 - 01 - 2020 00:00:00;
                                string original_date = DL_DateBCF;
                                string date = original_date.Split(' ')[0];
                                string time = original_date.Split(' ')[1];
                                DL_DateBCF = date.Split('/')[2] + "-" + date.Split('/')[1] + "-" + date.Split('/')[0] + " " + time;
                                // ouput DL_DateBC: 2020-01-03 00:00:00;

                                // DESADV prefix will be used to create document
                                list_of_cmd_lines[counter, 0] = "1"; // DO_Domaine
                                list_of_cmd_lines[counter, 1] = "13"; //DO_Type
                                list_of_cmd_lines[counter, 2] = "13"; //DO_DocType
                                list_of_cmd_lines[counter, 3] = list_of_supplier_info[0]; //CT_NUM
                                list_of_cmd_lines[counter, 4] = reference_BLF_doc; //DO_Piece
                                list_of_cmd_lines[counter, 5] = curr_date; //DO_Date
                                list_of_cmd_lines[counter, 6] = DL_DateBCF; //DL_DateBC
                                list_of_cmd_lines[counter, 7] = (position_item).ToString(); // DL_Ligne line number 1000,2000
                                list_of_cmd_lines[counter, 8] = dh.Ref_Commande_Fournisseur; // DO_Ref
                                list_of_cmd_lines[counter, 9] = ref_article; // AR_Ref
                                list_of_cmd_lines[counter, 10] = "1"; //DL_Valorise
                                list_of_cmd_lines[counter, 11] = "1"; //DE_NO
                                list_of_cmd_lines[counter, 12] = name_article.Replace("'", "''"); // DL_Design
                                list_of_cmd_lines[counter, 13] = Convert.ToInt16(line.Quantite).ToString().Replace(",", ".");  //line.Quantite_Colis; // DL_Qte
                                list_of_cmd_lines[counter, 14] = Convert.ToDouble(DL_PoidsNet).ToString().Replace(",", "."); // DL_PoidsNet
                                if (list_of_cmd_lines[counter, 14].Equals("0")) { list_of_cmd_lines[counter, 14] = "0.000000"; } else if (!list_of_cmd_lines[counter, 14].Contains(".")) { list_of_cmd_lines[counter, 14] = list_of_cmd_lines[counter, 14] + ".000000"; }

                                list_of_cmd_lines[counter, 15] = Convert.ToDouble(DL_PoidsBrut).ToString().Replace(",", "."); // DL_PoidsBrut
                                if (list_of_cmd_lines[counter, 15].Equals("0")) { list_of_cmd_lines[counter, 15] = "0.000000"; } else if (!list_of_cmd_lines[counter, 15].Contains(".")) { list_of_cmd_lines[counter, 15] = list_of_cmd_lines[counter, 15] + ".000000"; }

                                list_of_cmd_lines[counter, 16] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_PrixUnitaire
                                if (list_of_cmd_lines[counter, 16].Equals("0")) { list_of_cmd_lines[counter, 16] = "0.000000"; } else if (!list_of_cmd_lines[counter, 16].Contains(".")) { list_of_cmd_lines[counter, 16] = list_of_cmd_lines[counter, 16] + ".000000"; }

                                list_of_cmd_lines[counter, 17] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_PrixRU
                                if (list_of_cmd_lines[counter, 17].Equals("0")) { list_of_cmd_lines[counter, 17] = "0.000000"; } else if (!list_of_cmd_lines[counter, 17].Contains(".")) { list_of_cmd_lines[counter, 17] = list_of_cmd_lines[counter, 17] + ".000000"; }

                                list_of_cmd_lines[counter, 18] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_CMUP
                                list_of_cmd_lines[counter, 19] = "Heure";    //DL_PrixUnitaire.ToString().Replace(",", "."); // EU_Enumere
                                list_of_cmd_lines[counter, 20] = Convert.ToInt16(line.Quantite).ToString().Replace(",", "."); // EU_Qte; // EU_Qte
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }

                                list_of_cmd_lines[counter, 21] = (Convert.ToDouble(line.Quantite) * Convert.ToDouble(DL_PrixUnitaire_salePriceHT)).ToString().Replace(",", "."); //DL_MontantHT
                                list_of_cmd_lines[counter, 22] = (Convert.ToDouble(line.Quantite) * product_ttc).ToString().Replace(",", "."); //DL_MontantTTC
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }
                                if (list_of_cmd_lines[counter, 21].Equals("0")) { list_of_cmd_lines[counter, 21] = "0.000000"; } else if (!list_of_cmd_lines[counter, 21].Contains(".")) { list_of_cmd_lines[counter, 21] = list_of_cmd_lines[counter, 21] + ".0"; }
                                if (list_of_cmd_lines[counter, 22].Equals("0")) { list_of_cmd_lines[counter, 22] = "0.000000"; } else if (!list_of_cmd_lines[counter, 22].Contains(".")) { list_of_cmd_lines[counter, 22] = list_of_cmd_lines[counter, 22] + ".000000"; }

                                list_of_cmd_lines[counter, 23] = ""; //PF_Num
                                list_of_cmd_lines[counter, 24] = "0"; //DL_No
                                list_of_cmd_lines[counter, 25] = "0"; //DL_FactPoids
                                list_of_cmd_lines[counter, 26] = "0"; //DL_Escompte
                                list_of_cmd_lines[counter, 27] = DL_PUTTC; //DL_PUTTC
                                list_of_cmd_lines[counter, 28] = "0";   //DL_TTC

                                list_of_cmd_lines[counter, 29] = DL_PieceBCF;   //DL_PieceBC
                                list_of_cmd_lines[counter, 30] = reference_BLF_doc;   //DL_PieceBL
                                list_of_cmd_lines[counter, 31] = curr_date;   // DL_DateBL
                                list_of_cmd_lines[counter, 32] = "0";   //DL_TNomencl
                                list_of_cmd_lines[counter, 33] = "0";   //DL_TRemPied
                                list_of_cmd_lines[counter, 34] = "0";   //DL_TRemExep
                                list_of_cmd_lines[counter, 35] = DL_QteBCF.Replace(",", ".");   //DL_QteBC
                                list_of_cmd_lines[counter, 36] = Convert.ToInt16(line.Quantite).ToString().Replace(",", ".");   //DL_QteBL
                                list_of_cmd_lines[counter, 37] = "0.000000";    //DL_Remise01REM_Valeur
                                list_of_cmd_lines[counter, 38] = "0";           //DL_Remise01REM_Type
                                list_of_cmd_lines[counter, 39] = "0.000000";    //DL_Remise02REM_Valeur
                                list_of_cmd_lines[counter, 40] = "0";           //DL_Remise02REM_Type
                                list_of_cmd_lines[counter, 41] = "0.000000";    //DL_Remise03REM_Valeur
                                list_of_cmd_lines[counter, 42] = "0";           //DL_Remise03REM_Type
                                list_of_cmd_lines[counter, 43] = "1";                   //DL_NoRef
                                list_of_cmd_lines[counter, 44] = "0";                   //DL_TypePL
                                list_of_cmd_lines[counter, 45] = "0.000000";            //DL_PUDevise
                                list_of_cmd_lines[counter, 46] = "";                    //CA_Num
                                list_of_cmd_lines[counter, 47] = "0.000000";            //DL_Frais
                                list_of_cmd_lines[counter, 48] = "";                    //AC_RefClient
                                list_of_cmd_lines[counter, 49] = "";                   //DL_PiecePL
                                list_of_cmd_lines[counter, 50] = "NULL"; //curr_date;                    //DL_DatePL
                                list_of_cmd_lines[counter, 51] = "0.000000"; //Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");                   //DL_QtePL
                                list_of_cmd_lines[counter, 52] = "";                    //DL_NoColis
                                list_of_cmd_lines[counter, 53] = "0";                   //DL_NoLink
                                list_of_cmd_lines[counter, 54] = "0";                    //CO_No
                                list_of_cmd_lines[counter, 55] = "0";                //DT_No
                                list_of_cmd_lines[counter, 56] = "";                    //DL_PieceDE
                                list_of_cmd_lines[counter, 57] = "NULL";                   //DL_DateDe
                                list_of_cmd_lines[counter, 58] = "0.000000"; //Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");                    //DL_QteDE
                                list_of_cmd_lines[counter, 59] = "0";                  //DL_NoSousTotal
                                list_of_cmd_lines[counter, 60] = "0";                //CA_No
                                list_of_cmd_lines[counter, 61] = "0.000000";            // DL_PUBC
                                list_of_cmd_lines[counter, 62] = DL_CodeTaxe1;                  // DL_CodeTaxe1
                                list_of_cmd_lines[counter, 63] = DL_Taxe1.ToString().Replace(",", ".");         // DL_Taxe1
                                list_of_cmd_lines[counter, 64] = "0.000000";            // DL_Taxe2
                                list_of_cmd_lines[counter, 65] = "0.000000";            // DL_Taxe3
                                list_of_cmd_lines[counter, 66] = "0";                   // DL_TypeTaux1
                                list_of_cmd_lines[counter, 67] = "0";                   // DL_TypeTaxe1
                                list_of_cmd_lines[counter, 68] = "0";                   // DL_TypeTaux2
                                list_of_cmd_lines[counter, 69] = "0";                   // DL_TypeTaxe2
                                list_of_cmd_lines[counter, 70] = "0";                   // DL_TypeTaux3
                                list_of_cmd_lines[counter, 71] = "0";                   // DL_TypeTaxe3

                                list_of_cmd_lines[counter, 72] = "1";                                           // DL_MvtStock
                                list_of_cmd_lines[counter, 73] = "";                                            // AF_RefFourniss
                                list_of_cmd_lines[counter, 74] = ((COLIS_article == null || COLIS_article == "") ? "0.0" : COLIS_article).ToString().Replace(",", ".");    // COLIS
                                list_of_cmd_lines[counter, 75] = ((PCB_article == null || PCB_article == "") ? "0.0" : PCB_article).ToString().Replace(",", ".");      // PCB
                                list_of_cmd_lines[counter, 76] = COMPLEMENT_article;                            // COMPLEMENT
                                list_of_cmd_lines[counter, 77] = "";                                            // PourVeolog
                                list_of_cmd_lines[counter, 78] = "";                                            // DL_PieceOFProd
                                list_of_cmd_lines[counter, 79] = "";                                            // DL_Operation
                                list_of_cmd_lines[counter, 80] = veologDeliveryDateTime;    //DO_DateLivr
                                list_of_cmd_lines[counter, 81] = "0";    //DL_NonLivre

                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Exception : 2D table not working properly.\r\n" + ex.Message);
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ******************** Exception Line ********************");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Le tableau 'BLF' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", "Le tableau 'BLF' à 2 dimensions ne fonctionne pas correctement, message: " + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                        }

                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Compter => " + counter);
                        counter++;

                    }
                    // ===== End Foreach =====

                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Vérifier si un produit pour 1 = BLF");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertBcfDocument_Veolog(true, "13", reference_BLF_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_supplier_info));

                    //generate document BLF_____. in database.
                    try
                    {
                        OdbcCommand command = new OdbcCommand(QueryHelper.insertBcfDocument_Veolog(true, "13", reference_BLF_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_supplier_info), connexion); //calling the query and parsing the parameters into it
                        command.ExecuteReader(); // executing the query
                    }
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Message :" + ex.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace :" + ex.StackTrace);
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Import annulée");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }


                    string[,] products_BCF = new string[position_item / 1000, 82]; // create array with enough space

                    //insert documentline into the database with articles having 20 as value @index 2
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + ": insert documentline into the database with articles having 13 as value @index 2");

                    for (int x = 0; x < list_of_cmd_lines.GetLength(0); x++)
                    {
                        if (list_of_cmd_lines[x, 1] == "13")
                        {
                            for (int y = 0; y < list_of_cmd_lines.GetLength(1); y++)
                            {
                                products_BCF[x, y] = list_of_cmd_lines[x, y];
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : products_BLF_L[" + x + "," + y + "] = " + products_BCF[x, y]);
                            }

                            //insert the article to documentline in the database
                            try
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : insert the article " + products_BCF[x, 12] + " (Ref:" + products_BCF[x, 9] + ") to documentline in the database");

                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : requette sql ===> " + QueryHelper.insertBcfDocumentLine_Veolog(true, products_BCF, x));

                                OdbcCommand command = new OdbcCommand(QueryHelper.insertBcfDocumentLine_Veolog(true, products_BCF, x), connexion);
                                command.ExecuteReader();
                            }
                            catch (OdbcException ex)
                            {
                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ********************** OdbcException *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }

                            //Update docline artile stock
                            try
                            {
                                bool found_stock = false;
                                double AS_StockReel = 0.0;
                                double AS_StockReserve = 0.0;
                                double AS_StockMontant = 0.0;
                                double productPrixUnite = Convert.ToDouble(products_BCF[x, 16].Replace('.', ','));
                                double AS_StockCommande = 0.0;

                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : get current stock in F_ARTSTOCK table in the database");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : requette sql ===> " + QueryHelper.getArticleStock(true, products_BCF[x, 9]));
                                using (OdbcCommand command_ = new OdbcCommand(QueryHelper.getArticleStock(true, products_BCF[x, 9]), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                                {
                                    using (IDataReader reader = command_.ExecuteReader()) //read rows of the executed query
                                    {
                                        if (reader.Read()) // If any rows returned
                                        {
                                            found_stock = true;
                                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Stock trouvé : AS_StockReel (" + reader[0].ToString() + "), AS_StockReserve (" + reader[1].ToString() + "), AS_StockMontant (" + reader[2].ToString() + "), AS_StockCommande (" + reader[3].ToString() + ").");
                                            AS_StockReel = Convert.ToDouble(reader[0].ToString().Replace(".", ","));
                                            AS_StockReserve = Convert.ToDouble(reader[1].ToString().Replace(".", ","));
                                            AS_StockMontant = Convert.ToDouble(reader[2].ToString().Replace(".", ","));
                                            AS_StockCommande = Convert.ToDouble(reader[3].ToString().Replace(".", ","));
                                        }
                                        else// If no rows returned
                                        {
                                            //do nothing.
                                            found_stock = false;
                                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucune reponse.");
                                            logFileWriter.Flush();
                                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", "Echec à la récupération du stock de l'article " + products_BCF[x, 9], "" + METHODE_NAME + " : Aucune reponse.", fileName, logFileName_import));
                                            return null;
                                        }
                                    }
                                }


                                if (found_stock)
                                {
                                    //Calculate stock info
                                    double AS_CMUP = AS_StockMontant / AS_StockReel;
                                    double new_AS_StockReel = AS_StockReel + Convert.ToDouble(products_BCF[x, 13].Replace('.', ','));
                                    //double new_AS_StockReserve = AS_StockReserve - Convert.ToDouble(products_BCF[x, 13].Replace('.', ','));
                                    double new_AS_StockCommande = AS_StockCommande - Convert.ToDouble(products_BCF[x, 13].Replace('.', ','));
                                    double new_AS_StockMontant = new_AS_StockReel * productPrixUnite;
                                    double new_AS_CMUP = new_AS_StockMontant / new_AS_StockReel;

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Calculation...");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : new_AS_StockReel: " + new_AS_StockReel + " = AS_StockReel: " + AS_StockReel + " + products_DESADV[x, 13]: " + Convert.ToDouble(products_BCF[x, 13].Replace('.', ',')));
                                    //logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : new_AS_StockReserve: " + new_AS_StockReserve + " = AS_StockReserve: " + AS_StockReserve + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_BCF[x, 13].Replace('.', ',')));
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + ": new_AS_StockCommande: " + new_AS_StockCommande + " = AS_StockCommande: " + AS_StockCommande + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_BCF[x, 13].Replace('.', ',')));
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : new_AS_StockMontant: " + new_AS_StockMontant + " = new_AS_StockReel: " + new_AS_StockReel + " X productPrixUnite: " + productPrixUnite);
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : new_AS_CMUP: " + new_AS_CMUP + " = new_AS_StockMontant: " + new_AS_StockMontant + " / new_AS_StockReel: " + new_AS_StockReel);

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : update article " + products_BCF[x, 12] + " (Ref:" + products_BCF[x, 9] + ") stock in F_ARTSTOCK table in the database");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : requette sql ===> " + QueryHelper.updateArticleStockBLF(true, products_BCF[x, 9], new_AS_StockReel, new_AS_StockCommande, new_AS_StockMontant));

                                    OdbcCommand command = new OdbcCommand(QueryHelper.updateArticleStockBLF(true, products_BCF[x, 9], new_AS_StockReel, new_AS_StockCommande, new_AS_StockMontant), connexion);
                                    command.ExecuteReader();
                                }


                                // check if product needs a reliquat
                                // if so then, it is not valid OR if not, then it is valid
                                var xxx = produitReliquat.Exists(item => item.AR_Ref != products_BCF[x, 9]);
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : produitReliquat.Exists(item => item.AR_Ref != "+products_BCF[x, 9]+") ");
                                if (!xxx)
                                {
                                    validProducts.Add(products_BCF[x, 9]);
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : added: " + products_BCF[x, 9] + " | validProducts size : " + validProducts.Count);
                                }

                            }
                            catch (OdbcException ex)
                            {
                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ********************** OdbcException Update F_ARTSTOCK table *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    //set Veolog date time import
                    try
                    {
                        string delivery_date_veolog = string.Format("{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Ajouter la date de livraision \"" + delivery_date_veolog + "\" de Veolog au BLF \"" + reference_BLF_doc + "\".");

                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.updateVeologDeliveryDate(true, reference_BLF_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre));
                        OdbcCommand command = new OdbcCommand(QueryHelper.updateVeologDeliveryDate(true, reference_BLF_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre), connexion);
                        {
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Date de livraison veolog à jour !");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur Veolog Date BCF ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }


                    //Delete the BC of the BL if BL is good or last BL reliquat
                    if (total_lines != dl.Count)
                    {
                        isLastBlReliquat = false;
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Numbre total de produit dans la commande \""+total_lines+"\" est différent de celui dans le BLF \""+dl.Count+"\"");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Supprimer \"" + validProducts.Count + "\" produits dans \""+ dh.Ref_Commande_Donneur_Ordre + "\" ,qu'ils sont dans le nouveau "+ reference_BLF_doc);

                        for (int x = 0; x < validProducts.Count; x++)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Suppréssion de l'article "+ validProducts[x] + " du Bon de Commande Fournisseur (BCF) \"" + dh.Ref_Commande_Donneur_Ordre + "\".");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.deleteValidCommandeLigne_achat(true, dh.Ref_Commande_Donneur_Ordre, validProducts[x]));
                            OdbcCommand command_ = new OdbcCommand(QueryHelper.deleteValidCommandeLigne_achat(true, dh.Ref_Commande_Donneur_Ordre, validProducts[x]), connexion);
                            IDataReader reader_ = command_.ExecuteReader();
                        }

                    }
                    else
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Numbre total de produit dans la commande \"" + total_lines + "\" est équal à celui dans le BLF \"" + dl.Count + "\"");
                    }

                    if (isLastBlReliquat)
                    {
                        try
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Pas de BLF Reliquat en attente");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Supprimer le Bon de Commande Fournisseur (BCF) \"" + dh.Ref_Commande_Donneur_Ordre + "\".");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre));
                            OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre), connexion);
                            IDataReader reader = command.ExecuteReader();
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Bon de Commande Fournisseur supprimé!");
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            logFileWriter.Flush();

                            logFileWriter.WriteLine("");
                            if (!deleteLineAndHeaderOfDocument(true, reference_BLF_doc, connexion, logFileWriter))
                            {
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "", "Erreur lors de la suppression du document " + reference_BLF_doc, "", fileName, logFileName_import));
                            }
                            logFileWriter.WriteLine("");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }
                    }
                    else
                    {
                        // Update BCF : reliquat && lines (qte, prix ht ,ttc,tva)
                        try
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : En attente de BLF Reliquat pour la prochaine fois...");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Mise à jour du Bon de Commande Fournisseur (BCF) en ENTETE \"" + dh.Ref_Commande_Donneur_Ordre + "\".");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.updateCommandeReliquat(true, dh.Ref_Commande_Donneur_Ordre));
                            OdbcCommand command = new OdbcCommand(QueryHelper.updateCommandeReliquat(true, dh.Ref_Commande_Donneur_Ordre), connexion);
                            IDataReader reader = command.ExecuteReader();
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Bon de Commande Fournisseur ENTETE Mise à jour");

                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine("");
                            for (int x = 0; x < produitReliquat.Count; x++)
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Mise à jour du Bon de Commande Fournisseur (BCF) en LIGNE \"" + dh.Ref_Commande_Donneur_Ordre + "\".");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.updateCommandeLigneReliquat_achat(true, dh.Ref_Commande_Donneur_Ordre, produitReliquat[x]));
                                OdbcCommand command_ = new OdbcCommand(QueryHelper.updateCommandeLigneReliquat_achat(true, dh.Ref_Commande_Donneur_Ordre, produitReliquat[x]), connexion);
                                IDataReader reader_ = command_.ExecuteReader();
                            }

                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Bon de Commande Fournisseur (Reliquat) Mise à jour");
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            logFileWriter.Flush();

                            logFileWriter.WriteLine("");
                            if (!deleteLineAndHeaderOfDocument(true, reference_BLF_doc, connexion, logFileWriter))
                            {
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "", "Erreur lors de la suppression du document " + reference_BLF_doc, "", fileName, logFileName_import));
                            }
                            logFileWriter.WriteLine("");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }
                    }


                    //update document numbering
                    try
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Mettre à jour la numérotation du document \"" + reference_BLF_doc + "\".");

                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.updateDOC_NumerotationTable(true, ((reference_BLF_doc == "BLF") ? "BLF" : "LF"), reference_BLF_doc));
                        OdbcCommand command = new OdbcCommand(QueryHelper.updateDOC_NumerotationTable(true, ((reference_BLF_doc == "BLF") ? "BLF" : "LF"), reference_BLF_doc), connexion);
                        IDataReader reader = command.ExecuteReader();
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Nouvelle numérotation à jour!");
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                }
                catch (Exception ex)
                {
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ********************** Exception 1 *********************");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Message :: " + ex.Message);
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace :: " + ex.StackTrace);
                    connexion.Close(); //disconnect from database
                    logFileWriter.Flush();
                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                    return null;
                }
            }

            return list_of_cmd_lines;
        }


        public static bool deleteLineAndHeaderOfDocument(bool SqlConnexion, string reference, OdbcConnection connexion, StreamWriter writer)
        {
            try
            {
                writer.WriteLine(DateTime.Now + " | deleteLineAndHeaderOfDocument() : SQL ===> " + QueryHelper.deleteCommande(SqlConnexion, reference));
                OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(SqlConnexion, reference), connexion);
                command.ExecuteReader();
                return true;
            }
            catch (Exception ex)
            {
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " | deleteLineAndHeaderOfDocument() : ******************** OdbcException Delete Document ********************");
                writer.WriteLine(DateTime.Now + " | deleteLineAndHeaderOfDocument() : Message :" + ex.Message);
                writer.WriteLine(DateTime.Now + " | deleteLineAndHeaderOfDocument() : StackTrace :" + ex.StackTrace);
                writer.WriteLine("");
                return false;
            }
        }

        public static bool deleteLineOfDocument(bool SqlConnexion, string reference, OdbcConnection connexion, StreamWriter writer)
        {
            try
            {
                writer.WriteLine(DateTime.Now + " | deleteLineOfDocument() : SQL ===> " + QueryHelper.deleteLigneDocument(SqlConnexion, reference));
                OdbcCommand command = new OdbcCommand(QueryHelper.deleteLigneDocument(SqlConnexion, reference), connexion);
                command.ExecuteReader();
                return true;
            }
            catch (Exception ex)
            {
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " | deleteLineOfDocument() : ******************** OdbcException Delete Document ********************");
                writer.WriteLine(DateTime.Now + " | deleteLineOfDocument() : Message :" + ex.Message);
                writer.WriteLine(DateTime.Now + " | deleteLineOfDocument() : StackTrace :" + ex.StackTrace);
                writer.WriteLine("");
                return false;
            }
        }


    }
}
