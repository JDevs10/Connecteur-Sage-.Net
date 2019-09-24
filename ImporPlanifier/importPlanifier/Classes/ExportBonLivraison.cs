using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using importPlanifier.Helpers;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace importPlanifier.Classes
{
    class ExportBonLivraison
    {
        //private List<DocumentVente> BonLivrasonAExporter;
        //private Customer customer = new Customer();
        private string pathExport;

        public ExportBonLivraison(string path)
        {
            this.pathExport = path;
        }

        private List<DocumentVente> GetBonLivraisonFromDataBase()
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVente> listDocumentVente = new List<DocumentVente>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {
                    DocumentVente documentVente;
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVente(3), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                documentVente = new DocumentVente(reader[0].ToString(), reader[1].ToString(),
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
        public void ExportBonLivraisonAction()
        {
            try
            {
                 List<DocumentVente> BonLivrasonAExporter = GetBonLivraisonFromDataBase();

                 if (BonLivrasonAExporter != null)
                 {
                     string outputFile = this.pathExport + @"\Fichier Exporter\Bons de Livraisons\";

                     if (!Directory.Exists(outputFile))
                     {
                         System.IO.Directory.CreateDirectory(outputFile);
                     }

                     for (int i = 0; i < BonLivrasonAExporter.Count; i++)
                     {
                         Customer customer = GetClient(BonLivrasonAExporter[i].DO_TIERS);

                         var fileName = string.Format("BonLivraison{0:yyyyMMdd}." + customer.CT_Num + "." + customer.CT_EDI1 + ".csv", DateTime.Now);

                         using (StreamWriter writer = new StreamWriter(outputFile + @"\" + fileName, false, Encoding.UTF8))
                         {
                             //writer.WriteLine("DEMAT-AAA;v01.0;;;" + DateTime.Today.Year + addZero(DateTime.Today.Month.ToString()) + addZero(DateTime.Today.Day.ToString()) + ";;");
                             //writer.WriteLine("");
                             //writer.WriteLine("");

                             writer.WriteLine("DESHDR;v01.0;;" + BonLivrasonAExporter[i].DO_Piece.Replace("BL", "") + ";" + customer.CT_EDI1 + ";9;;9;" + customer.CT_EDI1 + ";9;" + customer.CT_EDI1 + ";9;;9;;9;;9;;" + ConvertDate(BonLivrasonAExporter[i].DO_dateLivr) + ";;;;;" + BonLivrasonAExporter[i].LI_ADRESSE + ";;;;;;;;;;;;;;9;");
                             writer.WriteLine("");

                             writer.WriteLine("DESHD2;;;;" + customer.CT_Adresse + ";;" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + customer.CT_Pays + ";" + customer.CT_Intitule + ";" + customer.CT_Telephone + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                             writer.WriteLine("");

                             //if (BonLivrasonAExporter[i].IntrastatTransportMode != "")
                             //{ // Return mode de transport                           
                             //    BonLivrasonAExporter[i].IntrastatTransportMode = GetModeTransport(BonLivrasonAExporter[i].IntrastatTransportMode);
                             //}

                             writer.WriteLine("DESTRP;;;;;;;;;;");
                             writer.WriteLine("");

                             writer.WriteLine("DESLOG;;;;" + BonLivrasonAExporter[i].FNT_PoidsBrut.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].FNT_PoidsNet.Replace(",", ".") + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                             writer.WriteLine("");


                             BonLivrasonAExporter[i].lines = getDocumentLine(BonLivrasonAExporter[i].DO_Piece);

                             for (int j = 0; j < BonLivrasonAExporter[i].lines.Count; j++)
                             {

                                 writer.WriteLine("DESLIN;" + BonLivrasonAExporter[i].lines[j].DL_Ligne + ";;" + BonLivrasonAExporter[i].lines[j].AR_CODEBARRE + ";;;;;;;" + BonLivrasonAExporter[i].lines[j].DL_Design + ";;;" + BonLivrasonAExporter[i].lines[j].DL_Qte + ";;" + BonLivrasonAExporter[i].lines[j].EU_Qte + ";;;;;;;;;;;" + ConvertDate(BonLivrasonAExporter[i].lines[j].DO_DateLivr) + ";;;" + BonLivrasonAExporter[i].lines[j].FNT_PrixUNet.Replace(",", ".") + ";;;;;;" + BonLivrasonAExporter[i].lines[j].FNT_MontantHT.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].lines[j].DL_NoColis + ";;;;;;;;;;;");
                                 writer.WriteLine("");


                             }


                             writer.WriteLine("DESEND;" + BonLivrasonAExporter[i].lines.Count + ";;;" + BonLivrasonAExporter[i].FNT_TotalHTNet.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_TotalHT.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_PoidsBrut.Replace(",", ".") + ";;;;;");
                             writer.WriteLine("");
                             writer.WriteLine("");




                         }

                         UpdateDocumentVente(BonLivrasonAExporter[i].DO_Piece);

                     }


                     Console.WriteLine(DateTime.Now + " : Nombre bon de livraison : " + BonLivrasonAExporter.Count);
                 }



            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
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
    }
}
