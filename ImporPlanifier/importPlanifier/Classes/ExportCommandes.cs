using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using importPlanifier.Helpers;
using System.Data.Odbc;
using System.Data;
using System.IO;

namespace importPlanifier.Classes
{
    class ExportCommandes
    {
        #region Champs privés
        /// <summary>
        /// commande à exporter
        /// </summary>
        //private Order CommandeAExporter;

        private string pathExport;

        private string logDirectoryName_export = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Export" + @"\" + "COMMANDE";
        private StreamWriter logFileWriter = null;

        #endregion



        public ExportCommandes(string path)
        {
            this.pathExport = path;
        }

        #region Intéractions avec l'application

        private List<Order> GetCommandesFromDataBase()
        {
            try
            {
            //DocumentVente Facture = new DocumentVente();
            List<Order> listCommande = new List<Order>();
             using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
               
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListCommandes(false), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                           while(reader.Read())
                            {
                                Order order = new Order(reader[0].ToString(), reader[1].ToString(), 
                                    reader[2].ToString().Replace(", ",",")+"."+reader[3].ToString()+"."+reader[6].ToString()+"."+reader[7].ToString(),
                                    reader[8].ToString(), reader[9].ToString().Replace("00:00:00",""),
                                    reader[10].ToString(), reader[11].ToString(),
                                    reader[12].ToString(), reader[13].ToString(), reader[15].ToString(),
                                    (reader[14].ToString().Split(';').Length == 2 ? reader[14].ToString().Split(';')[0] : null),
                                    (reader[14].ToString().Split(';').Length == 2 ? reader[14].ToString().Split(';')[1] : null),
                                    reader[16].ToString()
                                    );
                                listCommande.Add(order);
                            }
                        }
                    }
                    return listCommande;

            }

                                 }

                catch (Exception e)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    Console.WriteLine(e.Message);
                    return null;
                }
        }

       




        /// <summary>
        /// Génération du fichier d'import, lancement de l'application et import des commandes
        /// </summary>
        public void ExportCommande()
        {
            Classes.Path path = new Path();
            path.Load();
            string exportPath = path.path + @"\Export_Veolog";

            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }
            Order CommandeAExporter = null;

            if (!Directory.Exists(logDirectoryName_export))
            {
                Directory.CreateDirectory(logDirectoryName_export);
            }

            var logFileName_export = logDirectoryName_export + @"\" + string.Format("LOG_Export_Commande_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile_export = File.Create(logFileName_export);

            //Write in the log file 
            logFileWriter = new StreamWriter(logFile_export);
            //logFileWriter.Write(string.Format("{0:HH:mm:ss}", DateTime.Now) + " \r\n");
            logFileWriter.WriteLine("#####################################################################################");
            logFileWriter.WriteLine("################################ ConnecteurSage Sage ################################");
            logFileWriter.WriteLine("#####################################################################################");
            logFileWriter.WriteLine("");

            //Export all CMDs with status 1
            //Get Doc Entette DO_Statut
            /*  Get a list of 100 orders for Veolog with a DO_Statut == 1 
                Export the 'Bon de Livraison' BC as .csv file
                send the csv file to Velog  */
            string[,] lits_of_stock = new string[100, 2];
            int countLimit = 0;

            using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
            {
                try
                {
                    connexion.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.getCommandeStatut(true), connexion);

                    //Console.WriteLine(DateTime.Now + " | ExportCommande() : SQL ===> " + QueryHelper.getCommandeStatut());
                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : SQL ===> " + QueryHelper.getCommandeStatut(true));

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // reads lines/rows from the query
                        {
                            //Console.WriteLine(DateTime.Now + " | ExportCommande() : reader[0].ToString() == "+ reader[1].ToString());
                            if (reader[1].ToString().Equals("1"))
                            {
                                if (countLimit < 100)
                                {
                                    lits_of_stock[countLimit, 0] = reader[0].ToString(); // cbMarq
                                    lits_of_stock[countLimit, 1] = reader[1].ToString(); // DO_Statut
                                    Console.WriteLine(DateTime.Now + " | ExportCommande() : cbMarq = " + reader[0].ToString() + " DO_Statut = " + reader[1].ToString());
                                    countLimit++;
                                }
                            }
                        }
                    }
                    Console.WriteLine(DateTime.Now + " | ExportCommande() : Connexion close.");
                    connexion.Close();

                }
                catch (OdbcException ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine(DateTime.Now + " : ExportCommande() |  ********************** OdbcException *********************");
                    Console.WriteLine(DateTime.Now + " : ExportCommande() |  SQL ===> " + QueryHelper.getCommandeStatut(true));
                    Console.WriteLine(DateTime.Now + " : ExportCommande() |  Message : " + ex.Message + ".");
                    Console.WriteLine(DateTime.Now + " : ExportCommande() |  Scan annulée");
                    return;
                }
            }

            //Console.WriteLine("OK1 countLimit:" + countLimit + " ");

            for (int index=0; index< countLimit; index++)
            {
                //Console.WriteLine("OK2 index:" + index+ " countLimit:" + countLimit+" ");
                //CommandeAExporter
                using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
                {
                    //Console.WriteLine("OK3");
                    try
                    {
                        connexion.Open();
                        OdbcCommand command = new OdbcCommand(QueryHelper.getCoommandeById(true, lits_of_stock[index,0]), connexion);

                        Console.WriteLine(DateTime.Now + " | ExportCommande() : SQL ===> " + QueryHelper.getCoommandeById(true, lits_of_stock[index, 0]));
                        logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : SQL ===> " + QueryHelper.getCoommandeById(true, lits_of_stock[index, 0]));

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) // reads lines/rows from the query
                            {
                                CommandeAExporter = new Order(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString());
                            }
                        }
                        if (CommandeAExporter != null)
                        {
                            if (!CommandeAExporter.NomClient.Equals("") && !CommandeAExporter.NomClient.Equals(" "))
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Export Commande du client \"" + CommandeAExporter.NomClient + "\"");
                            }
                            else
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Export Commande du client \"...\"");
                            }
                            try
                            {
                                if (CommandeAExporter.deviseCommande == "0")
                                {
                                    CommandeAExporter.deviseCommande = "1";
                                }
                                if (CommandeAExporter.deviseCommande != "")
                                {
                                    CommandeAExporter.deviseCommande = getDeviseIso(CommandeAExporter.deviseCommande);
                                }
                                if (CommandeAExporter.DO_MOTIF == "")
                                {
                                    CommandeAExporter.DO_MOTIF = CommandeAExporter.NumCommande;
                                }
                                if (CommandeAExporter.DO_MOTIF == "")
                                {
                                    CommandeAExporter.DO_MOTIF = "";
                                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : N° de commande non enregistrer, valeur '" + CommandeAExporter.DO_MOTIF + "'.");
                                }
                                else
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : N° de commande non enregistrer, valuer '" + CommandeAExporter.DO_MOTIF + "'.");
                                }
                                if (CommandeAExporter.codeClient == "")
                                {
                                    CommandeAExporter.codeClient = "";
                                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Code GNL client n'est pas enregistrer, valeur '" + CommandeAExporter.codeClient + "'.");
                                }
                                else
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Code GNL client n'est pas enregistrer, valeur '" + CommandeAExporter.codeClient + "'.");
                                }
                                if (!IsNumeric(CommandeAExporter.DO_MOTIF) && CommandeAExporter.DO_MOTIF != "")
                                {
                                    CommandeAExporter.DO_MOTIF = "";
                                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : N° de commande est mal enregistrer, valeur '" + CommandeAExporter.DO_MOTIF + "'.");
                                }
                                else
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : N° de commande est mal enregistrer, valeur '" + CommandeAExporter.DO_MOTIF + "'.");
                                }
                                if (!IsNumeric(CommandeAExporter.codeClient) && CommandeAExporter.codeClient != "")
                                {
                                    CommandeAExporter.DO_MOTIF = "";
                                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Code GNL client est mal enregistrer.");
                                }
                                else
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : N° de commande est mal enregistrer, valeur '" + CommandeAExporter.DO_MOTIF + "'.");
                                }
                                var fileName = string.Format("EDI_ORDERS." + CommandeAExporter.codeClient + "." + CommandeAExporter.NumCommande + "." + ConvertDate(CommandeAExporter.DateCommande) + "." + CommandeAExporter.adresseLivraison + ".{0:yyyyMMddhhmmss}.csv", DateTime.Now);

                                fileName = fileName.Replace("...", ".");
                                /*
                                DialogResult resultDialog5 = MessageBox.Show("Voulez-vous générer l'exportation du fichier en format Veolog?",
                                                            "Information !",
                                                            MessageBoxButtons.YesNo,
                                                            MessageBoxIcon.Question,
                                                            MessageBoxDefaultButton.Button2);
                                var veolog_format = false;

                                if (resultDialog5 == DialogResult.No)
                                {
                                    veolog_format = false;
                                    fileName = fileName.Replace("..", ".");
                                }

                                if (resultDialog5 == DialogResult.Yes)
                                {
                                    veolog_format = true;
                                    fileName = string.Format("orders_{0:yyyyMMddhhmmss}.csv", DateTime.Now);
                                }
                                */

                                //Verifier le format utilise depuis le fichier de config
                                ConfigurationExport export = new ConfigurationExport();
                                export.Load();

                                var veolog_format = ((export.exportBonsCommandes_Format == "Velog") ? true : false);
                                if (veolog_format)
                                {
                                    veolog_format = true;
                                    fileName = string.Format("orders_{0:yyyyMMddhhmmss}.csv", DateTime.Now);
                                }
                                else
                                {
                                    veolog_format = false;
                                    fileName = fileName.Replace("..", ".");
                                }

                                bool veolog_file_check = false;
                                using (StreamWriter writer = new StreamWriter(exportPath + @"\" + fileName, false, Encoding.Default))
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Ecrire le fichier dans : " + exportPath + @"\" + fileName.Replace("..", "."));

                                    if (veolog_format)
                                    {
                                        //format Veolog
                                        writer.WriteLine("E;" + CommandeAExporter.NumCommande + ";;;Veolog;35 Avenue de Bethunes;ZI de Bethunes;;95310;Saint Ouen l'Aumone;FR;766666666;a.bertolino@veolog.fr;20180917;1200;ENLEVEMENT;;;COMMANDE DE TEST"); // E line

                                        CommandeAExporter.Lines = getLigneCommande(CommandeAExporter.NumCommande); // Maybe thisssss

                                        int qteTotal = 0;
                                        string[] declarerpourrien = new string[2];
                                        for (int i = 0; i < CommandeAExporter.Lines.Count; i++)
                                        {
                                            if (!IsNumeric(CommandeAExporter.Lines[i].codeAcheteur))
                                            {
                                                CommandeAExporter.Lines[i].codeAcheteur = "";
                                            }

                                            if (!IsNumeric(CommandeAExporter.Lines[i].codeFournis))
                                            {
                                                CommandeAExporter.Lines[i].codeFournis = "";
                                            }

                                            declarerpourrien = CommandeAExporter.Lines[i].Quantite.Split(',');
                                            qteTotal = qteTotal + Convert.ToInt16(declarerpourrien[0]);

                                            writer.WriteLine("L;;" + ((CommandeAExporter.Lines[i].codeArticle.Length > 30) ? CommandeAExporter.Lines[i].codeArticle.Substring(0, 30) : CommandeAExporter.Lines[i].codeArticle) + ";;" + declarerpourrien[0] + ";"); // L line

                                        }
                                        writer.WriteLine("F;" + CommandeAExporter.Lines.Count + ";" + qteTotal + ";"); // F line
                                        writer.Close();

                                        //Vérifier si le fichier a bien été créé et écrit
                                        if (File.Exists(exportPath + @"\" + fileName))
                                        {
                                            if (new FileInfo(exportPath + @"\" + fileName).Length > 0)
                                            {
                                                veolog_file_check = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Format Fichier plat
                                        writer.WriteLine("ORDERS;" + CommandeAExporter.DO_MOTIF + ";" + CommandeAExporter.codeClient + ";" + CommandeAExporter.codeAcheteur + ";" + CommandeAExporter.codeFournisseur + ";;;" + CommandeAExporter.nom_contact + "." + CommandeAExporter.adresseLivraison.Replace("..", ".").Replace("...", ".") + ";" + CommandeAExporter.deviseCommande + ";;");
                                        if (CommandeAExporter.DateCommande != "")
                                        {
                                            CommandeAExporter.DateCommande = ConvertDate(CommandeAExporter.DateCommande);
                                        }

                                        //if (CommandeAExporter.DateCommande != " ")
                                        //{
                                        //    CommandeAExporter.conditionLivraison = "";
                                        //}

                                        writer.WriteLine("ORDHD1;" + CommandeAExporter.DateCommande + ";" + CommandeAExporter.conditionLivraison + ";;");

                                        CommandeAExporter.Lines = getLigneCommande(CommandeAExporter.NumCommande);

                                        for (int i = 0; i < CommandeAExporter.Lines.Count; i++)
                                        {
                                            if (!IsNumeric(CommandeAExporter.Lines[i].codeAcheteur))
                                            {
                                                CommandeAExporter.Lines[i].codeAcheteur = "";
                                            }

                                            if (!IsNumeric(CommandeAExporter.Lines[i].codeFournis))
                                            {
                                                CommandeAExporter.Lines[i].codeFournis = "";
                                            }

                                            writer.WriteLine("ORDLIN;" + CommandeAExporter.Lines[i].NumLigne + ";" + CommandeAExporter.Lines[i].codeArticle + ";GS1;" + CommandeAExporter.Lines[i].codeAcheteur + ";" + CommandeAExporter.Lines[i].codeFournis + ";;A;" + CommandeAExporter.Lines[i].descriptionArticle + ";" + CommandeAExporter.Lines[i].Quantite.Replace(",", ".") + ";LM;" + CommandeAExporter.Lines[i].MontantLigne.Replace(",", ".") + ";;;" + CommandeAExporter.Lines[i].PrixNetHT.Replace(",", ".") + ";;;LM;;;;" + ConvertDate(CommandeAExporter.Lines[i].DateLivraison) + ";");
                                        }
                                        writer.WriteLine("ORDEND;" + CommandeAExporter.MontantTotal.ToString().Replace(",", ".") + ";");

                                        writer.Close();
                                    }
                                }

                                //changer le statut de la commande
                                if (veolog_file_check)
                                {
                                    try
                                    {
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | ExportFacture() : Changer le statut de la commande \"" + CommandeAExporter.NumCommande + "\".");

                                        using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL())
                                        {
                                            connection.Open();

                                            logFileWriter.WriteLine(DateTime.Now + " | ExportFacture() : SQL ===> " + QueryHelper.changeOrderStatut(true, CommandeAExporter.NumCommande));
                                            OdbcCommand command1 = new OdbcCommand(QueryHelper.changeOrderStatut(true, CommandeAExporter.NumCommande), connection);
                                            {
                                                using (IDataReader reader = command1.ExecuteReader())
                                                {
                                                    while (reader.Read())
                                                    {
                                                        //Statut Update
                                                    }
                                                }
                                            }
                                            connection.Close();
                                            logFileWriter.WriteLine(DateTime.Now + " SQL Connexion Close. ");
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                                        return;
                                    }
                                }

                                logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Commande exportée avec succés.");

                            jamp:;

                                //logFileWriter.Close();
                                //Close();
                            }
                            catch (Exception ex)
                            {
                                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                                logFileWriter.WriteLine(DateTime.Now + " | ExportStock() : ERREUR :: " + ex.Message);
                                //logFileWriter.Close();
                                //Close();
                            }
                        }

                        Console.WriteLine(DateTime.Now + " | ExportCommande() : Connexion close.");
                        connexion.Close();
                    }
                    catch (OdbcException ex)
                    {
                        Console.WriteLine("");
                        Console.WriteLine(DateTime.Now + " : ExportCommande() |  ********************** OdbcException *********************");
                        Console.WriteLine(DateTime.Now + " : ExportCommande() |  SQL ===> " + QueryHelper.getCommandeStatut(true));
                        Console.WriteLine(DateTime.Now + " : ExportCommande() |  Message : " + ex.Message + ".");
                        Console.WriteLine(DateTime.Now + " : ExportCommande() |  Scan annulée");
                        return;
                    }
                }

            }
        }
        #endregion

        #region Méthodes diverses
        /// <summary>
        /// Chargement de la fenêtre
        /// </summary>
        /// <param name="e">paramètres de l'évènement</param>
       

        public static string ConvertDate(string date)
        {
            if (date.Length == 11 || date.Length == 19)
            {
                return date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2);
            }
            return date;
        }


        #endregion




        private string getDeviseIso(string code)
        {
            try
            {
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getDeviseIso(false, code), connection);
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

        private List<OrderLine> getLigneCommande(string code)
        {
            try
            {
                using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL())
                {
                    List<OrderLine> lines = new List<OrderLine>();

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    Console.WriteLine("SQL: "+QueryHelper.getListLignesCommandes(true, code));
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListLignesCommandes(true, code), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lines.Add(new OrderLine(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                            }

                            return lines;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }

        public static Boolean IsNumeric(string Nombre)
        {
            try
            {
                long.Parse(Nombre);
                return true;
            }
            catch
            {
                return false;
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
                    OdbcCommand command = new OdbcCommand(QueryHelper.updateDocumentdeVente(false, do_piece), connection);
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
