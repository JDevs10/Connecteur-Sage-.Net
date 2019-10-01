using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data;
using importPlanifier.Helpers;
using System.IO;
using System.Globalization;

namespace importPlanifier.Classes
{
    class ExportFactures
    {

        //private List<DocumentVente> FacturesAExporter;
        //private Customer client = new Customer();

        private string logDirectoryName_export = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Export" + @"\" + "FACTURE";
        private StreamWriter logFileWriter_export = null;

        private string pathExport;

        public ExportFactures()
        {

        }

        public ExportFactures(string path)
        {
            this.pathExport = path;
        }

        private List<DocumentVente> GetFacturesFromDataBase()
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVente> listDocumentVente = new List<DocumentVente>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVente(67), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DocumentVente documentVente = new DocumentVente(reader[0].ToString(), reader[1].ToString(),
                                    reader[2].ToString().Replace("00:00:00", ""), reader[3].ToString().Replace("00:00:00", ""), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(),
                                    reader[8].ToString(), reader[9].ToString(),
                                    reader[10].ToString(), reader[11].ToString(),
                                    reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(),
                                    reader[16].ToString(), reader[17].ToString(), reader[18].ToString(), reader[19].ToString(),
                                    reader[20].ToString(), reader[21].ToString(), reader[22].ToString(), reader[23].ToString(),
                                     reader[24].ToString(), reader[25].ToString(), reader[26].ToString(), reader[27].ToString(),
                                     reader[28].ToString(), reader[29].ToString(), reader[30].ToString(), reader[31].ToString(),
                                     reader[32].ToString(), reader[33].ToString()
                                    );
                                if (documentVente.DO_Statut == "0")
                                {
                                    documentVente.DO_Statut = "Saisie";
                                }
                                if (documentVente.DO_Statut == "1")
                                {
                                    documentVente.DO_Statut = "Confirmé";
                                }
                                if (documentVente.DO_Statut == "2")
                                {
                                    documentVente.DO_Statut = "Accepté";
                                }
                                listDocumentVente.Add(documentVente);
                            }
                        }
                    }
                    return listDocumentVente;

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }

        private List<DocumentVenteLine> getDocumentLine(string codeDocument)
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVenteLine> lignesDocumentVente = new List<DocumentVenteLine>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVenteLine(codeDocument), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DocumentVenteLine ligne = new DocumentVenteLine(reader[0].ToString().Replace("00:00:00", ""), reader[1].ToString().Replace("00:00:00", ""),
                                    reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(),
                                    reader[8].ToString(), reader[9].ToString(),
                                    reader[10].ToString(), reader[11].ToString(),
                                    reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(),
                                    reader[16].ToString(), reader[17].ToString(), reader[18].ToString(), reader[19].ToString(),
                                    reader[20].ToString(), reader[21].ToString(), reader[22].ToString(), reader[23].ToString(),
                                     reader[24].ToString(), reader[25].ToString(), reader[26].ToString(), reader[27].ToString(),
                                        reader[28].ToString(), reader[29].ToString(), reader[30].ToString(), reader[31].ToString()
                                    );
                                lignesDocumentVente.Add(ligne);
                            }
                        }
                    }
                    return lignesDocumentVente;

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }

        private Customer GetClient(string do_tiers)
        {
            try
            {
                //List<Customer> listClient = new List<Customer>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getCustomer(do_tiers), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Customer(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(), reader[16].ToString());
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }

        private void UpdateDocumentVente(string do_piece)
        {
            try
            {
                //List<Customer> listClient = new List<Customer>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.updateDocumentdeVente(do_piece), connection);
                    command.ExecuteNonQuery();
                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
            }
        }


        public static string ConvertDate(string date)
        {
            if (date.Length == 11 || date.Length == 19)
            {
                return date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2);
            }
            return date;
        }


        public static string addZero(string date)
        {
            if (date.Length == 1)
            {
                return "0" + date;
            }
            return date;
        }

        /// <summary>
        /// Génération du fichier d'export, lancement de l'application et exporter les factures
        /// </summary>
        //public void ExportFacture()
        //{
        //    try
        //    {
        //        List<DocumentVente> FacturesAExporter = GetFacturesFromDataBase();

        //        if (FacturesAExporter != null)
        //        {
        //            string outputFile = this.pathExport + @"\Fichier Exporter\Factures\";

        //            if (!Directory.Exists(outputFile))
        //            {
        //                System.IO.Directory.CreateDirectory(outputFile);
        //            }

        //            for (int i = 0; i < FacturesAExporter.Count; i++)
        //            {

        //                Customer customer = GetClient(FacturesAExporter[i].DO_TIERS);

        //                var fileName = string.Format("INV-{0:HHmmss}.{0:yyyyMMdd}." + customer.CT_Num + "." + customer.CT_EDI1 + ".csv", DateTime.Now);

        //                using (StreamWriter writer = new StreamWriter(outputFile + @"\" + fileName, false, Encoding.UTF8))
        //                {
        //                    writer.WriteLine("DEMAT-AAA;v01.0;;;" + DateTime.Today.Year + addZero(DateTime.Today.Month.ToString()) + addZero(DateTime.Today.Day.ToString()) + ";;");
        //                    writer.WriteLine("");
        //                    writer.WriteLine("");



        //                        //string[] tab = new string[] { "", "", "" };



        //                        //if (FacturesAExporter[i].OriginDocumentType == "8")
        //                        //{
        //                        //    tab = GetCommandeFacture(FacturesAExporter[i].Id).Split(';');
        //                        //}

        //                        string modeReglement = GetModeReglement(FacturesAExporter[i].DO_Piece);


        //                        writer.WriteLine("DEMAT-HD1;v01.0;;" + FacturesAExporter[i].DO_Piece.Replace("FA", "") + ";380;9;" + ConvertDate(FacturesAExporter[i].DO_date) + ";" + ConvertDate(FacturesAExporter[i].DO_dateLivr) + ";;;;;" + modeReglement + ";;;;0;;" + FacturesAExporter[i].DO_COORD01 + ";;" + FacturesAExporter[i].DO_COORD01 + ";;;;;;;;;;;;;;;;;;;;;;;;EUR;;;;;" + FacturesAExporter[i].FNT_Escompte.Replace(",", ".").Replace("00000", "") + ";;;;;;;;;;;;;");
        //                        writer.WriteLine("");

        //                        writer.WriteLine("DEMAT-HD2;" + FacturesAExporter[i].DO_Piece + ";;" + customer.CT_Adresse + ";" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + customer.CT_Pays + ";;;;;;;;;;;;;;;;;;;;;;;;;;;" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + FacturesAExporter[i].LI_PAYS + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
        //                        writer.WriteLine("");

        //                        //writer.WriteLine("DEMAT-CTH;1; Type de Ligne Texte Libre (voir liste ci-dessous) ; Texte libre ; Texte libre ; Texte libre ; Texte libre ; Texte libre ;");

        //                        //writer.WriteLine("DEMAT-CTA;" + FacturesAExporter[i].InvoicingContact_Function + ";;" + FacturesAExporter[i].InvoicingContact_Name + " " + FacturesAExporter[i].InvoicingContact_FirstName + ";" + FacturesAExporter[i].InvoicingContact_Email + ";" + FacturesAExporter[i].InvoicingContact_Fax + ";" + FacturesAExporter[i].InvoicingContact_Phone + ";" + FacturesAExporter[i].InvoicingContact_Function + ";;" + FacturesAExporter[i].InvoicingContact_Name + " " + FacturesAExporter[i].InvoicingContact_FirstName + ";" + FacturesAExporter[i].InvoicingContact_Email + ";" + FacturesAExporter[i].InvoicingContact_Fax + ";" + FacturesAExporter[i].InvoicingContact_Phone + ";;;;;;;;;;;;;;;;;;;" + FacturesAExporter[i].DeliveryContact_Function + ";;" + FacturesAExporter[i].DeliveryContact_Name + " " + FacturesAExporter[i].DeliveryContact_FirstName + ";" + FacturesAExporter[i].DeliveryContact_Email + ";" + FacturesAExporter[i].DeliveryContact_Fax + ";" + FacturesAExporter[i].DeliveryContact_Phone + ";;;;;;;");
        //                        //writer.WriteLine("");

        //                        //if (FacturesAExporter[i].DiscountAmount != "0,00000000" || FacturesAExporter[i].DiscountAmount != "0,00000000")
        //                        //{
        //                        //    writer.WriteLine("DEMAT-REM;;A;;;;;;;;" + FacturesAExporter[i].DiscountAmount.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].DiscountRate.Replace(",", ".").Replace("00000", "") + ";;");
        //                        //    writer.WriteLine("");
        //                        //}

        //                        FacturesAExporter[i].lines = getDocumentLine(FacturesAExporter[i].DO_Piece);

        //                        for (int j = 0; j < FacturesAExporter[i].lines.Count; j++)
        //                        {

        //                            writer.WriteLine("DEMAT-LIN;" + FacturesAExporter[i].lines[j].DL_Ligne + ";" + FacturesAExporter[i].lines[j].AR_CODEBARRE + ";EAN;;;" + customer.CT_EDI1 + ";;;;" + FacturesAExporter[i].lines[j].DL_Design + ";;" + FacturesAExporter[i].lines[j].DL_PoidsNet.Replace(",", ".") + ";" + FacturesAExporter[i].lines[j].DL_PoidsBrut.Replace(",", ".") + ";;" + FacturesAExporter[i].lines[j].DL_Qte + ";" + FacturesAExporter[i].lines[j].DL_QteBL + ";" + FacturesAExporter[i].lines[j].EU_Qte + ";;;;" + FacturesAExporter[i].lines[j].FNT_MontantHT.Replace(",", ".") + ";" + FacturesAExporter[i].lines[j].FNT_PrixUNet.Replace(",", ".") + ";;;1;;;" + ConvertDate(FacturesAExporter[i].lines[j].DO_DateLivr.Replace("00:00:00", "")) + ";" + FacturesAExporter[i].lines[j].DL_NoColis + ";;;;;;;;;;;;;" + FacturesAExporter[i].lines[j].FNT_MontantTTC.Replace(",", ".") + ";;;;;;;;");
        //                            writer.WriteLine("");

        //                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe1 == "0")
        //                            {
        //                                FacturesAExporter[i].lines[j].DL_TypeTaxe1 = "TVA/Débit";
        //                            }
        //                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe2 == "0")
        //                            {
        //                                FacturesAExporter[i].lines[j].DL_TypeTaxe2 = "TVA/Débit";
        //                            }
        //                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe3 == "0")
        //                            {
        //                                FacturesAExporter[i].lines[j].DL_TypeTaxe3 = "TVA/Débit";
        //                            }

        //                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe1 == "1")
        //                            {
        //                                FacturesAExporter[i].lines[j].DL_TypeTaxe1 = "TVA/Encaissement";
        //                            }
        //                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe2 == "1")
        //                            {
        //                                FacturesAExporter[i].lines[j].DL_TypeTaxe2 = "TVA/Encaissement";
        //                            }
        //                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe3 == "1")
        //                            {
        //                                FacturesAExporter[i].lines[j].DL_TypeTaxe3 = "TVA/Encaissement";
        //                            }


        //                            if (FacturesAExporter[i].lines[j].DL_Taxe1 != "0")
        //                            {
        //                                writer.WriteLine("DEMAT-TAX;1;;;" + FacturesAExporter[i].lines[j].DL_TypeTaxe1 + ";;" + FacturesAExporter[i].lines[j].DL_Taxe1.Replace(",", ".") + ";;;");
        //                                writer.WriteLine("");
        //                            }
        //                            if (FacturesAExporter[i].lines[j].DL_Taxe2 != FacturesAExporter[i].lines[j].DL_Taxe1 && FacturesAExporter[i].lines[j].DL_Taxe2 != "0")
        //                            {
        //                                writer.WriteLine("DEMAT-TAX;2;;;" + FacturesAExporter[i].lines[j].DL_TypeTaxe2 + ";;" + FacturesAExporter[i].lines[j].DL_Taxe2.Replace(",", ".") + ";;;");
        //                                writer.WriteLine("");
        //                            }

        //                            if ((FacturesAExporter[i].lines[j].DL_Taxe3 != FacturesAExporter[i].lines[j].DL_Taxe1) && (FacturesAExporter[i].lines[j].DL_Taxe3 != FacturesAExporter[i].lines[j].DL_Taxe2) && FacturesAExporter[i].lines[j].DL_Taxe3 != "0")
        //                            {
        //                                writer.WriteLine("DEMAT-TAX;3;;;" + FacturesAExporter[i].lines[j].DL_TypeTaxe3 + ";;" + FacturesAExporter[i].lines[j].DL_Taxe3.Replace(",", ".") + ";;;");
        //                                writer.WriteLine("");
        //                            }

        //                            //---- Remise ----

        //                            string MontantRemise = "";
        //                            string PourcentageRemise = "";

        //                            if (FacturesAExporter[i].lines[j].DL_Remise01REM_Type == "0")
        //                            {
        //                                MontantRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur.Replace(",", ".");
        //                                PourcentageRemise = "";
        //                            }

        //                            if (FacturesAExporter[i].lines[j].DL_Remise01REM_Type == "1")
        //                            {
        //                                MontantRemise = "";
        //                                PourcentageRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur;
        //                            }

        //                            if (FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur != "0")
        //                            {
        //                                writer.WriteLine("DEMAT-DED;;A;;;;;;;" + FacturesAExporter[i].lines[j].DL_Remise01REM_Type + ";" + MontantRemise + ";" + PourcentageRemise + ";;");
        //                                writer.WriteLine("");
        //                            }


        //                            if (FacturesAExporter[i].lines[j].DL_Remise03REM_Type == "0")
        //                            {
        //                                MontantRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur;
        //                                PourcentageRemise = "";
        //                            }

        //                            if (FacturesAExporter[i].lines[j].DL_Remise03REM_Type == "1")
        //                            {
        //                                MontantRemise = "";
        //                                PourcentageRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur;
        //                            }

        //                            if ((FacturesAExporter[i].lines[j].DL_Remise03REM_Valeur != "0") && (FacturesAExporter[i].lines[j].DL_Remise03REM_Valeur != FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur))
        //                            {
        //                                writer.WriteLine("DEMAT-DED;;A;;;;;;;;" + MontantRemise + ";" + PourcentageRemise + ";;");
        //                                writer.WriteLine("");
        //                            }

        //                        }

        //                        //  Les lignes des taxes


        //                        if (FacturesAExporter[i].DO_TypeTaxe1 == "0")
        //                        {
        //                            FacturesAExporter[i].DO_TypeTaxe1 = "TVA/Débit";
        //                        }
        //                        if (FacturesAExporter[i].DO_TypeTaxe2 == "0")
        //                        {
        //                            FacturesAExporter[i].DO_TypeTaxe2 = "TVA/Débit";
        //                        }
        //                        if (FacturesAExporter[i].DO_TypeTaxe3 == "0")
        //                        {
        //                            FacturesAExporter[i].DO_TypeTaxe3 = "TVA/Débit";
        //                        }

        //                        if (FacturesAExporter[i].DO_TypeTaxe1 == "1")
        //                        {
        //                            FacturesAExporter[i].DO_TypeTaxe1 = "TVA/Encaissement";
        //                        }
        //                        if (FacturesAExporter[i].DO_TypeTaxe2 == "1")
        //                        {
        //                            FacturesAExporter[i].DO_TypeTaxe2 = "TVA/Encaissement";
        //                        }
        //                        if (FacturesAExporter[i].DO_TypeTaxe3 == "1")
        //                        {
        //                            FacturesAExporter[i].DO_TypeTaxe3 = "TVA/Encaissement";
        //                        }

        //                        if (FacturesAExporter[i].DO_taxe1 != "0")
        //                        {
        //                            writer.WriteLine("DEMAT-TTX;1;" + FacturesAExporter[i].DO_TypeTaxe1.Replace(",", ".").Replace("00000", "") + ";;;;" + FacturesAExporter[i].DO_taxe1.Replace(",", ".").Replace("00000", "") + ";;");
        //                            writer.WriteLine("");
        //                        }
        //                        if (FacturesAExporter[i].DO_taxe2 != FacturesAExporter[i].DO_taxe1 && FacturesAExporter[i].DO_taxe2 != "0")
        //                        {
        //                            writer.WriteLine("DEMAT-TTX;2;" + FacturesAExporter[i].DO_TypeTaxe2.Replace(",", ".").Replace("00000", "") + ";;;;" + FacturesAExporter[i].DO_taxe1.Replace(",", ".").Replace("00000", "") + ";;");
        //                            writer.WriteLine("");
        //                        }

        //                        if ((FacturesAExporter[i].DO_taxe3 != FacturesAExporter[i].DO_taxe1) && (FacturesAExporter[i].DO_taxe3 != FacturesAExporter[i].DO_taxe2) && FacturesAExporter[i].DO_taxe3 != "0")
        //                        {
        //                            writer.WriteLine("DEMAT-TTX;3;" + FacturesAExporter[i].DO_TypeTaxe3.Replace(",", ".").Replace("00000", "") + ";;;;" + FacturesAExporter[i].DO_taxe1.Replace(",", ".").Replace("00000", "") + ";;");
        //                            writer.WriteLine("");
        //                        }


        //                        writer.WriteLine("DEMAT-END;;;" + FacturesAExporter[i].DO_Piece.Replace("FA", "") + ";" + FacturesAExporter[i].FNT_TotalHT.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].FNT_TotalTTC.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].FNT_MontantTotalTaxes.Replace(",", ".") + ";;;;" + FacturesAExporter[i].FNT_Escompte.Replace(",", ".") + ";;" + FacturesAExporter[i].FNT_NetAPayer.Replace(",", ".") + ";;;;");
        //                        writer.WriteLine("");
        //                        writer.WriteLine("");


        //                    writer.WriteLine("DEMAT-ZZZ;v01.0;;;;");


        //                }

        //                UpdateDocumentVente(FacturesAExporter[i].DO_Piece);

        //            }

        //            Console.WriteLine(DateTime.Now + " : Nombre de facture : " + FacturesAExporter.Count);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
        //        Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
        //    }
        //}

        private Societe getInfoSociete()
        {
            try
            {
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getInfoSociete(), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Societe(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString());
                            }
                        }
                    }
                    return null;

                }

            }

            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }

        private string getGNLClientLivraison(string intitule)
        {
            try
            {
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getGNLClientLivraison(intitule), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader[0].ToString();
                            }
                        }
                    }
                    return null;

                }

            }

            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }

        public void ExportFacture()
        {
            try
            {
                if (!Directory.Exists(logDirectoryName_export))
                {
                    Directory.CreateDirectory(logDirectoryName_export);
                }

                var logFileName_export = logDirectoryName_export + @"\" + string.Format("LOG_Export_Facture_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
                var logFile_export = File.Create(logFileName_export);
                logFileWriter_export = new StreamWriter(logFile_export);

                
                try
                {

                    logFileWriter_export.WriteLine("#####################################################################################");
                    logFileWriter_export.WriteLine("################################# Import Planifier ##################################");
                    logFileWriter_export.WriteLine("#####################################################################################");
                    logFileWriter_export.WriteLine("");

                    List<DocumentVente> FacturesAExporter = GetFacturesFromDataBase();

                    if (FacturesAExporter != null)
                    {
                        string outputFile = "";
                        var fileName = "";

                        for (int i = 0; i < FacturesAExporter.Count; i++)
                        {
                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportFacture() : Nombre de facture à exporter ===> " + i + "/" + FacturesAExporter.Count);

                            Customer customer = GetClient(FacturesAExporter[i].DO_TIERS);

                            Societe societe = getInfoSociete();
                            string prefix = "FA";
                            string identifiant = "380";

                            if (FacturesAExporter[i].DO_Piece.StartsWith("BA"))
                            {
                                outputFile = this.pathExport + @"\Fichier Exporter\Avoirs\";
                                fileName = string.Format("AVOIR-{0:HHmmss}.{0:yyyyMMdd}." + customer.CT_Num + "." + customer.CT_EDI1 + ".csv", DateTime.Now);
                                prefix = "BA";
                                identifiant = "381";
                            }
                            else
                            {
                                outputFile = this.pathExport + @"\Fichier Exporter\Factures\";
                                fileName = string.Format("INV-{0:HHmmss}.{0:yyyyMMdd}." + customer.CT_Num + "." + customer.CT_EDI1 + ".csv", DateTime.Now);

                            }

                            if (!Directory.Exists(outputFile))
                            {
                                System.IO.Directory.CreateDirectory(outputFile);
                            }


                            using (StreamWriter writer = new StreamWriter(outputFile + @"\" + fileName, false, Encoding.Default))
                            {
                                writer.WriteLine("DEMAT-AAA;v01.0;;;" + DateTime.Today.Year + addZero(DateTime.Today.Month.ToString()) + addZero(DateTime.Today.Day.ToString()) + ";;");
                                writer.WriteLine("");
                                writer.WriteLine("");



                                //string[] tab = new string[] { "", "", "" };



                                //if (FacturesAExporter[i].OriginDocumentType == "8")
                                //{
                                //    tab = GetCommandeFacture(FacturesAExporter[i].Id).Split(';');
                                //}

                                string[] docRegl = GetModeReglement(FacturesAExporter[i].DO_Piece).Split(';');

                                string modeReglement = "";
                                string DR_DATE = "";
                                string DR_TYPEREGL = "";
                                string DR_POURCENT = "";
                                string DR_MONTANT = "";

                                if (docRegl.Length != 0)
                                {
                                    modeReglement = docRegl[0];
                                    DR_DATE = docRegl[1];
                                    DR_TYPEREGL = docRegl[2];
                                    DR_POURCENT = docRegl[3];
                                    DR_MONTANT = docRegl[4];
                                }

                                string devise = "";
                                if (FacturesAExporter[i].DO_devise != "0")
                                {
                                    if (FacturesAExporter[i].DO_devise == "1")
                                    {
                                        devise = "EUR";
                                    }
                                    else
                                    {
                                        devise = getDeviseIso(FacturesAExporter[i].DO_devise);
                                    }
                                }

                                writer.WriteLine("DEMAT-HD1;v01.0;;" + FacturesAExporter[i].DO_Piece.Replace(prefix, "") + ";" + identifiant + ";9;" + ConvertDate(FacturesAExporter[i].DO_date) + ";" + ConvertDate(FacturesAExporter[i].DO_dateLivr) + ";;;;;" + modeReglement + ";;" + customer.CT_SvFormeJuri + ";;0;;" + FacturesAExporter[i].DO_COORD01 + ";;" + FacturesAExporter[i].DO_COORD01 + ";;;;;;;;;;;;;;;;;;;;;;;;" + devise + ";;;" + ConvertDate(DR_DATE) + ";;" + FacturesAExporter[i].FNT_Escompte.Replace(",", ".").Replace("00000", "") + ";;;;;;;;;;;;;");
                                writer.WriteLine("");

                                // Code GNL extraie de DO_MOTIF
                                //writer.WriteLine("DEMAT-HD2;" + customer.CT_EDI1 + ";" + customer.CT_Num + ";" + customer.CT_Adresse + ";" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + customer.CT_Pays + ";;;;;;;3700471600002;TRACE SPORT;32 RUE DE PARADIS;75010;PARIS;FR;;;;;;;;;;;;;" + (FacturesAExporter[i].DO_MOTIF.Split(';').Length == 2 ? FacturesAExporter[i].DO_MOTIF.Split(';')[0] : null) + ";" + FacturesAExporter[i].LI_Intitule + ";" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + FacturesAExporter[i].LI_PAYS + ";XXXX;500110226;ESA28425270;FR68500110226;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");

                                // Code GLN extraie de li_complement
                                // writer.WriteLine("DEMAT-HD2;" + customer.CT_EDI1 + ";" + customer.CT_Num + ";" + customer.CT_Adresse + ";" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + customer.CT_Pays + ";" + FacturesAExporter[i].LI_COMPLEMENT + ";" + FacturesAExporter[i].LI_Intitule + ";" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + FacturesAExporter[i].LI_PAYS + ";3700471600002;TRACE SPORT;32 RUE DE PARADIS;75010;PARIS;FR;;;;;;;;;;;;;" + FacturesAExporter[i].LI_COMPLEMENT + ";" + FacturesAExporter[i].LI_Intitule + ";" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + FacturesAExporter[i].LI_PAYS + ";XXXX;500110226;ESA28425270;FR68500110226;;;;;;;;;;;;;;;;;;;;;;;;;;;;3700471600002;;;;;;;;;;;;;;;;;;");
                                // DEMAT-HD2 est changé pour extraire les infos de la société la table p_dossier(version 5.2)
                                // D_commentaire c'est le GLN de la société
                                string glnLivraison = null;

                                if (FacturesAExporter[i].LI_Intitule == customer.CT_Intitule)
                                {
                                    glnLivraison = customer.CT_EDI1;
                                }
                                else
                                {
                                    glnLivraison = getGNLClientLivraison(FacturesAExporter[i].LI_Intitule);
                                }

                                writer.WriteLine("DEMAT-HD2;" + glnLivraison + ";" + FacturesAExporter[i].LI_Intitule + ";" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + (FacturesAExporter[i].LI_PAYS.ToUpper() == "FRANCE" ? "FR" : FacturesAExporter[i].LI_PAYS) + ";" + customer.CT_EDI1 + ";" + customer.CT_Num + ";" + customer.CT_Adresse + ";" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + (customer.CT_Pays.ToUpper() == "FRANCE" ? "FR" : customer.CT_Pays) + ";" + societe.D_Commentaire + ";" + societe.D_RaisonSoc + ";" + societe.D_Adresse + ";" + societe.D_CodePostal + ";" + societe.D_Ville + ";" + (societe.D_Pays.ToUpper() == "FRANCE" ? "FR" : societe.D_Pays) + ";;;;;;;;;;;;;" + FacturesAExporter[i].LI_COMPLEMENT + ";" + FacturesAExporter[i].LI_Intitule + ";" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + (FacturesAExporter[i].LI_PAYS.ToUpper() == "FRANCE" ? "FR" : FacturesAExporter[i].LI_PAYS) + ";XXXX;" + societe.D_Siret + ";ESA28425270;" + societe.D_Identifiant + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;" + societe.D_Commentaire + ";;;;;;;;;;;;;;;;;;");                      
                                writer.WriteLine("");

                                //writer.WriteLine("DEMAT-CTA;" + FacturesAExporter[i].InvoicingContact_Function + ";;" + FacturesAExporter[i].InvoicingContact_Name + " " + FacturesAExporter[i].InvoicingContact_FirstName + ";" + FacturesAExporter[i].InvoicingContact_Email + ";" + FacturesAExporter[i].InvoicingContact_Fax + ";" + FacturesAExporter[i].InvoicingContact_Phone + ";" + FacturesAExporter[i].InvoicingContact_Function + ";;" + FacturesAExporter[i].InvoicingContact_Name + " " + FacturesAExporter[i].InvoicingContact_FirstName + ";" + FacturesAExporter[i].InvoicingContact_Email + ";" + FacturesAExporter[i].InvoicingContact_Fax + ";" + FacturesAExporter[i].InvoicingContact_Phone + ";;;;;;;;;;;;;;;;;;;" + FacturesAExporter[i].DeliveryContact_Function + ";;" + FacturesAExporter[i].DeliveryContact_Name + " " + FacturesAExporter[i].DeliveryContact_FirstName + ";" + FacturesAExporter[i].DeliveryContact_Email + ";" + FacturesAExporter[i].DeliveryContact_Fax + ";" + FacturesAExporter[i].DeliveryContact_Phone + ";;;;;;;");
                                //writer.WriteLine("");

                                if (FacturesAExporter[i].DO_Piece.StartsWith("FA"))
                                {
                                    writer.WriteLine("DEMAT-CTH;1;AAI;Type de document;Facture;;;;");
                                    writer.WriteLine("");
                                }
                                else if (FacturesAExporter[i].DO_Piece.StartsWith("BA"))
                                {
                                    writer.WriteLine("DEMAT-CTH;1;AAI;Type de document;Bon d’avoir;;;;");
                                    writer.WriteLine("");
                                }

                                writer.WriteLine("DEMAT-CTH;2;AAI;N° d'accord;" + FacturesAExporter[i].ca_num + ";;;;");
                                writer.WriteLine("");

                                writer.WriteLine("DEMAT-REM;;A;;;;;;;;" + FacturesAExporter[i].FNT_Escompte + ";" + FacturesAExporter[i].do_txescompte.Replace(",", ".").Replace("00000", "") + ";;");
                                writer.WriteLine("");

                                FacturesAExporter[i].lines = getDocumentLine(FacturesAExporter[i].DO_Piece);

                                for (int j = 0; j < FacturesAExporter[i].lines.Count; j++)
                                {

                                    writer.WriteLine("DEMAT-LIN;" + FacturesAExporter[i].lines[j].DL_Ligne + ";" + FacturesAExporter[i].lines[j].AR_CODEBARRE + ";EAN;;;" + customer.CT_EDI1 + ";;;;" + FacturesAExporter[i].lines[j].DL_Design + ";;" + FacturesAExporter[i].lines[j].DL_PoidsNet.Replace(",", ".") + ";" + FacturesAExporter[i].lines[j].DL_PoidsBrut.Replace(",", ".") + ";;" + FacturesAExporter[i].lines[j].DL_Qte + ";" + FacturesAExporter[i].lines[j].DL_QteBL + ";" + FacturesAExporter[i].lines[j].EU_Qte + ";;;;" + FacturesAExporter[i].lines[j].FNT_MontantHT.Replace(",", ".") + ";" + FacturesAExporter[i].lines[j].FNT_PrixUNet.Replace(",", ".") + ";" + FacturesAExporter[i].lines[j].FNT_PrixUNet.Replace(",", ".") + ";;1;;;" + ConvertDate(FacturesAExporter[i].lines[j].DO_DateLivr.Replace("00:00:00", "")) + ";" + FacturesAExporter[i].lines[j].DL_NoColis + ";;;;;;;;;;;;;" + FacturesAExporter[i].lines[j].FNT_MontantTTC.Replace(",", ".") + ";;;;;;;;");
                                    writer.WriteLine("");

                                    if (FacturesAExporter[i].lines[j].DL_TypeTaxe1 == "0")
                                    {
                                        FacturesAExporter[i].lines[j].DL_TypeTaxe1 = "TVA/Débit";
                                    }
                                    if (FacturesAExporter[i].lines[j].DL_TypeTaxe2 == "0")
                                    {
                                        FacturesAExporter[i].lines[j].DL_TypeTaxe2 = "TVA/Débit";
                                    }
                                    if (FacturesAExporter[i].lines[j].DL_TypeTaxe3 == "0")
                                    {
                                        FacturesAExporter[i].lines[j].DL_TypeTaxe3 = "TVA/Débit";
                                    }

                                    if (FacturesAExporter[i].lines[j].DL_TypeTaxe1 == "1")
                                    {
                                        FacturesAExporter[i].lines[j].DL_TypeTaxe1 = "TVA/Encaissement";
                                    }
                                    if (FacturesAExporter[i].lines[j].DL_TypeTaxe2 == "1")
                                    {
                                        FacturesAExporter[i].lines[j].DL_TypeTaxe2 = "TVA/Encaissement";
                                    }
                                    if (FacturesAExporter[i].lines[j].DL_TypeTaxe3 == "1")
                                    {
                                        FacturesAExporter[i].lines[j].DL_TypeTaxe3 = "TVA/Encaissement";
                                    }


                                    // Calcule taxe
                                    decimal montantTaxe = (Decimal.Parse(FacturesAExporter[i].lines[j].FNT_MontantHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].lines[j].DL_Taxe1.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                                    writer.WriteLine("DEMAT-TAX;1;;" + FacturesAExporter[i].lines[j].DL_TypeTaxe1 + ";" + FacturesAExporter[i].lines[j].DL_Taxe1.Replace(",", ".") + ";;" + Math.Round(montantTaxe, 2).ToString().Replace(",", ".") + ";;;");
                                    writer.WriteLine("");

                                    //if (FacturesAExporter[i].lines[j].DL_Taxe2 != FacturesAExporter[i].lines[j].DL_Taxe1 && FacturesAExporter[i].lines[j].DL_Taxe2 != "0")
                                    if (FacturesAExporter[i].lines[j].DL_Taxe2 != "0")
                                    {
                                        montantTaxe = (Decimal.Parse(FacturesAExporter[i].lines[j].FNT_MontantHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].lines[j].DL_Taxe2.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                                        writer.WriteLine("DEMAT-TAX;2;;" + FacturesAExporter[i].lines[j].DL_TypeTaxe2 + ";" + FacturesAExporter[i].lines[j].DL_Taxe2.Replace(",", ".") + ";;" + Math.Round(montantTaxe, 2).ToString().Replace(",", ".") + ";;;");
                                        writer.WriteLine("");
                                    }

                                    //if ((FacturesAExporter[i].lines[j].DL_Taxe3 != FacturesAExporter[i].lines[j].DL_Taxe1) && (FacturesAExporter[i].lines[j].DL_Taxe3 != FacturesAExporter[i].lines[j].DL_Taxe2) && FacturesAExporter[i].lines[j].DL_Taxe3 != "0")
                                    if (FacturesAExporter[i].lines[j].DL_Taxe3 != "0")
                                    {
                                        montantTaxe = (Decimal.Parse(FacturesAExporter[i].lines[j].FNT_MontantHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].lines[j].DL_Taxe3.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                                        writer.WriteLine("DEMAT-TAX;3;;" + FacturesAExporter[i].lines[j].DL_TypeTaxe3 + ";" + FacturesAExporter[i].lines[j].DL_Taxe3.Replace(",", ".") + ";;" + Math.Round(montantTaxe, 2).ToString().Replace(",", ".") + ";;;");
                                        writer.WriteLine("");
                                    }

                                    //---- Remise ----

                                    string MontantRemise = "";
                                    string PourcentageRemise = "";

                                    if (FacturesAExporter[i].lines[j].DL_Remise01REM_Type == "0")
                                    {
                                        MontantRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur.Replace(",", ".");
                                        PourcentageRemise = "";
                                    }

                                    if (FacturesAExporter[i].lines[j].DL_Remise01REM_Type == "1")
                                    {
                                        MontantRemise = "";
                                        PourcentageRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur;
                                    }

                                    //if (FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur != "0")
                                    //{
                                        writer.WriteLine("DEMAT-DED;;A;;;;;;;" + FacturesAExporter[i].lines[j].DL_Remise01REM_Type + ";" + MontantRemise + ";" + PourcentageRemise + ";;");
                                        writer.WriteLine("");
                                    //}


                                    if (FacturesAExporter[i].lines[j].DL_Remise03REM_Type == "0")
                                    {
                                        MontantRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur;
                                        PourcentageRemise = "";
                                    }

                                    if (FacturesAExporter[i].lines[j].DL_Remise03REM_Type == "1")
                                    {
                                        MontantRemise = "";
                                        PourcentageRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur;
                                    }

                                    if ((FacturesAExporter[i].lines[j].DL_Remise03REM_Valeur != "0") && (FacturesAExporter[i].lines[j].DL_Remise03REM_Valeur != FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur))
                                    {
                                        writer.WriteLine("DEMAT-DED;;A;;;;;;;;" + MontantRemise + ";" + PourcentageRemise + ";;");
                                        writer.WriteLine("");
                                    }

                                }

                                //  Les lignes des taxes


                                if (FacturesAExporter[i].DO_TypeTaxe1 == "0")
                                {
                                    FacturesAExporter[i].DO_TypeTaxe1 = "TVA/Débit";
                                }
                                if (FacturesAExporter[i].DO_TypeTaxe2 == "0")
                                {
                                    FacturesAExporter[i].DO_TypeTaxe2 = "TVA/Débit";
                                }
                                if (FacturesAExporter[i].DO_TypeTaxe3 == "0")
                                {
                                    FacturesAExporter[i].DO_TypeTaxe3 = "TVA/Débit";
                                }

                                if (FacturesAExporter[i].DO_TypeTaxe1 == "1")
                                {
                                    FacturesAExporter[i].DO_TypeTaxe1 = "TVA/Encaissement";
                                }
                                if (FacturesAExporter[i].DO_TypeTaxe2 == "1")
                                {
                                    FacturesAExporter[i].DO_TypeTaxe2 = "TVA/Encaissement";
                                }
                                if (FacturesAExporter[i].DO_TypeTaxe3 == "1")
                                {
                                    FacturesAExporter[i].DO_TypeTaxe3 = "TVA/Encaissement";
                                }

                                // Calcule taxe
                                decimal montantTaxe1 = (Decimal.Parse(FacturesAExporter[i].FNT_TotalHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].DO_taxe1.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                                writer.WriteLine("DEMAT-TTX;1;;" + FacturesAExporter[i].DO_TypeTaxe1.Replace(",", ".") + ";" + FacturesAExporter[i].DO_taxe1.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].FNT_TotalHT.Replace(",", ".").Replace("00000", "") + ";" + Math.Round(montantTaxe1, 2).ToString().Replace(",", ".") + ";;");
                                writer.WriteLine("");

                                //if (FacturesAExporter[i].DO_taxe2 != FacturesAExporter[i].DO_taxe1 && FacturesAExporter[i].DO_taxe2 != "0")

                                if (FacturesAExporter[i].DO_taxe2 != "0")
                                {
                                    // Calcule taxe
                                    montantTaxe1 = (Decimal.Parse(FacturesAExporter[i].FNT_TotalHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].DO_taxe3.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                                    writer.WriteLine("DEMAT-TTX;2;;" + FacturesAExporter[i].DO_TypeTaxe2.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].DO_taxe1.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].FNT_TotalHT.Replace(",", ".").Replace("00000", "") + ";" + Math.Round(montantTaxe1, 2).ToString().Replace(",", ".") + ";;");
                                    writer.WriteLine("");
                                }

                                if (FacturesAExporter[i].DO_taxe3 != "0")
                                // if ((FacturesAExporter[i].DO_taxe3 != FacturesAExporter[i].DO_taxe1) && (FacturesAExporter[i].DO_taxe3 != FacturesAExporter[i].DO_taxe2) && FacturesAExporter[i].DO_taxe3 != "0")
                                {
                                    // Calcule taxe
                                    montantTaxe1 = (Decimal.Parse(FacturesAExporter[i].FNT_TotalHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].DO_taxe3.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                                    writer.WriteLine("DEMAT-TTX;3;;" + FacturesAExporter[i].DO_TypeTaxe3.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].DO_taxe1.Replace(",", ".") + ";" + FacturesAExporter[i].FNT_TotalHT.Replace(",", ".") + ";" + Math.Round(montantTaxe1, 2).ToString().Replace(",", ".") + ";;");
                                    writer.WriteLine("");
                                }


                                writer.WriteLine("DEMAT-END;;;" + FacturesAExporter[i].DO_Piece.Replace(prefix, "") + ";" + FacturesAExporter[i].FNT_TotalHT.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].FNT_TotalTTC.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].FNT_MontantTotalTaxes.Replace(",", ".") + ";;;;" + FacturesAExporter[i].FNT_Escompte.Replace(",", ".") + ";;" + FacturesAExporter[i].FNT_NetAPayer.Replace(",", ".") + ";;;;");
                                writer.WriteLine("");
                                writer.WriteLine("");


                                writer.WriteLine("DEMAT-ZZZ;v01.0;;;;");


                            }

                            UpdateDocumentVente(FacturesAExporter[i].DO_Piece);

                        }

                        Console.WriteLine(DateTime.Now + " : Nombre de facture : " + FacturesAExporter.Count);
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportFacture() : Nombre de facture : " + FacturesAExporter.Count);

                }
                }
                catch (Exception ex)
                {
                    //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                    Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    
                    logFileWriter_export.WriteLine(DateTime.Now + "********************************* Exception *********************************");
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : Message :: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : Export annullé");
                    logFileWriter_export.Close();
                }
            }

            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                logFileWriter_export.WriteLine(DateTime.Now + "********************************* Exception *********************************");
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : Message :: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : Export annullé");
                logFileWriter_export.Close();
            }
            logFileWriter_export.Close();
        }

        private string GetModeReglement(string do_piece)
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                //List<DocumentVente> listDocumentVente = new List<DocumentVente>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getModeReglement(do_piece), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader[0].ToString() + ";" + reader[1].ToString() + ";" + reader[2].ToString() + ";" + reader[3].ToString() + ";" + reader[4].ToString();
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }

        private string getDeviseIso(string code)
        {
            try
            {
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getDeviseIso(code), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader[0].ToString();
                            }
                        }
                    }
                    return null;

                }

            }

            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }

    }
}
