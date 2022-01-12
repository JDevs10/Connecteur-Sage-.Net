using Connexion;
using Import.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import
{
    public class DocumentVent
    {
        private static Database.Database db;
        private string logFileName_import;
        private List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new;

        public DocumentVent() { }
        public DocumentVent(string logFileName_import_, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new_)
        {
            this.recapLinesList_new = recapLinesList_new_;
            this.logFileName_import = logFileName_import_;
            db = new Database.Database();
        }


        public List<Alert_Mail.Classes.Custom.CustomMailRecapLines> returnAlertLogs()
        {
            return recapLinesList_new;
        }


        public static string[,] importBC(string reference_DESADV_doc, Veolog_DESADV dh, List<Veolog_DESADV_Lines> dl, string fileName, StreamWriter logFileWriter, string logFileName_import, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
        {
            string[,] list_of_cmd_lines = new string[dl.Count, 82];    // new string [x,y]
            string[] list_of_client_info = null;

            int position_item = 0;
            DateTime d = DateTime.Now;
            //string curr_date_time = d.ToString("yyyy-MM-dd hh:mm:ss");
            string curr_date = d.ToString("yyyy-MM-dd");
            //string curr_time = "000" + d.ToString("hhmmss");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;


            // AR_Design, AR_PoidsNet, AR_PoidsBrut, AR_PrixAch

            using (OdbcConnection connection = ConnexionManager.CreateOdbcConnexionSQL()) //connecting to database as handler
            {
                try
                {
                    connection.Open(); //opening the connection
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Connexion ouverte.");

                    //Get ref client CMD, nature_OP_P && total ht
                    string nature_op_p_ = "";
                    string do_totalHT_ = "";
                    string do_totalHTNet_ = "";
                    string do_totalTTC_ = "";
                    string do_NetAPayer_ = "";
                    string do_MontantRegle_ = "";
                    string CO_No = "";

                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Récupérer des informations de la commande la référence " + dh.Ref_Commande_Donneur_Ordre);
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getRefCMDClient(true, dh.Ref_Commande_Donneur_Ordre));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getRefCMDClient(true, dh.Ref_Commande_Donneur_Ordre), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                dh.Ref_Commande_Client_Livre = reader[0].ToString();
                                nature_op_p_ = reader[1].ToString();
                                do_totalHT_ = reader[2].ToString().Replace(",", ".");
                                do_totalHTNet_ = reader[3].ToString().Replace(",", ".");
                                do_totalTTC_ = reader[4].ToString().Replace(",", ".");
                                do_NetAPayer_ = reader[5].ToString().Replace(",", ".");
                                do_MontantRegle_ = reader[6].ToString().Replace(",", ".");
                                CO_No = reader[7].ToString();
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune commande trouvé!. ");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. La commande " + dh.Ref_Commande_Donneur_Ordre + " n'exist pas dans la base Sage", "La commande " + dh.Ref_Commande_Donneur_Ordre + " n'exist pas dans la BDD", "", fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    //Get the list of all Taxes (TVA)
                    //So i can calculate the ttc later
                    List<TVA> tvaList = null;
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Récupére tous les tva");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getAllTVA(true));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getAllTVA(true), connection))
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
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. ");
                            }
                        }
                    }

                    //get veolog delivery date and time
                    string veologDeliveryDateTime = "";
                    try
                    {
                        string year = dh.Date_De_Expedition.Substring(0, 4);
                        string month = dh.Date_De_Expedition.Substring(4, 2);
                        string day = dh.Date_De_Expedition.Substring(6, 2);

                        string hour = "00";
                        string mins = "00";
                        if (dh.Heure_De_Expedition != "" && dh.Heure_De_Expedition.Length == 4)
                        {
                            hour = dh.Heure_De_Expedition.Substring(0, 2);
                            mins = dh.Heure_De_Expedition.Substring(2, 2);
                        }
                        string veologDeliveryDate = year + "-" + month + "-" + day;
                        string veologDeliveryTime = hour + ":" + mins + ":00";
                        veologDeliveryDateTime = veologDeliveryDate + " " + veologDeliveryTime;
                    }
                    catch (Exception e)
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Erreur Date/Heure de livraison ********************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message: " + e.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace: " + e.StackTrace);
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", e.Message, e.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    int counter = 0;

                    foreach (Veolog_DESADV_Lines line in dl) //read item by item
                    {
                        string ref_client = "";
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
                        string DL_PieceBC = "";
                        string DL_DateBC = "";
                        string DL_QteBC = "";

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Lire la ligne de l'article.");

                        Connexion.ConnexionSaveLoad connexionSaveLoad = new Connexion.ConnexionSaveLoad();
                        connexionSaveLoad.Load();

                        if ((connexionSaveLoad.configurationConnexion.SQL.PREFIX.Equals("CFCI") ||
                            connexionSaveLoad.configurationConnexion.SQL.PREFIX.Equals("TABLEWEAR")) &&
                            line.Quantite_Colis.Equals("0"))
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : L'article " + line.Code_Article + " est retiré de la commande " + dh.Ref_Commande_Donneur_Ordre + ", alors on intègre pas. ");
                            continue;
                        }

                        //get Product Name By Reference
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getProductNameByReference_DESADV(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getProductNameByReference_DESADV(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    ref_article = (reader[0].ToString());                   // get product ref
                                    name_article = (reader[1].ToString());                  // sum up the total_negative variable. - check query
                                    DL_PoidsNet = (reader[2].ToString());                   // get unit weight NET - check query
                                    DL_PoidsBrut = (reader[3].ToString());                  // get unit weight BRUT - check query  
                                    //DL_PrixUnitaire_buyPrice = (reader[4].ToString());    // get (Prix d'achat) unit price - check query 
                                    DL_PrixUnitaire_salePriceHT = (reader[4].ToString());   // get (Prix de vente) unit price ht - check query
                                    COLIS_article = reader[5].ToString();
                                    PCB_article = reader[6].ToString();
                                    COMPLEMENT_article = reader[7].ToString();
                                    DL_Taxe1 = reader[8].ToString();
                                    DL_CodeTaxe1 = reader[9].ToString();
                                    DL_PieceBC = reader[10].ToString();
                                    DL_DateBC = reader[11].ToString();
                                    DL_QteBC = reader[12].ToString().Replace(",", ".");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Info du produit " + line.Code_Article + " trouvé");

                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : L'article \"" + line.Code_Article + "\" n'est pas trouvé dans la commande " + dh.Ref_Commande_Donneur_Ordre + ".");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. L'article \"" + line.Code_Article + "\" n'est pas trouvé dans le champ CodeBare ou dans la base Sage", "L'article \"" + line.Code_Article + "\" n'est pas trouvé dans la BDD.", "", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        //get Client Reference From CMD Ref
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Obtenir la référence client depuis le BC.");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientReferenceFromCMD_DESADV(true, dh.Ref_Commande_Donneur_Ordre));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceFromCMD_DESADV(true, dh.Ref_Commande_Donneur_Ordre), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    ref_client = reader[0].ToString();
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Une reponse. Ref Client ===> " + ref_client);
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. Le client n'est pas trouvé dans la commande " + dh.Ref_Commande_Donneur_Ordre, "Le client n'est pas trouvé dans la commande " + dh.Ref_Commande_Donneur_Ordre, "", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        //get Client Reference by Ref
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Obtenir les infos client par la référence.");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientReferenceById_DESADV(true, ref_client));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceById_DESADV(true, ref_client), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_client_info = new string[15];
                                    list_of_client_info[0] = reader[0].ToString();      // CT_Num
                                    list_of_client_info[1] = reader[1].ToString();      // CA_Num 
                                    list_of_client_info[2] = reader[2].ToString();      // CG_NumPrinc
                                    list_of_client_info[3] = reader[3].ToString();      // CT_NumPayeur
                                    list_of_client_info[4] = reader[4].ToString();      // N_Condition
                                    list_of_client_info[5] = reader[5].ToString();      // N_Devise
                                    list_of_client_info[6] = reader[6].ToString();      // CT_Langue
                                    list_of_client_info[7] = reader[7].ToString();      // DO_NbFacture = CT_Facture
                                    list_of_client_info[8] = reader[8].ToString().Replace(',', '.');      // DO_TxEscompte = CT_Taux02
                                    list_of_client_info[9] = reader[9].ToString();      // N_CatCompta
                                    list_of_client_info[10] = reader[10].ToString();    // CO_No
                                    list_of_client_info[11] = reader[11].ToString();    //  DO_Tarif = N_CatTarif
                                    list_of_client_info[12] = reader[12].ToString();    //  DO_Expedit = N_Expedition du tier
                                    list_of_client_info[13] = reader[13].ToString();    //  CT_Intitule
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Client trouvé.");
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. list_of_client_info est null");
                                }
                            }
                        }

                        //get client delivery adress
                        if (list_of_client_info != null)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientDeliveryAddress_DESADV(true, ref_client));
                            using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientDeliveryAddress_DESADV(true, ref_client), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                            {
                                using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                {
                                    if (reader.Read()) // If any rows returned
                                    {
                                        list_of_client_info[14] = reader[0].ToString();    // LI_No
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Adresse de livraison (" + reader[0].ToString() + ") trouvé!");
                                    }
                                    else// If no rows returned
                                    {
                                        //do nothing.
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. list_of_client_info est null");
                                        logFileWriter.Flush();
                                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. L'adresse de livraison du client " + ref_client + " n'est pas trouvé dans Sage", "L'adresse de livraison du client " + ref_client + " n'est pas trouvé dans Sage", "Aucune reponse. list_of_client_info est null", fileName, logFileName_import));
                                        return null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Erreur ********************");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucun client trouver.");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. Le client " + ref_client + " n'existe pas dans Sage", "Le client " + ref_client + " n'existe pas dans la BDD", "Aucune reponse. list_of_client_info est null", fileName, logFileName_import));
                            return null;
                        }


                        if (ref_article != "" && name_article != "" && list_of_client_info != null)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Article trouvé.");
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
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : List des TVA trouvé");
                                        TVA tva = null;
                                        bool tva_error = false;
                                        foreach (TVA tva_ in tvaList)
                                        {
                                            if (DL_CodeTaxe1 != null && DL_CodeTaxe1 != "" && tva_.TA_Code == DL_CodeTaxe1)
                                            {
                                                tva = tva_;
                                                tva_error = true;
                                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA " + tva.TA_Code + " trouvé \"" + tva.TA_Taux + "\"");
                                                break;
                                            }
                                            else
                                            {
                                                if (DL_CodeTaxe1 == null)
                                                {
                                                    //tva = tva_;
                                                    tva_error = false;
                                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA NULL trouvé, alors TVA mis à 0");
                                                    break;
                                                }
                                                else if (DL_CodeTaxe1 == "")
                                                {
                                                    //tva = tva_;
                                                    tva_error = false;
                                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA VIDE trouvé, alors TVA mis à 0");
                                                    break;
                                                }
                                            }
                                        }

                                        string endTVA = null;
                                        if (tva_error)
                                        {
                                            endTVA = tva.TA_Taux;
                                        }
                                        else
                                        {
                                            DL_CodeTaxe1 = "C00";
                                            endTVA = "0,000000";
                                        }
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : endTVA = " + endTVA);

                                        double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                        double product_20_P = (product_ht * Convert.ToDouble(endTVA)) / 100;
                                        product_ttc = product_ht + product_20_P;
                                        DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Prix TTC créé");
                                    }
                                    else
                                    {
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Warning TVA ********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Liste des tva non trouvée, tous les tva et prix ttc de chaque produit dans ce BL seront 0");

                                        double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                        double product_20_P = (product_ht * 0.0) / 100;
                                        product_ttc = product_ht + product_20_P;
                                        DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Prix TTC créé");
                                    }
                                    logFileWriter.Flush();
                                }
                                catch (Exception ex)
                                {
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Exception TVA ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Erreur lors du calcule du prix d'article TTC, message :\n" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Erreur lors du calcule du prix d'article TTC, message : " + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }

                                // input DL_DateBC: 03 - 01 - 2020 00:00:00;
                                string original_date = DL_DateBC;
                                string date = original_date.Split(' ')[0];
                                string time = original_date.Split(' ')[1];
                                DL_DateBC = date.Split('/')[2] + "-" + date.Split('/')[1] + "-" + date.Split('/')[0] + " " + time;
                                // ouput DL_DateBC: 2020-01-03 00:00:00;

                                // DESADV prefix will be used to create document
                                list_of_cmd_lines[counter, 0] = "0"; // DO_Domaine
                                list_of_cmd_lines[counter, 1] = "3"; //DO_Type
                                list_of_cmd_lines[counter, 2] = "3"; //DO_DocType
                                list_of_cmd_lines[counter, 3] = list_of_client_info[0]; //CT_NUM
                                list_of_cmd_lines[counter, 4] = reference_DESADV_doc; //DO_Piece
                                list_of_cmd_lines[counter, 5] = curr_date; //DO_Date
                                list_of_cmd_lines[counter, 6] = DL_DateBC; //DL_DateBC
                                list_of_cmd_lines[counter, 7] = (position_item).ToString(); // DL_Ligne line number 1000,2000
                                list_of_cmd_lines[counter, 8] = dh.Ref_Commande_Client_Livre; // DO_Ref
                                list_of_cmd_lines[counter, 9] = ref_article; // AR_Ref
                                list_of_cmd_lines[counter, 10] = "1"; //DL_Valorise
                                list_of_cmd_lines[counter, 11] = "1"; //DE_NO
                                list_of_cmd_lines[counter, 12] = name_article.Replace("'", "''"); // DL_Design
                                list_of_cmd_lines[counter, 13] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");  //line.Quantite_Colis; // DL_Qte
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
                                list_of_cmd_lines[counter, 20] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", "."); // EU_Qte; // EU_Qte
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }

                                list_of_cmd_lines[counter, 21] = (Convert.ToDouble(line.Quantite_Colis) * Convert.ToDouble(DL_PrixUnitaire_salePriceHT)).ToString().Replace(",", "."); //DL_MontantHT
                                list_of_cmd_lines[counter, 22] = (Convert.ToDouble(line.Quantite_Colis) * product_ttc).ToString().Replace(",", "."); //DL_MontantTTC
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }
                                if (list_of_cmd_lines[counter, 21].Equals("0")) { list_of_cmd_lines[counter, 21] = "0.000000"; } else if (!list_of_cmd_lines[counter, 21].Contains(".")) { list_of_cmd_lines[counter, 21] = list_of_cmd_lines[counter, 21] + ".0"; }
                                if (list_of_cmd_lines[counter, 22].Equals("0")) { list_of_cmd_lines[counter, 22] = "0.000000"; } else if (!list_of_cmd_lines[counter, 22].Contains(".")) { list_of_cmd_lines[counter, 22] = list_of_cmd_lines[counter, 22] + ".000000"; }

                                list_of_cmd_lines[counter, 23] = ""; //PF_Num
                                list_of_cmd_lines[counter, 24] = "0"; //DL_No
                                list_of_cmd_lines[counter, 25] = "0"; //DL_FactPoids
                                list_of_cmd_lines[counter, 26] = "0"; //DL_Escompte
                                list_of_cmd_lines[counter, 27] = DL_PUTTC; //DL_PUTTC
                                list_of_cmd_lines[counter, 28] = "0";   //DL_TTC

                                list_of_cmd_lines[counter, 29] = DL_PieceBC;   //DL_PieceBC
                                list_of_cmd_lines[counter, 30] = reference_DESADV_doc;   //DL_PieceBL
                                list_of_cmd_lines[counter, 31] = curr_date;   // DL_DateBL
                                list_of_cmd_lines[counter, 32] = "0";   //DL_TNomencl
                                list_of_cmd_lines[counter, 33] = "0";   //DL_TRemPied
                                list_of_cmd_lines[counter, 34] = "0";   //DL_TRemExep
                                list_of_cmd_lines[counter, 35] = DL_QteBC.Replace(",", ".");   //DL_QteBC
                                list_of_cmd_lines[counter, 36] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");   //DL_QteBL
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
                                list_of_cmd_lines[counter, 54] = CO_No;                    //CO_No
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

                                list_of_cmd_lines[counter, 72] = "3";                                           // DL_MvtStock
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
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Exception ********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Le tableau 'BL' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Le tableau 'BL' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                            counter++;
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Compter => " + counter);
                        }
                        else
                        {
                            counter--;
                            logFileWriter.WriteLine("Aucun article trouvé ou Aucun information client trouvé !");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Compter => " + counter);
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Aucun article trouvé ou Aucun information client trouvé !", "", fileName, logFileName_import));
                            return null;
                        }


                    }
                    // ===== End Foreach =====

                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Vérifier si un produit pour 0 = BL");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertDesadvDocument_Veolog(true, "3", reference_DESADV_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_client_info, dh.Etat, CO_No));

                    //generate document BL_____. in database.
                    try
                    {
                        OdbcCommand command = new OdbcCommand(QueryHelper.insertDesadvDocument_Veolog(true, "3", reference_DESADV_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_client_info, dh.Etat, CO_No), connection); //calling the query and parsing the parameters into it
                        command.ExecuteReader(); // executing the query
                    }
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }


                    string[,] products_DESADV = new string[position_item / 1000, 82]; // create array with enough space

                    //insert documentline into the database with articles having 20 as value @index 2
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : insert documentline into the database with articles having 3 as value @index 2");

                    for (int x = 0; x < list_of_cmd_lines.GetLength(0); x++)
                    {
                        if (list_of_cmd_lines[x, 1] == "3")
                        {
                            logFileWriter.WriteLine("");
                            for (int y = 0; y < list_of_cmd_lines.GetLength(1); y++)
                            {
                                products_DESADV[x, y] = list_of_cmd_lines[x, y];
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : products_BL_L[" + x + "," + y + "] = " + products_DESADV[x, y]);
                            }

                            //insert the article to documentline in the database
                            try
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : insert the article " + products_DESADV[x, 12] + " (Ref:" + products_DESADV[x, 9] + ") to documentline in the database");

                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : requette sql ===> " + QueryHelper.insertDesadvDocumentLine_Veolog(true, products_DESADV, x));

                                OdbcCommand command = new OdbcCommand(QueryHelper.insertDesadvDocumentLine_Veolog(true, products_DESADV, x), connection);
                                command.ExecuteReader();
                            }
                            catch (OdbcException ex)
                            {
                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter);
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }


                            //Update docline artile stock
                            try
                            {
                                bool found_stock = false;
                                double AS_StockReel = 0.0;
                                double AS_StockReserve = 0.0;
                                double AS_StockMontant = 0.0;
                                double productPrixUnite = Convert.ToDouble(products_DESADV[x, 16].Replace('.', ','));

                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : get current stock in F_ARTSTOCK table in the database");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : requette sql ===> " + QueryHelper.getArticleStock(true, products_DESADV[x, 9], dh.Entrepot));
                                using (OdbcCommand command_ = new OdbcCommand(QueryHelper.getArticleStock(true, products_DESADV[x, 9], dh.Entrepot), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                                {
                                    using (IDataReader reader = command_.ExecuteReader()) // read rows of the executed query
                                    {
                                        if (reader.Read()) // If any rows returned
                                        {
                                            found_stock = true;
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Stock trouvé : AS_StockReel (" + reader[0].ToString() + "), AS_StockReserve (" + reader[1].ToString() + "), AS_StockMontant (" + reader[2].ToString() + ").");
                                            AS_StockReel = Convert.ToDouble(reader[0].ToString().Replace(".", ","));
                                            AS_StockReserve = Convert.ToDouble(reader[1].ToString().Replace(".", ","));
                                            AS_StockMontant = Convert.ToDouble(reader[2].ToString().Replace(".", ","));
                                        }
                                        else// If no rows returned
                                        {
                                            //do nothing.
                                            found_stock = false;
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse.");
                                            logFileWriter.WriteLine("");
                                            deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter);
                                            logFileWriter.WriteLine("");
                                            logFileWriter.Flush();
                                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Echec à la récupértion du stock de l'article " + products_DESADV[x, 9], "", fileName, logFileName_import));
                                            return null;
                                        }
                                    }
                                }


                                if (found_stock)
                                {
                                    //Calculate stock info
                                    //double AS_CMUP = AS_StockMontant / AS_StockReel;
                                    double new_AS_StockReel = AS_StockReel - Convert.ToDouble(products_DESADV[x, 13].Replace('.', ','));
                                    double new_AS_StockReserve = AS_StockReserve - Convert.ToDouble(products_DESADV[x, 13].Replace('.', ','));
                                    double new_AS_StockMontant = new_AS_StockReel * productPrixUnite;
                                    double new_AS_CMUP = new_AS_StockMontant / new_AS_StockReel;

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_StockReel: " + new_AS_StockReel + " = AS_StockReel: " + AS_StockReel + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_DESADV[x, 13].Replace('.', ',')));
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_StockReserve: " + new_AS_StockReserve + " = AS_StockReserve: " + AS_StockReserve + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_DESADV[x, 13].Replace('.', ',')));
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_StockMontant: " + new_AS_StockMontant + " = new_AS_StockReel: " + new_AS_StockReel + " * productPrixUnite: " + productPrixUnite);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_CMUP: " + new_AS_CMUP + " = new_AS_StockMontant: " + new_AS_StockMontant + " / new_AS_StockReel: " + new_AS_StockReel);

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : update article " + products_DESADV[x, 12] + " (Ref:" + products_DESADV[x, 9] + ") stock in F_ARTSTOCK table in the database");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : requette sql ===> " + QueryHelper.updateArticleStock(true, products_DESADV[x, 9], new_AS_StockReel, new_AS_StockReserve, new_AS_StockMontant, dh.Entrepot));

                                    OdbcCommand command = new OdbcCommand(QueryHelper.updateArticleStock(true, products_DESADV[x, 9], new_AS_StockReel, new_AS_StockReserve, new_AS_StockMontant, dh.Entrepot), connection);
                                    command.ExecuteReader();
                                }
                            }
                            catch (OdbcException ex)
                            {
                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException Update F_ARTSTOCK table *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                logFileWriter.Flush();

                                logFileWriter.WriteLine("");
                                if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                                {
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                                }
                                logFileWriter.WriteLine("");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    //set Veolog date time import
                    try
                    {
                        string delivery_date_veolog = string.Format("{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Ajouter la date de livraision \"" + delivery_date_veolog + "\" de Veolog au DESADV \"" + reference_DESADV_doc + "\".");

                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.updateVeologDeliveryDate(true, reference_DESADV_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre));
                        OdbcCommand command = new OdbcCommand(QueryHelper.updateVeologDeliveryDate(true, reference_DESADV_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre), connection);
                        {
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Date de livraison veolog à jour !");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();

                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    //Delete the BC of the BL
                    try
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Supprimer le Bon de Commande (BC) \"" + dh.Ref_Commande_Donneur_Ordre + "\".");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre));
                        OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre), connection);
                        IDataReader reader = command.ExecuteReader();
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Bon de Commande supprimé!");
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();

                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    //update document numbering
                    try
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Mettre à jour la numérotation du document \"" + reference_DESADV_doc + "\".");

                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.updateDOC_NumerotationTable(true, "BL", reference_DESADV_doc));
                        OdbcCommand command = new OdbcCommand(QueryHelper.updateDOC_NumerotationTable(true, "BL", reference_DESADV_doc), connection);
                        IDataReader reader = command.ExecuteReader();
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Nouvelle numérotation à jour!");
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();

                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                }
                catch (Exception ex)
                {
                    // Exceptions pouvant survenir durant l'exécution de la requête SQL
                    // return list_of_products[0][0];//return false because the query failed to execute

                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** Exception 3 *********************");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :: " + ex.Message);
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :: " + ex.StackTrace);
                    connection.Close(); //disconnect from database
                    logFileWriter.Flush();
                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                    return null;
                }
            }

            return list_of_cmd_lines;
        }


        public static string[,] insertDesadv_Veolog_(string reference_DESADV_doc, Veolog_DESADV dh, List<Veolog_DESADV_Lines> dl, string fileName, StreamWriter logFileWriter, string logFileName_import, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
        {
            string[,] list_of_cmd_lines = new string[dl.Count, 82];    // new string [x,y]
            string[] list_of_client_info = null;

            int position_item = 0;
            DateTime d = DateTime.Now;
            //string curr_date_time = d.ToString("yyyy-MM-dd hh:mm:ss");
            string curr_date = d.ToString("yyyy-MM-dd");
            //string curr_time = "000" + d.ToString("hhmmss");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;


            // AR_Design, AR_PoidsNet, AR_PoidsBrut, AR_PrixAch

            using (OdbcConnection connection = ConnexionManager.CreateOdbcConnexionSQL()) //connecting to database as handler
            {
                try
                {
                    connection.Open(); //opening the connection
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Connexion ouverte.");

                    //Get ref client CMD, nature_OP_P && total ht
                    string nature_op_p_ = "";
                    string do_totalHT_ = "";
                    string do_totalHTNet_ = "";
                    string do_totalTTC_ = "";
                    string do_NetAPayer_ = "";
                    string do_MontantRegle_ = "";
                    string CO_No = "";

                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Récupérer des informations de la commande la référence " + dh.Ref_Commande_Donneur_Ordre);
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getRefCMDClient(true, dh.Ref_Commande_Donneur_Ordre));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getRefCMDClient(true, dh.Ref_Commande_Donneur_Ordre), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                dh.Ref_Commande_Client_Livre = reader[0].ToString();
                                nature_op_p_ = reader[1].ToString();
                                do_totalHT_ = reader[2].ToString().Replace(",", ".");
                                do_totalHTNet_ = reader[3].ToString().Replace(",", ".");
                                do_totalTTC_ = reader[4].ToString().Replace(",", ".");
                                do_NetAPayer_ = reader[5].ToString().Replace(",", ".");
                                do_MontantRegle_ = reader[6].ToString().Replace(",", ".");
                                CO_No = reader[7].ToString();
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune commande trouvé!. ");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. La commande " + dh.Ref_Commande_Donneur_Ordre + " n'exist pas dans la base Sage", "La commande " + dh.Ref_Commande_Donneur_Ordre + " n'exist pas dans la BDD", "", fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    //Get the list of all Taxes (TVA)
                    //So i can calculate the ttc later
                    List<TVA> tvaList = null;
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Récupére tous les tva");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getAllTVA(true));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getAllTVA(true), connection))
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
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. ");
                            }
                        }
                    }

                    //get veolog delivery date and time
                    string veologDeliveryDateTime = "";
                    try
                    {
                        string year = dh.Date_De_Expedition.Substring(0, 4);
                        string month = dh.Date_De_Expedition.Substring(4, 2);
                        string day = dh.Date_De_Expedition.Substring(6, 2);

                        string hour = "00";
                        string mins = "00";
                        if (dh.Heure_De_Expedition != "" && dh.Heure_De_Expedition.Length == 4)
                        {
                            hour = dh.Heure_De_Expedition.Substring(0, 2);
                            mins = dh.Heure_De_Expedition.Substring(2, 2);
                        }
                        string veologDeliveryDate = year + "-" + month + "-" + day;
                        string veologDeliveryTime = hour + ":" + mins + ":00";
                        veologDeliveryDateTime = veologDeliveryDate + " " + veologDeliveryTime;
                    }
                    catch (Exception e)
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Erreur Date/Heure de livraison ********************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message: " + e.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace: " + e.StackTrace);
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", e.Message, e.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    int counter = 0;

                    foreach (Veolog_DESADV_Lines line in dl) //read item by item
                    {
                        string ref_client = "";
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
                        string DL_PieceBC = "";
                        string DL_DateBC = "";
                        string DL_QteBC = "";

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Lire la ligne de l'article.");

                        Connexion.ConnexionSaveLoad connexionSaveLoad = new Connexion.ConnexionSaveLoad();
                        connexionSaveLoad.Load();

                        if ((connexionSaveLoad.configurationConnexion.SQL.PREFIX.Equals("CFCI") ||
                            connexionSaveLoad.configurationConnexion.SQL.PREFIX.Equals("TABLEWEAR")) &&
                            line.Quantite_Colis.Equals("0"))
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : L'article " + line.Code_Article + " est retiré de la commande " + dh.Ref_Commande_Donneur_Ordre + ", alors on intègre pas. ");
                            continue;
                        }

                        //get Product Name By Reference
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getProductNameByReference_DESADV(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getProductNameByReference_DESADV(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    ref_article = (reader[0].ToString());                   // get product ref
                                    name_article = (reader[1].ToString());                  // sum up the total_negative variable. - check query
                                    DL_PoidsNet = (reader[2].ToString());                   // get unit weight NET - check query
                                    DL_PoidsBrut = (reader[3].ToString());                  // get unit weight BRUT - check query  
                                    //DL_PrixUnitaire_buyPrice = (reader[4].ToString());    // get (Prix d'achat) unit price - check query 
                                    DL_PrixUnitaire_salePriceHT = (reader[4].ToString());   // get (Prix de vente) unit price ht - check query
                                    COLIS_article = reader[5].ToString();
                                    PCB_article = reader[6].ToString();
                                    COMPLEMENT_article = reader[7].ToString();
                                    DL_Taxe1 = reader[8].ToString();
                                    DL_CodeTaxe1 = reader[9].ToString();
                                    DL_PieceBC = reader[10].ToString();
                                    DL_DateBC = reader[11].ToString();
                                    DL_QteBC = reader[12].ToString().Replace(",", ".");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Info du produit " + line.Code_Article + " trouvé");

                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : L'article \"" + line.Code_Article + "\" n'est pas trouvé dans la commande " + dh.Ref_Commande_Donneur_Ordre + ".");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. L'article \"" + line.Code_Article + "\" n'est pas trouvé dans le champ CodeBare ou dans la base Sage", "L'article \"" + line.Code_Article + "\" n'est pas trouvé dans la BDD.", "", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        //get Client Reference From CMD Ref
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Obtenir la référence client depuis le BC.");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientReferenceFromCMD_DESADV(true, dh.Ref_Commande_Donneur_Ordre));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceFromCMD_DESADV(true, dh.Ref_Commande_Donneur_Ordre), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    ref_client = reader[0].ToString();
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Une reponse. Ref Client ===> " + ref_client);
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. Le client n'est pas trouvé dans la commande " + dh.Ref_Commande_Donneur_Ordre, "Le client n'est pas trouvé dans la commande " + dh.Ref_Commande_Donneur_Ordre, "", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        //get Client Reference by Ref
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Obtenir les infos client par la référence.");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientReferenceById_DESADV(true, ref_client));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceById_DESADV(true, ref_client), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_client_info = new string[15];
                                    list_of_client_info[0] = reader[0].ToString();      // CT_Num
                                    list_of_client_info[1] = reader[1].ToString();      // CA_Num 
                                    list_of_client_info[2] = reader[2].ToString();      // CG_NumPrinc
                                    list_of_client_info[3] = reader[3].ToString();      // CT_NumPayeur
                                    list_of_client_info[4] = reader[4].ToString();      // N_Condition
                                    list_of_client_info[5] = reader[5].ToString();      // N_Devise
                                    list_of_client_info[6] = reader[6].ToString();      // CT_Langue
                                    list_of_client_info[7] = reader[7].ToString();      // DO_NbFacture = CT_Facture
                                    list_of_client_info[8] = reader[8].ToString().Replace(',', '.');      // DO_TxEscompte = CT_Taux02
                                    list_of_client_info[9] = reader[9].ToString();      // N_CatCompta
                                    list_of_client_info[10] = reader[10].ToString();    // CO_No
                                    list_of_client_info[11] = reader[11].ToString();    //  DO_Tarif = N_CatTarif
                                    list_of_client_info[12] = reader[12].ToString();    //  DO_Expedit = N_Expedition du tier
                                    list_of_client_info[13] = reader[13].ToString();    //  CT_Intitule
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Client trouvé.");
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. list_of_client_info est null");
                                }
                            }
                        }

                        //get client delivery adress
                        if (list_of_client_info != null)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientDeliveryAddress_DESADV(true, ref_client));
                            using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientDeliveryAddress_DESADV(true, ref_client), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                            {
                                using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                {
                                    if (reader.Read()) // If any rows returned
                                    {
                                        list_of_client_info[14] = reader[0].ToString();    // LI_No
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Adresse de livraison (" + reader[0].ToString() + ") trouvé!");
                                    }
                                    else// If no rows returned
                                    {
                                        //do nothing.
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. list_of_client_info est null");
                                        logFileWriter.Flush();
                                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. L'adresse de livraison du client " + ref_client + " n'est pas trouvé dans Sage", "L'adresse de livraison du client " + ref_client + " n'est pas trouvé dans Sage", "Aucune reponse. list_of_client_info est null", fileName, logFileName_import));
                                        return null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Erreur ********************");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucun client trouver.");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. Le client " + ref_client + " n'existe pas dans Sage", "Le client " + ref_client + " n'existe pas dans la BDD", "Aucune reponse. list_of_client_info est null", fileName, logFileName_import));
                            return null;
                        }


                        if (ref_article != "" && name_article != "" && list_of_client_info != null)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Article trouvé.");
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
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : List des TVA trouvé");
                                        TVA tva = null;
                                        bool tva_error = false;
                                        foreach (TVA tva_ in tvaList)
                                        {
                                            if (DL_CodeTaxe1 != null && DL_CodeTaxe1 != "" && tva_.TA_Code == DL_CodeTaxe1)
                                            {
                                                tva = tva_;
                                                tva_error = true;
                                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA " + tva.TA_Code + " trouvé \"" + tva.TA_Taux + "\"");
                                                break;
                                            }
                                            else
                                            {
                                                if (DL_CodeTaxe1 == null)
                                                {
                                                    //tva = tva_;
                                                    tva_error = false;
                                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA NULL trouvé, alors TVA mis à 0");
                                                    break;
                                                }
                                                else if (DL_CodeTaxe1 == "")
                                                {
                                                    //tva = tva_;
                                                    tva_error = false;
                                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA VIDE trouvé, alors TVA mis à 0");
                                                    break;
                                                }
                                            }
                                        }

                                        string endTVA = null;
                                        if (tva_error)
                                        {
                                            endTVA = tva.TA_Taux;
                                        }
                                        else
                                        {
                                            DL_CodeTaxe1 = "C00";
                                            endTVA = "0,000000";
                                        }
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : endTVA = " + endTVA);

                                        double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                        double product_20_P = (product_ht * Convert.ToDouble(endTVA)) / 100;
                                        product_ttc = product_ht + product_20_P;
                                        DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Prix TTC créé");
                                    }
                                    else
                                    {
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Warning TVA ********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Liste des tva non trouvée, tous les tva et prix ttc de chaque produit dans ce BL seront 0");

                                        double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                        double product_20_P = (product_ht * 0.0) / 100;
                                        product_ttc = product_ht + product_20_P;
                                        DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Prix TTC créé");
                                    }
                                    logFileWriter.Flush();
                                }
                                catch (Exception ex)
                                {
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Exception TVA ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Erreur lors du calcule du prix d'article TTC, message :\n" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Erreur lors du calcule du prix d'article TTC, message : " + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }

                                // input DL_DateBC: 03 - 01 - 2020 00:00:00;
                                string original_date = DL_DateBC;
                                string date = original_date.Split(' ')[0];
                                string time = original_date.Split(' ')[1];
                                DL_DateBC = date.Split('/')[2] + "-" + date.Split('/')[1] + "-" + date.Split('/')[0] + " " + time;
                                // ouput DL_DateBC: 2020-01-03 00:00:00;

                                // DESADV prefix will be used to create document
                                list_of_cmd_lines[counter, 0] = "0"; // DO_Domaine
                                list_of_cmd_lines[counter, 1] = "3"; //DO_Type
                                list_of_cmd_lines[counter, 2] = "3"; //DO_DocType
                                list_of_cmd_lines[counter, 3] = list_of_client_info[0]; //CT_NUM
                                list_of_cmd_lines[counter, 4] = reference_DESADV_doc; //DO_Piece
                                list_of_cmd_lines[counter, 5] = curr_date; //DO_Date
                                list_of_cmd_lines[counter, 6] = DL_DateBC; //DL_DateBC
                                list_of_cmd_lines[counter, 7] = (position_item).ToString(); // DL_Ligne line number 1000,2000
                                list_of_cmd_lines[counter, 8] = dh.Ref_Commande_Client_Livre; // DO_Ref
                                list_of_cmd_lines[counter, 9] = ref_article; // AR_Ref
                                list_of_cmd_lines[counter, 10] = "1"; //DL_Valorise
                                list_of_cmd_lines[counter, 11] = "1"; //DE_NO
                                list_of_cmd_lines[counter, 12] = name_article.Replace("'", "''"); // DL_Design
                                list_of_cmd_lines[counter, 13] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");  //line.Quantite_Colis; // DL_Qte
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
                                list_of_cmd_lines[counter, 20] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", "."); // EU_Qte; // EU_Qte
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }

                                list_of_cmd_lines[counter, 21] = (Convert.ToDouble(line.Quantite_Colis) * Convert.ToDouble(DL_PrixUnitaire_salePriceHT)).ToString().Replace(",", "."); //DL_MontantHT
                                list_of_cmd_lines[counter, 22] = (Convert.ToDouble(line.Quantite_Colis) * product_ttc).ToString().Replace(",", "."); //DL_MontantTTC
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }
                                if (list_of_cmd_lines[counter, 21].Equals("0")) { list_of_cmd_lines[counter, 21] = "0.000000"; } else if (!list_of_cmd_lines[counter, 21].Contains(".")) { list_of_cmd_lines[counter, 21] = list_of_cmd_lines[counter, 21] + ".0"; }
                                if (list_of_cmd_lines[counter, 22].Equals("0")) { list_of_cmd_lines[counter, 22] = "0.000000"; } else if (!list_of_cmd_lines[counter, 22].Contains(".")) { list_of_cmd_lines[counter, 22] = list_of_cmd_lines[counter, 22] + ".000000"; }

                                list_of_cmd_lines[counter, 23] = ""; //PF_Num
                                list_of_cmd_lines[counter, 24] = "0"; //DL_No
                                list_of_cmd_lines[counter, 25] = "0"; //DL_FactPoids
                                list_of_cmd_lines[counter, 26] = "0"; //DL_Escompte
                                list_of_cmd_lines[counter, 27] = DL_PUTTC; //DL_PUTTC
                                list_of_cmd_lines[counter, 28] = "0";   //DL_TTC

                                list_of_cmd_lines[counter, 29] = DL_PieceBC;   //DL_PieceBC
                                list_of_cmd_lines[counter, 30] = reference_DESADV_doc;   //DL_PieceBL
                                list_of_cmd_lines[counter, 31] = curr_date;   // DL_DateBL
                                list_of_cmd_lines[counter, 32] = "0";   //DL_TNomencl
                                list_of_cmd_lines[counter, 33] = "0";   //DL_TRemPied
                                list_of_cmd_lines[counter, 34] = "0";   //DL_TRemExep
                                list_of_cmd_lines[counter, 35] = DL_QteBC.Replace(",", ".");   //DL_QteBC
                                list_of_cmd_lines[counter, 36] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");   //DL_QteBL
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
                                list_of_cmd_lines[counter, 54] = CO_No;                    //CO_No
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

                                list_of_cmd_lines[counter, 72] = "3";                                           // DL_MvtStock
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
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Exception ********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Le tableau 'BL' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Le tableau 'BL' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                            counter++;
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Compter => " + counter);
                        }
                        else
                        {
                            counter--;
                            logFileWriter.WriteLine("Aucun article trouvé ou Aucun information client trouvé !");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Compter => " + counter);
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Aucun article trouvé ou Aucun information client trouvé !", "", fileName, logFileName_import));
                            return null;
                        }


                    }
                    // ===== End Foreach =====

                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Vérifier si un produit pour 0 = BL");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertDesadvDocument_Veolog(true, "3", reference_DESADV_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_client_info, dh.Etat, CO_No));

                    //generate document BL_____. in database.
                    try
                    {
                        OdbcCommand command = new OdbcCommand(QueryHelper.insertDesadvDocument_Veolog(true, "3", reference_DESADV_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_client_info, dh.Etat, CO_No), connection); //calling the query and parsing the parameters into it
                        command.ExecuteReader(); // executing the query
                    }
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }


                    string[,] products_DESADV = new string[position_item / 1000, 82]; // create array with enough space

                    //insert documentline into the database with articles having 20 as value @index 2
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : insert documentline into the database with articles having 3 as value @index 2");

                    for (int x = 0; x < list_of_cmd_lines.GetLength(0); x++)
                    {
                        if (list_of_cmd_lines[x, 1] == "3")
                        {
                            logFileWriter.WriteLine("");
                            for (int y = 0; y < list_of_cmd_lines.GetLength(1); y++)
                            {
                                products_DESADV[x, y] = list_of_cmd_lines[x, y];
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : products_BL_L[" + x + "," + y + "] = " + products_DESADV[x, y]);
                            }

                            //insert the article to documentline in the database
                            try
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : insert the article " + products_DESADV[x, 12] + " (Ref:" + products_DESADV[x, 9] + ") to documentline in the database");

                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : requette sql ===> " + QueryHelper.insertDesadvDocumentLine_Veolog(true, products_DESADV, x));

                                OdbcCommand command = new OdbcCommand(QueryHelper.insertDesadvDocumentLine_Veolog(true, products_DESADV, x), connection);
                                command.ExecuteReader();
                            }
                            catch (OdbcException ex)
                            {
                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter);
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }


                            //Update docline artile stock
                            try
                            {
                                bool found_stock = false;
                                double AS_StockReel = 0.0;
                                double AS_StockReserve = 0.0;
                                double AS_StockMontant = 0.0;
                                double productPrixUnite = Convert.ToDouble(products_DESADV[x, 16].Replace('.', ','));

                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : get current stock in F_ARTSTOCK table in the database");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : requette sql ===> " + QueryHelper.getArticleStock(true, products_DESADV[x, 9], dh.Entrepot));
                                using (OdbcCommand command_ = new OdbcCommand(QueryHelper.getArticleStock(true, products_DESADV[x, 9], dh.Entrepot), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                                {
                                    using (IDataReader reader = command_.ExecuteReader()) // read rows of the executed query
                                    {
                                        if (reader.Read()) // If any rows returned
                                        {
                                            found_stock = true;
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Stock trouvé : AS_StockReel (" + reader[0].ToString() + "), AS_StockReserve (" + reader[1].ToString() + "), AS_StockMontant (" + reader[2].ToString() + ").");
                                            AS_StockReel = Convert.ToDouble(reader[0].ToString().Replace(".", ","));
                                            AS_StockReserve = Convert.ToDouble(reader[1].ToString().Replace(".", ","));
                                            AS_StockMontant = Convert.ToDouble(reader[2].ToString().Replace(".", ","));
                                        }
                                        else// If no rows returned
                                        {
                                            //do nothing.
                                            found_stock = false;
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse.");
                                            logFileWriter.WriteLine("");
                                            deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter);
                                            logFileWriter.WriteLine("");
                                            logFileWriter.Flush();
                                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Echec à la récupértion du stock de l'article " + products_DESADV[x, 9], "", fileName, logFileName_import));
                                            return null;
                                        }
                                    }
                                }


                                if (found_stock)
                                {
                                    //Calculate stock info
                                    //double AS_CMUP = AS_StockMontant / AS_StockReel;
                                    double new_AS_StockReel = AS_StockReel - Convert.ToDouble(products_DESADV[x, 13].Replace('.', ','));
                                    double new_AS_StockReserve = AS_StockReserve - Convert.ToDouble(products_DESADV[x, 13].Replace('.', ','));
                                    double new_AS_StockMontant = new_AS_StockReel * productPrixUnite;
                                    double new_AS_CMUP = new_AS_StockMontant / new_AS_StockReel;

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_StockReel: " + new_AS_StockReel + " = AS_StockReel: " + AS_StockReel + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_DESADV[x, 13].Replace('.', ',')));
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_StockReserve: " + new_AS_StockReserve + " = AS_StockReserve: " + AS_StockReserve + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_DESADV[x, 13].Replace('.', ',')));
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_StockMontant: " + new_AS_StockMontant + " = new_AS_StockReel: " + new_AS_StockReel + " * productPrixUnite: " + productPrixUnite);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_CMUP: " + new_AS_CMUP + " = new_AS_StockMontant: " + new_AS_StockMontant + " / new_AS_StockReel: " + new_AS_StockReel);

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : update article " + products_DESADV[x, 12] + " (Ref:" + products_DESADV[x, 9] + ") stock in F_ARTSTOCK table in the database");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : requette sql ===> " + QueryHelper.updateArticleStock(true, products_DESADV[x, 9], new_AS_StockReel, new_AS_StockReserve, new_AS_StockMontant, dh.Entrepot));

                                    OdbcCommand command = new OdbcCommand(QueryHelper.updateArticleStock(true, products_DESADV[x, 9], new_AS_StockReel, new_AS_StockReserve, new_AS_StockMontant, dh.Entrepot), connection);
                                    command.ExecuteReader();
                                }
                            }
                            catch (OdbcException ex)
                            {
                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException Update F_ARTSTOCK table *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                logFileWriter.Flush();

                                logFileWriter.WriteLine("");
                                if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                                {
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                                }
                                logFileWriter.WriteLine("");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    //set Veolog date time import
                    try
                    {
                        string delivery_date_veolog = string.Format("{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Ajouter la date de livraision \"" + delivery_date_veolog + "\" de Veolog au DESADV \"" + reference_DESADV_doc + "\".");

                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.updateVeologDeliveryDate(true, reference_DESADV_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre));
                        OdbcCommand command = new OdbcCommand(QueryHelper.updateVeologDeliveryDate(true, reference_DESADV_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre), connection);
                        {
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Date de livraison veolog à jour !");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();

                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    //Delete the BC of the BL
                    try
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Supprimer le Bon de Commande (BC) \"" + dh.Ref_Commande_Donneur_Ordre + "\".");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre));
                        OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre), connection);
                        IDataReader reader = command.ExecuteReader();
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Bon de Commande supprimé!");
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();

                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    //update document numbering
                    try
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Mettre à jour la numérotation du document \"" + reference_DESADV_doc + "\".");

                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.updateDOC_NumerotationTable(true, "BL", reference_DESADV_doc));
                        OdbcCommand command = new OdbcCommand(QueryHelper.updateDOC_NumerotationTable(true, "BL", reference_DESADV_doc), connection);
                        IDataReader reader = command.ExecuteReader();
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Nouvelle numérotation à jour!");
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();

                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                }
                catch (Exception ex)
                {
                    // Exceptions pouvant survenir durant l'exécution de la requête SQL
                    // return list_of_products[0][0];//return false because the query failed to execute

                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** Exception 3 *********************");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :: " + ex.Message);
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :: " + ex.StackTrace);
                    connection.Close(); //disconnect from database
                    logFileWriter.Flush();
                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                    return null;
                }
            }

            return list_of_cmd_lines;
        }

        public static string[,] insert_DESADV_Veolog(string reference_DESADV_doc, Veolog_DESADV dh, List<Veolog_DESADV_Lines> dl, string fileName, StreamWriter logFileWriter, string logFileName_import, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
        {
            string METHODE_NAME = "insert_DESADV_Veolog()";
            string[,] list_of_cmd_lines = new string[dl.Count, 82];    // new string [x,y]
            string[] list_of_client_info = null;

            int position_item = 0;
            DateTime d = DateTime.Now;
            string curr_date = d.ToString("yyyy-MM-dd");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;

            using (OdbcConnection connection = ConnexionManager.CreateOdbcConnexionSQL()) //connecting to database as handler
            {
                try
                {
                    // Get all tva
                    // Mapping of delivery date time
                    // Get CMD
                    // Check if CMD is not reliquat
                        // ||=> import the document
                    // if CMD is Reliquat
                        // ||=> 


                    connection.Open(); //opening the connection
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Connexion ouverte.");

                    //Get the list of all Taxes (TVA)
                    //So i can calculate the ttc later
                    List<TVA> tvaList = null;
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Récupére tous les tva");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getAllTVA(true));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getAllTVA(true), connection))
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
                        string year = dh.Date_De_Expedition.Substring(0, 4);
                        string month = dh.Date_De_Expedition.Substring(4, 2);
                        string day = dh.Date_De_Expedition.Substring(6, 2);

                        string hour = "00";
                        string mins = "00";
                        if (dh.Heure_De_Expedition != "" && dh.Heure_De_Expedition.Length == 4)
                        {
                            hour = dh.Heure_De_Expedition.Substring(0, 2);
                            mins = dh.Heure_De_Expedition.Substring(2, 2);
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
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", e.Message, e.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    //Get CMD info
                    string ref_client = "";
                    string nature_op_p_ = "";
                    string do_totalHT_ = "";
                    string do_totalHTNet_ = "";
                    string do_totalTTC_ = "";
                    string do_NetAPayer_ = "";
                    string do_MontantRegle_ = "";
                    string CO_No = "";
                    string DO_Reliquat = "0"; // 0 => Non Reliquat | 1 => Un Reliquat
                    int total_lines = 0; // number of lines in the order

                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Récupérer des informations de la commande la référence " + dh.Ref_Commande_Donneur_Ordre);
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getRefCMDClient(true, dh.Ref_Commande_Donneur_Ordre));
                    try
                    {
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getRefCMDClient(true, dh.Ref_Commande_Donneur_Ordre), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    dh.Ref_Commande_Client_Livre = reader[0].ToString();
                                    nature_op_p_ = reader[1].ToString();
                                    do_totalHT_ = reader[2].ToString().Replace(",", ".");
                                    do_totalHTNet_ = reader[3].ToString().Replace(",", ".");
                                    do_totalTTC_ = reader[4].ToString().Replace(",", ".");
                                    do_NetAPayer_ = reader[5].ToString().Replace(",", ".");
                                    do_MontantRegle_ = reader[6].ToString().Replace(",", ".");
                                    ref_client = reader[7].ToString();
                                    CO_No = reader[8].ToString();
                                    DO_Reliquat = reader[9].ToString();
                                    total_lines = (reader[10].ToString() != null && reader[10].ToString() != "null" && reader[10].ToString() != "NULL" && reader[10].ToString() != "" ? Convert.ToInt16(reader[10].ToString()) : 0);
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucune commande trouvé!. ");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. La commande " + dh.Ref_Commande_Donneur_Ordre + " n'exist pas dans la base Sage", "La commande " + dh.Ref_Commande_Donneur_Ordre + " n'exist pas dans la BDD", "", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }
                    }catch(Exception e)
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ******************** Erreur SQL GET CMD ********************");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Message => "+e.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace => " + e.StackTrace);
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. Le client " + ref_client + " n'existe pas dans Sage", "Le client " + ref_client + " n'existe pas dans la BDD", "Aucune reponse. list_of_client_info est null", fileName, logFileName_import));
                        return null;
                    }


                    //get Client Reference by Ref
                    logFileWriter.WriteLine("");
                    logFileWriter.Flush();
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Obtenir les infos client par la référence.");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getClientReferenceById_DESADV(true, ref_client));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceById_DESADV(true, ref_client), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                list_of_client_info = new string[15];
                                list_of_client_info[0] = reader[0].ToString();      // CT_Num
                                list_of_client_info[1] = reader[1].ToString();      // CA_Num 
                                list_of_client_info[2] = reader[2].ToString();      // CG_NumPrinc
                                list_of_client_info[3] = reader[3].ToString();      // CT_NumPayeur
                                list_of_client_info[4] = reader[4].ToString();      // N_Condition
                                list_of_client_info[5] = reader[5].ToString();      // N_Devise
                                list_of_client_info[6] = reader[6].ToString();      // CT_Langue
                                list_of_client_info[7] = reader[7].ToString();      // DO_NbFacture = CT_Facture
                                list_of_client_info[8] = reader[8].ToString().Replace(',', '.');      // DO_TxEscompte = CT_Taux02
                                list_of_client_info[9] = reader[9].ToString();      // N_CatCompta
                                list_of_client_info[10] = reader[10].ToString();    // CO_No
                                list_of_client_info[11] = reader[11].ToString();    //  DO_Tarif = N_CatTarif
                                list_of_client_info[12] = reader[12].ToString();    //  DO_Expedit = N_Expedition du tier
                                list_of_client_info[13] = reader[13].ToString();    //  CT_Intitule
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Client trouvé.");
                            }
                            else// If no rows returned
                            {
                                // do nothing.
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucune reponse. list_of_client_info est null");
                            }
                        }
                    }

                    logFileWriter.WriteLine("");
                    logFileWriter.Flush();

                    //get client delivery adress
                    if (list_of_client_info != null)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getClientDeliveryAddress_DESADV(true, ref_client));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientDeliveryAddress_DESADV(true, ref_client), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_client_info[14] = reader[0].ToString();    // LI_No
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Adresse de livraison (" + reader[0].ToString() + ") trouvé!");
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucune reponse. list_of_client_info est null");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. L'adresse de livraison du client " + ref_client + " n'est pas trouvé dans Sage", "L'adresse de livraison du client " + ref_client + " n'est pas trouvé dans Sage", "Aucune reponse. list_of_client_info est null", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }
                    }
                    else
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ******************** Erreur ********************");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucun client trouver.");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Import annulée");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. Le client " + ref_client + " n'existe pas dans Sage", "Le client " + ref_client + " n'existe pas dans la BDD", "Aucune reponse. list_of_client_info est null", fileName, logFileName_import));
                        return null;
                    }

                    logFileWriter.Flush();


                    // get all DO_Piece lines
                    List<Import.Classes.OrderLines> tmpOrderLines;
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getOrderLinesByDoPiece(true, dh.Ref_Commande_Donneur_Ordre));

                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getOrderLinesByDoPiece(true, dh.Ref_Commande_Donneur_Ordre), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            tmpOrderLines = new List<Import.Classes.OrderLines>();
                            if (reader.Read()) // If any rows returned
                            {
                                tmpOrderLines.Add(new Import.Classes.OrderLines(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                                while (reader.Read())
                                {
                                    tmpOrderLines.Add(new Import.Classes.OrderLines(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                                }
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                tmpOrderLines = null;
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucune reponse. ");
                            }
                        }
                    }

                    
                    bool isLastBlReliquat = true;
                    List<DocumentVenteLine> produitReliquat = new List<DocumentVenteLine>();
                    List<Import.Classes.OrderLines> reliquatOrderLines = new List<Import.Classes.OrderLines>();
                    int counter = 0;
                    List<Database.Model.Reliquat>  reliquat = new List<Database.Model.Reliquat>();


                    foreach (Veolog_DESADV_Lines line in dl) //read item by item
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
                        string DL_PieceBC = "";
                        string DL_DateBC = "";
                        string DL_QteBC = "";
                        string DL_QtePL = ""; //Qte Livrées
                        string DL_LigneBC = "";

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Lire la ligne de l'article.");

                        Connexion.ConnexionSaveLoad connexionSaveLoad = new Connexion.ConnexionSaveLoad();
                        connexionSaveLoad.Load();

                        if ((connexionSaveLoad.configurationConnexion.SQL.PREFIX.Equals("CFCI") ||
                            connexionSaveLoad.configurationConnexion.SQL.PREFIX.Equals("TABLEWEAR")) &&
                            line.Quantite_Colis.Equals("0"))
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : L'article " + line.Code_Article + " est retiré de la commande " + dh.Ref_Commande_Donneur_Ordre + ", alors on intègre pas. ");
                            continue;
                        }

                        //get Product Name By Reference
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getProductNameByReference_DESADV(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getProductNameByReference_DESADV(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    ref_article = (reader[0].ToString());                   // get product ref
                                    name_article = (reader[1].ToString());                  // sum up the total_negative variable. - check query
                                    DL_PoidsNet = (reader[2].ToString());                   // get unit weight NET - check query
                                    DL_PoidsBrut = (reader[3].ToString());                  // get unit weight BRUT - check query  
                                    //DL_PrixUnitaire_buyPrice = (reader[4].ToString());    // get (Prix d'achat) unit price - check query 
                                    DL_PrixUnitaire_salePriceHT = (reader[4].ToString());   // get (Prix de vente) unit price ht - check query
                                    COLIS_article = reader[5].ToString();
                                    PCB_article = reader[6].ToString();
                                    COMPLEMENT_article = reader[7].ToString();
                                    DL_Taxe1 = reader[8].ToString();
                                    DL_CodeTaxe1 = reader[9].ToString();
                                    DL_PieceBC = reader[10].ToString();
                                    DL_DateBC = reader[11].ToString();
                                    DL_QteBC = reader[12].ToString().Replace(",", ".");
                                    DL_QtePL = reader[13].ToString().Replace(",", ".");
                                    DL_LigneBC = reader[14].ToString().Replace(",", ".");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Info du produit " + line.Code_Article + " trouvé");

                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : L'article \"" + line.Code_Article + "\" n'est pas trouvé dans la commande " + dh.Ref_Commande_Donneur_Ordre + ".");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Alors je saute cette article.");
                                    // logFileWriter.Flush();
                                    // recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. L'article \"" + line.Code_Article + "\" n'est pas trouvé dans le champ CodeBare ou dans la base Sage", "L'article \"" + line.Code_Article + "\" n'est pas trouvé dans la BDD.", "", fileName, logFileName_import));
                                    // return null;
                                    continue;
                                }
                            }
                        }


                        Import.Classes.OrderLines sageOrderLine = tmpOrderLines.Find(item => item.ar_codebarre == line.Code_Article);
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine("");
                        // tmpOrderLines.Exists(item => item.do_piece == dh.Ref_Commande_Donneur_Ordre && item.ar_codebarre == line.Code_Article);

                        logFileWriter.WriteLine("tmpOrderLines: \n" + new Database.Database().JsonFormat(tmpOrderLines));

                        logFileWriter.WriteLine("sageOrderLine: \n" + new Database.Database().JsonFormat(sageOrderLine));

                        if (sageOrderLine != null)
                        {
                            Import.Classes.OrderLines orderLine = sageOrderLine;

                            if (sageOrderLine.dl_qte.Split(',')[0] == line.Quantite_Colis)
                            {
                                // continue import normally
                                orderLine.isReliquat = false;
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Article => " + line.Code_Article);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Qte commandé : " + sageOrderLine.dl_qte.Split(',')[0] + " == Qte livrer : " + line.Quantite_Colis);
                                logFileWriter.WriteLine("");
                            }
                            else
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Article => " + line.Code_Article);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Qte commandé : " + sageOrderLine.dl_qte.Split(',')[0] + " != Qte livrer : " + line.Quantite_Colis);
                                logFileWriter.WriteLine("");

                                DO_Reliquat = "1";
                                isLastBlReliquat = false;

                                orderLine.dl_qte = (Convert.ToInt32(DL_QteBC.Split('.')[0]) - Convert.ToInt32(line.Quantite_Colis)).ToString();
                                orderLine.dl_qtepl = (Convert.ToInt32(DL_QtePL.Split('.')[0]) - Convert.ToInt32(line.Quantite_Colis)).ToString();
                                orderLine.isReliquat = true;
                            }
                            reliquatOrderLines.Add(orderLine);
                        }
                        else
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ******************** Warning Order/Reliquat ********************");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Le produit \""+ line.Code_Article + "\" dans \""+ dh.Ref_Commande_Donneur_Ordre + "\" n'existe pas dans la commande sage, alors j'importe...");
                            logFileWriter.WriteLine("");
                        }

                        /*
                        if (DL_QteBC.Split('.')[0].Equals(line.Quantite_Colis))
                        {
                            // continue import normally
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Article => "+ line.Code_Article);
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Qte commandé : " + DL_QteBC.Split('.')[0] + " == Qte livrer : " + line.Quantite_Colis);
                            logFileWriter.WriteLine("");
                        }
                        else
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Article => " + line.Code_Article);
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Qte commandé : " + DL_QteBC.Split('.')[0] + " != Qte livrer : " + line.Quantite_Colis);
                            logFileWriter.WriteLine("");

                            DO_Reliquat = "1";
                            isLastBlReliquat = false;
                            DocumentVenteLine doc = new DocumentVenteLine();
                            doc.AR_Ref = ref_article;
                            doc.AR_CODEBARRE = line.Code_Article;
                            doc.DL_Qte = (Convert.ToInt32(DL_QteBC.Split('.')[0]) - Convert.ToInt32(line.Quantite_Colis)).ToString();
                            doc.DL_QtePL = (Convert.ToInt32(DL_QteBC.Split('.')[0]) - Convert.ToInt32(line.Quantite_Colis)).ToString();

                            produitReliquat.Add(doc);
                        }
                        */


                        if (ref_article != "" && name_article != "" && list_of_client_info != null)
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
                                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : List des TVA trouvé");
                                        TVA tva = null;
                                        bool tva_error = false;
                                        foreach (TVA tva_ in tvaList)
                                        {
                                            if (DL_CodeTaxe1 != null && DL_CodeTaxe1 != "" && tva_.TA_Code == DL_CodeTaxe1)
                                            {
                                                tva = tva_;
                                                tva_error = true;
                                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : TVA " + tva.TA_Code + " trouvé \"" + tva.TA_Taux + "\"");
                                                break;
                                            }
                                            else
                                            {
                                                if (DL_CodeTaxe1 == null)
                                                {
                                                    //tva = tva_;
                                                    tva_error = false;
                                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : TVA NULL trouvé, alors TVA mis à 0");
                                                    break;
                                                }
                                                else if (DL_CodeTaxe1 == "")
                                                {
                                                    //tva = tva_;
                                                    tva_error = false;
                                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : TVA VIDE trouvé, alors TVA mis à 0");
                                                    break;
                                                }
                                            }
                                        }

                                        string endTVA = null;
                                        if (tva_error)
                                        {
                                            endTVA = tva.TA_Taux;
                                        }
                                        else
                                        {
                                            DL_CodeTaxe1 = "C00";
                                            endTVA = "0,000000";
                                        }
                                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : endTVA = " + endTVA);

                                        double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                        double product_20_P = (product_ht * Convert.ToDouble(endTVA)) / 100;
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
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Erreur lors du calcule du prix d'article TTC, message :\n" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Erreur lors du calcule du prix d'article TTC, message : " + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }

                                // input DL_DateBC: 03 - 01 - 2020 00:00:00;
                                string original_date = DL_DateBC;
                                string date = original_date.Split(' ')[0];
                                string time = original_date.Split(' ')[1];
                                DL_DateBC = date.Split('/')[2] + "-" + date.Split('/')[1] + "-" + date.Split('/')[0] + " " + time;
                                // ouput DL_DateBC: 2020-01-03 00:00:00;

                                // DESADV prefix will be used to create document
                                list_of_cmd_lines[counter, 0] = "0"; // DO_Domaine
                                list_of_cmd_lines[counter, 1] = "3"; //DO_Type
                                list_of_cmd_lines[counter, 2] = "3"; //DO_DocType
                                list_of_cmd_lines[counter, 3] = list_of_client_info[0]; //CT_NUM
                                list_of_cmd_lines[counter, 4] = reference_DESADV_doc; //DO_Piece
                                list_of_cmd_lines[counter, 5] = curr_date; //DO_Date
                                list_of_cmd_lines[counter, 6] = DL_DateBC; //DL_DateBC
                                list_of_cmd_lines[counter, 7] = (position_item).ToString(); // DL_Ligne line number 1000,2000
                                list_of_cmd_lines[counter, 8] = dh.Ref_Commande_Client_Livre; // DO_Ref
                                list_of_cmd_lines[counter, 9] = ref_article; // AR_Ref
                                list_of_cmd_lines[counter, 10] = "1"; //DL_Valorise
                                list_of_cmd_lines[counter, 11] = dh.Entrepot; //DE_NO
                                list_of_cmd_lines[counter, 12] = name_article.Replace("'", "''"); // DL_Design
                                list_of_cmd_lines[counter, 13] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");  //line.Quantite_Colis; // DL_Qte
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
                                list_of_cmd_lines[counter, 20] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", "."); // EU_Qte; // EU_Qte
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }

                                list_of_cmd_lines[counter, 21] = (Convert.ToDouble(line.Quantite_Colis) * Convert.ToDouble(DL_PrixUnitaire_salePriceHT)).ToString().Replace(",", "."); //DL_MontantHT
                                list_of_cmd_lines[counter, 22] = (Convert.ToDouble(line.Quantite_Colis) * product_ttc).ToString().Replace(",", "."); //DL_MontantTTC
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }
                                if (list_of_cmd_lines[counter, 21].Equals("0")) { list_of_cmd_lines[counter, 21] = "0.000000"; } else if (!list_of_cmd_lines[counter, 21].Contains(".")) { list_of_cmd_lines[counter, 21] = list_of_cmd_lines[counter, 21] + ".0"; }
                                if (list_of_cmd_lines[counter, 22].Equals("0")) { list_of_cmd_lines[counter, 22] = "0.000000"; } else if (!list_of_cmd_lines[counter, 22].Contains(".")) { list_of_cmd_lines[counter, 22] = list_of_cmd_lines[counter, 22] + ".000000"; }

                                list_of_cmd_lines[counter, 23] = ""; //PF_Num
                                list_of_cmd_lines[counter, 24] = "0"; //DL_No
                                list_of_cmd_lines[counter, 25] = "0"; //DL_FactPoids
                                list_of_cmd_lines[counter, 26] = "0"; //DL_Escompte
                                list_of_cmd_lines[counter, 27] = DL_PUTTC; //DL_PUTTC
                                list_of_cmd_lines[counter, 28] = "0";   //DL_TTC

                                list_of_cmd_lines[counter, 29] = DL_PieceBC;   //DL_PieceBC
                                list_of_cmd_lines[counter, 30] = reference_DESADV_doc;   //DL_PieceBL
                                list_of_cmd_lines[counter, 31] = curr_date;   // DL_DateBL
                                list_of_cmd_lines[counter, 32] = "0";   //DL_TNomencl
                                list_of_cmd_lines[counter, 33] = "0";   //DL_TRemPied
                                list_of_cmd_lines[counter, 34] = "0";   //DL_TRemExep
                                list_of_cmd_lines[counter, 35] = DL_QteBC.Replace(",", ".");   //DL_QteBC
                                list_of_cmd_lines[counter, 36] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");   //DL_QteBL
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
                                list_of_cmd_lines[counter, 54] = CO_No;                    //CO_No
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

                                list_of_cmd_lines[counter, 72] = "3";                                           // DL_MvtStock
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
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ******************** Exception ********************");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Le tableau 'BL' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Le tableau 'BL' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                            counter++;
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Compter => " + counter);
                        }
                        else
                        {
                            counter--;
                            logFileWriter.WriteLine("Aucun article trouvé ou Aucun information client trouvé !");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Compter => " + counter);
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Aucun article trouvé ou Aucun information client trouvé !", "", fileName, logFileName_import));
                            return null;
                        }


                    }
                    // ===== End Foreach =====

                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Vérifier si un produit pour 0 = BL");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertDesadvDocument_Veolog(true, "3", reference_DESADV_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_client_info, dh.Etat, CO_No));


                    //generate document BL_____. in database.
                    try
                    {
                        OdbcCommand command = new OdbcCommand(QueryHelper.insertDesadvDocument_Veolog(true, "3", reference_DESADV_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_client_info, dh.Etat, CO_No), connection); //calling the query and parsing the parameters into it
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
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }


                    string[,] products_DESADV = new string[position_item / 1000, 82]; // create array with enough space

                    //insert documentline into the database with articles having 20 as value @index 2
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : insert documentline into the database with articles having 3 as value @index 2");

                    for (int x = 0; x < list_of_cmd_lines.GetLength(0); x++)
                    {
                        if (list_of_cmd_lines[x, 1] == "3")
                        {
                            logFileWriter.WriteLine("");
                            for (int y = 0; y < list_of_cmd_lines.GetLength(1); y++)
                            {
                                products_DESADV[x, y] = list_of_cmd_lines[x, y];
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : products_BL_L[" + x + "," + y + "] = " + products_DESADV[x, y]);
                            }

                            //insert the article to documentline in the database
                            try
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : insert the article " + products_DESADV[x, 12] + " (Ref:" + products_DESADV[x, 9] + ") to documentline in the database");

                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : requette sql ===> " + QueryHelper.insertDesadvDocumentLine_Veolog(true, products_DESADV, x));

                                OdbcCommand command = new OdbcCommand(QueryHelper.insertDesadvDocumentLine_Veolog(true, products_DESADV, x), connection);
                                command.ExecuteReader();
                            }
                            catch (OdbcException ex)
                            {
                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter);
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ********************** OdbcException *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }


                            //Update docline artile stock
                            try
                            {
                                bool found_stock = false;
                                double AS_StockReel = 0.0;
                                double AS_StockReserve = 0.0;
                                double AS_StockMontant = 0.0;
                                double productPrixUnite = Convert.ToDouble(products_DESADV[x, 16].Replace('.', ','));

                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : get current stock in F_ARTSTOCK table in the database");
                                logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : requette sql ===> " + QueryHelper.getArticleStock(true, products_DESADV[x, 9], dh.Entrepot));
                                using (OdbcCommand command_ = new OdbcCommand(QueryHelper.getArticleStock(true, products_DESADV[x, 9], dh.Entrepot), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                                {
                                    using (IDataReader reader = command_.ExecuteReader()) // read rows of the executed query
                                    {
                                        if (reader.Read()) // If any rows returned
                                        {
                                            found_stock = true;
                                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Stock trouvé : AS_StockReel (" + reader[0].ToString() + "), AS_StockReserve (" + reader[1].ToString() + "), AS_StockMontant (" + reader[2].ToString() + ").");
                                            AS_StockReel = Convert.ToDouble(reader[0].ToString().Replace(".", ","));
                                            AS_StockReserve = Convert.ToDouble(reader[1].ToString().Replace(".", ","));
                                            AS_StockMontant = Convert.ToDouble(reader[2].ToString().Replace(".", ","));
                                        }
                                        else// If no rows returned
                                        {
                                            //do nothing.
                                            found_stock = false;
                                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Aucune reponse.");
                                            logFileWriter.WriteLine("");
                                            deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter);
                                            logFileWriter.WriteLine("");
                                            logFileWriter.Flush();
                                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Echec à la récupértion du stock de l'article " + products_DESADV[x, 9], "", fileName, logFileName_import));
                                            return null;
                                        }
                                    }
                                }


                                if (found_stock)
                                {
                                    //Calculate stock info
                                    //double AS_CMUP = AS_StockMontant / AS_StockReel;
                                    double new_AS_StockReel = AS_StockReel - Convert.ToDouble(products_DESADV[x, 13].Replace('.', ','));
                                    double new_AS_StockReserve = AS_StockReserve - Convert.ToDouble(products_DESADV[x, 13].Replace('.', ','));
                                    double new_AS_StockMontant = new_AS_StockReel * productPrixUnite;
                                    double new_AS_CMUP = new_AS_StockMontant / new_AS_StockReel;

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : new_AS_StockReel: " + new_AS_StockReel + " = AS_StockReel: " + AS_StockReel + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_DESADV[x, 13].Replace('.', ',')));
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : new_AS_StockReserve: " + new_AS_StockReserve + " = AS_StockReserve: " + AS_StockReserve + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_DESADV[x, 13].Replace('.', ',')));
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : new_AS_StockMontant: " + new_AS_StockMontant + " = new_AS_StockReel: " + new_AS_StockReel + " * productPrixUnite: " + productPrixUnite);
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : new_AS_CMUP: " + new_AS_CMUP + " = new_AS_StockMontant: " + new_AS_StockMontant + " / new_AS_StockReel: " + new_AS_StockReel);

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : update article " + products_DESADV[x, 12] + " (Ref:" + products_DESADV[x, 9] + ") stock in F_ARTSTOCK table in the database");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : requette sql ===> " + QueryHelper.updateArticleStock(true, products_DESADV[x, 9], new_AS_StockReel, new_AS_StockReserve, new_AS_StockMontant, dh.Entrepot));

                                    OdbcCommand command = new OdbcCommand(QueryHelper.updateArticleStock(true, products_DESADV[x, 9], new_AS_StockReel, new_AS_StockReserve, new_AS_StockMontant, dh.Entrepot), connection);
                                    command.ExecuteReader();
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

                                logFileWriter.WriteLine("");
                                if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                                {
                                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                                }
                                logFileWriter.WriteLine("");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    // get DE_No name from db
                    string DE_No_Name = "";
                    try
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Récupérer le depot.");

                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.getDepotById(true, dh.Entrepot));
                        using (OdbcCommand command_ = new OdbcCommand(QueryHelper.getDepotById(true, dh.Entrepot), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command_.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Depot trouvé");
                                    DE_No_Name = reader[0].ToString();
                                }
                                else// If no rows returned
                                {
                                    //put the depot name empty.
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Depot non trouvé");
                                    DE_No_Name = "";
                                }
                            }
                        }
                        logFileWriter.Flush();
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Warning Depot ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Depot non trouvé");
                        logFileWriter.Flush();
                        DE_No_Name = "";
                        logFileWriter.Flush();
                    }

                    //set logistic name and date time import in "Complement" field
                    try
                    {
                        string delivery_date_veolog = string.Format("{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Ajouter la date de livraision \"" + delivery_date_veolog + "\" de \""+ DE_No_Name + "\" au DESADV \"" + reference_DESADV_doc + "\".");

                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.updateComplementDeliveryDate(true, reference_DESADV_doc, DE_No_Name + "  "+delivery_date_veolog + "  " + dh.Ref_Commande_Donneur_Ordre));
                        OdbcCommand command = new OdbcCommand(QueryHelper.updateComplementDeliveryDate(true, reference_DESADV_doc, DE_No_Name + "  " + delivery_date_veolog + "  " + dh.Ref_Commande_Donneur_Ordre), connection);
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
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();

                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    //Delete the BC of the BL if BL is good or last BL reliquat
                    // check if current BC lines is equal to BL lines
                    if (total_lines != dl.Count)
                    {
                        isLastBlReliquat = false;
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Numbre total de produit dans la commande \"" + total_lines + "\" est différent de celui dans le BL \"" + dl.Count + "\"");
                    }

                    if (isLastBlReliquat)
                    {
                        try
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Pas de BL Reliquat en attente");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Supprimer le Bon de Commande (BC) \"" + dh.Ref_Commande_Donneur_Ordre + "\".");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre));
                            OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre), connection);
                            IDataReader reader = command.ExecuteReader();
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Bon de Commande supprimé!");
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            logFileWriter.Flush();

                            logFileWriter.WriteLine("");
                            if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                            {
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                            }
                            logFileWriter.WriteLine("");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }
                    }
                    else
                    {
                        // Update BC : reliquat && lines (qte, prix ht ,ttc,tva)
                        try
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : En attente de BL Reliquat pour la prochaine fois...");
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Mise à jour du Bon de Commande (BC) en ENTETE \"" + dh.Ref_Commande_Donneur_Ordre + "\".");

                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.updateCommandeReliquat(true, dh.Ref_Commande_Donneur_Ordre));
                            OdbcCommand command = new OdbcCommand(QueryHelper.updateCommandeReliquat(true, dh.Ref_Commande_Donneur_Ordre), connection);
                            IDataReader reader = command.ExecuteReader();
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Bon de Commande ENTETE Mise à jour");

                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine("reliquatOrderLines list : " + reliquatOrderLines.Count);
                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Mise à jour du Bon de Commande (BC) en LIGNE \"" + dh.Ref_Commande_Donneur_Ordre + "\".");

                            for (int x = 0; x < reliquatOrderLines.Count; x++)
                            {
                                logFileWriter.WriteLine("");

                                // if order line is waiting on a futur reliquat
                                // ie order product qte is different to BL product qte
                                // update order product
                                if (reliquatOrderLines[x].isReliquat)
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Mettre à jour le produit : \"" + reliquatOrderLines[x].dl_ligne + "\" - \"" + reliquatOrderLines[x].dl_design+ "\" ("+ reliquatOrderLines[x].ar_codebarre+") dans \"" + dh.Ref_Commande_Donneur_Ordre + "\" ");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.updateCommandeLigneReliquat(true, dh.Ref_Commande_Donneur_Ordre, reliquatOrderLines[x]));
                                    OdbcCommand command_ = new OdbcCommand(QueryHelper.updateCommandeLigneReliquat(true, dh.Ref_Commande_Donneur_Ordre, reliquatOrderLines[x]), connection);
                                    IDataReader reader_ = command_.ExecuteReader();
                                }
                                else
                                {
                                    // if order line is equal/not waiting on a futur reliquat
                                    // ie order product qte is equal to BL product qte
                                    // remove order product
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Retirer le produit : \"" + reliquatOrderLines[x].dl_ligne + "\" - \"" + reliquatOrderLines[x].dl_design + "\" (" + reliquatOrderLines[x].ar_codebarre + ") dans \"" + dh.Ref_Commande_Donneur_Ordre + "\" ");
                                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.deleteCommandeLine(true, dh.Ref_Commande_Donneur_Ordre, reliquatOrderLines[x]));
                                    OdbcCommand command_ = new OdbcCommand(QueryHelper.deleteCommandeLine(true, dh.Ref_Commande_Donneur_Ordre, reliquatOrderLines[x]), connection);
                                    IDataReader reader_ = command_.ExecuteReader();
                                }


                            }

                            logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Bon de Commande (Reliquat) Mise à jour");
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            logFileWriter.Flush();

                            logFileWriter.WriteLine("");
                            if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                            {
                                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                            }
                            logFileWriter.WriteLine("");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }
                    }

                    //update document numbering
                    try
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Mettre à jour la numérotation du document \"" + reference_DESADV_doc + "\".");

                        logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : SQL ===> " + QueryHelper.updateDOC_NumerotationTable(true, "BL", reference_DESADV_doc));
                        OdbcCommand command = new OdbcCommand(QueryHelper.updateDOC_NumerotationTable(true, "BL", reference_DESADV_doc), connection);
                        IDataReader reader = command.ExecuteReader();
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Nouvelle numérotation à jour!");
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();

                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }


                }
                catch (Exception ex)
                {
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : ********************** Exception Main *********************");
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : Message :: " + ex.Message);
                    logFileWriter.WriteLine(DateTime.Now + " | " + METHODE_NAME + " : StackTrace :: " + ex.StackTrace);
                    connection.Close(); //disconnect from database
                    logFileWriter.Flush();
                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
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
