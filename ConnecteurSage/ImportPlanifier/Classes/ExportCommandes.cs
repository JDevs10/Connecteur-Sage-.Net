using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using importPlanifier.Helpers;
using System.Data.Odbc;
using System.Data;
using System.IO;
using System.Globalization;
using ImportPlanifier.Classes;

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
        private string docRefMail = "";
        private string logFileName_export;
        public string logDirectoryName_export = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Export" + @"\" + "COMMANDE";
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
        public List<CustomMailRecapLines> ExportCommande(List<CustomMailRecapLines> recapLinesList_new)
        {
            Classes.Path path = new Path();
            path.Load();
            string exportPath = path.path;
            string exportTo = "";

            Order CommandeAExporter = null;

            if (!Directory.Exists(logDirectoryName_export))
            {
                Directory.CreateDirectory(logDirectoryName_export);
            }

            logFileName_export = logDirectoryName_export + @"\" + string.Format("LOG_Export_Commande_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
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

                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : SQL ===> " + QueryHelper.getCommandeStatut(true));

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // reads lines/rows from the query
                        {
                            //if (reader[1].ToString().Equals("1"))
                            //{
                                if (countLimit < 100)
                                {
                                    lits_of_stock[countLimit, 0] = reader[0].ToString(); // cbMarq
                                    lits_of_stock[countLimit, 1] = reader[1].ToString(); // DO_Statut
                                    //Console.WriteLine(DateTime.Now + " | ExportCommande() : cbMarq = " + reader[0].ToString() + " DO_Statut = " + reader[1].ToString());
                                    countLimit++;
                                }
                            //}
                        }
                    }
                    connexion.Close();

                }
                catch (OdbcException ex)
                {
                    logFileWriter.WriteLine(DateTime.Now + " : ExportCommande() |  ********************** OdbcException *********************");
                    logFileWriter.WriteLine(DateTime.Now + " : ExportCommande() |  SQL ===> " + QueryHelper.getCommandeStatut(true));
                    logFileWriter.WriteLine(DateTime.Now + " : ExportCommande() |  Message : " + ex.Message + ".");
                    logFileWriter.WriteLine(DateTime.Now + " : ExportCommande() |  Scan annulée");
                    logFileWriter.Flush();
                    logFileWriter.Close();
                    recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export de la commande est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
                    return recapLinesList_new;
                }
            }

            logFileWriter.Flush();
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
                        //string deliveryClientName = "";
                        connexion.Open();

                        OdbcCommand command = new OdbcCommand(QueryHelper.getCoommandeById(true, lits_of_stock[index, 0]), connexion);

                        //Console.WriteLine(DateTime.Now + " | ExportCommande() : SQL ===> " + QueryHelper.getCoommandeById(true, lits_of_stock[index, 0]));
                            
                        logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : SQL ===> " + QueryHelper.getCoommandeById(true, lits_of_stock[index, 0]));

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) // reads lines/rows from the query
                            {
                            // DO_Piece, cli.CT_Num, Adresse, cmd.DO_DEVISE, cmd.DO_Date, cmd.DO_DateLivr, cmd.DO_Condition, cmd.DO_TotalHT, cli.CT_Intitule, cmd.DO_Motif, cli.CT_EdiCode, cmd.N_CATCOMPTA, cmd.DO_MOTIF, liv.LI_Contact, cli.N_Expedition, cli.CT_Telephone, cli.CT_EMail, cli.CT_Commentaire

                            CommandeAExporter = new Order(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString());

                            CommandeAExporter.telephone = reader[13].ToString();
                            CommandeAExporter.email = reader[14].ToString();
                            CommandeAExporter.commentaires = "";
                            CommandeAExporter.Transporteur = "";
                            }

                        }
                        if (CommandeAExporter != null)
                        {
                            docRefMail = CommandeAExporter.NumCommande;
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


                                //Verifier le format utilise depuis le fichier de config
                                ConfigurationExport export = new ConfigurationExport();
                                export.Load();

                                bool veolog_format = (export.exportBonsCommandes_Format == "Véolog" ? true : false);
                                Console.WriteLine("veolog_format : "+ veolog_format);
                                if (veolog_format)
                                {
                                    exportTo = @"Export\Veolog_Commande";
                                    if (!exportPath.Contains("Export_Veolog"))
                                    {
                                        exportPath = exportPath + @"\Export_Veolog";
                                    }

                                    if (!Directory.Exists(exportPath))
                                    {
                                        Directory.CreateDirectory(exportPath);
                                    }

                                    ConfigurationDNS dns = new ConfigurationDNS();
                                    dns.LoadSQL();
                                        
                                    veolog_format = true;
                                    fileName = string.Format("orders_"+dns.Prefix+"_{0:yyyyMMdd}_"+CommandeAExporter.NumCommande+".csv", DateTime.Now);
                                }
                                else
                                {
                                    exportTo = @"Export\Plat_Commande";
                                    if (!exportPath.Contains("Export_Plat"))
                                    {
                                        exportPath = exportPath + @"\Export_Plat";
                                    }

                                    if (!Directory.Exists(exportPath))
                                    {
                                        Directory.CreateDirectory(exportPath);
                                    }

                                    veolog_format = false;
                                    fileName = fileName.Replace("..", ".");
                                }

                                bool veolog_file_check = false;
                                using (StreamWriter orderFileWriter = new StreamWriter(exportPath + @"\" + fileName, false, Encoding.Default))
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Ecrire le fichier dans : " + exportPath + @"\" + fileName);

                                //Console.WriteLine("Export File Name: " + exportPath + @"\" + fileName);

                                if (veolog_format)
                                    {
                                    //format Veolog 
                                    string[] adresse = new string[5];
                                    string[] date_time_delivery = new string[2];

                                    adresse = CommandeAExporter.adresseLivraison.Split(',');
                                    date_time_delivery = CommandeAExporter.DateLivraison.Split(' ');

                                    //Split the adresse
                                    CommandeAExporter.adresse = adresse[0];
                                    CommandeAExporter.adresse_2 = adresse[1];
                                    CommandeAExporter.codepostale = adresse[2];
                                    CommandeAExporter.ville = adresse[3];

                                    // Get the country
                                    CommandeAExporter.pays = adresse[4];

                                    //Get Country ISO
                                    CountryFormatISO iso = new CountryFormatISO();
                                    string[,] country_iso = iso.getAllStaticCountryISOCode();

                                    if (CommandeAExporter.pays == "")
                                    {
                                        CommandeAExporter.pays = "France";
                                    }

                                    for (int i = 0; i < country_iso.GetLength(0); i++)
                                    {
                                        if (country_iso[i, 0].ToUpper().Equals(CommandeAExporter.pays.ToUpper()))
                                        {
                                            CommandeAExporter.pays = country_iso[i, 1];
                                        }
                                    }


                                    //Split the DateTime
                                    DateTime date_delivery = Convert.ToDateTime(CommandeAExporter.DateLivraison);

                                    //CommandeAExporter.DateCommande = date_time[0].Replace("/", "");
                                    //CommandeAExporter.DateLivraison = date_delivery.Year + "" + date_delivery.Month + "" + date_delivery.Day + "";
                                    CommandeAExporter.DateLivraison = string.Format("{0:yyyyMMdd}", date_delivery);

                                    string[] time_delivery = date_time_delivery[1].Split(':');
                                    CommandeAExporter.HeureLivraison = time_delivery[0] + "" + time_delivery[1];

                                    orderFileWriter.WriteLine("E;" + CommandeAExporter.NumCommande + ";"+CommandeAExporter.codeClient+";;"+ CommandeAExporter.NomClient + ";"+ CommandeAExporter.adresse + ";"+CommandeAExporter.adresse_2+";;"+ CommandeAExporter.codepostale + ";"+ CommandeAExporter.ville + ";"+ CommandeAExporter.pays + ";"+ CommandeAExporter.telephone + ";"+ CommandeAExporter.email + ";"+ CommandeAExporter.DateLivraison + ";"+ CommandeAExporter.HeureLivraison + ";"+ CommandeAExporter.Transporteur + ";;;"+ CommandeAExporter.commentaires); // E line

                                        CommandeAExporter.Lines = getLigneCommande(CommandeAExporter.NumCommande, recapLinesList_new); // Maybe thisssss

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

                                            //add zeros to the article reference only for ALDI 
                                            ConfigurationDNS dns = new ConfigurationDNS();
                                            dns.LoadSQL();
                                            if (dns.Prefix.Contains("CFCI") || dns.Prefix.Contains("TABLEWEAR") && CommandeAExporter.NomClient.Contains("ALDI"))
                                            {
                                                int maxChar = 13;
                                                int refChar = CommandeAExporter.Lines[i].codeArticle.Length;
                                                int addZero = maxChar - refChar;
                                                for (int index1 = 0; index1 < addZero; index1++)
                                                {
                                                    CommandeAExporter.Lines[i].codeArticle = "0" + CommandeAExporter.Lines[i].codeArticle;
                                                }
                                            }

                                            orderFileWriter.WriteLine("L;;" + ((CommandeAExporter.Lines[i].codeArticle.Length > 30) ? CommandeAExporter.Lines[i].codeArticle.Substring(0, 30) : CommandeAExporter.Lines[i].codeArticle) + ";;" + declarerpourrien[0] + ";"); // L line

                                        }
                                        orderFileWriter.WriteLine("F;" + CommandeAExporter.Lines.Count + ";" + qteTotal + ";"); // F line
                                        orderFileWriter.Close();

                                    }
                                    else
                                    {
                                    //Format Fichier plat
                                        orderFileWriter.WriteLine("ORDERS;" + CommandeAExporter.DO_MOTIF + ";" + CommandeAExporter.codeClient + ";" + CommandeAExporter.codeAcheteur + ";" + CommandeAExporter.codeFournisseur + ";;;" + CommandeAExporter.nom_contact + "." + CommandeAExporter.adresseLivraison.Replace("..", ".").Replace("...", ".") + ";" + CommandeAExporter.deviseCommande + ";;");
                                        if (CommandeAExporter.DateCommande != "")
                                        {
                                            CommandeAExporter.DateCommande = ConvertDate(CommandeAExporter.DateCommande);
                                        }

                                    //if (CommandeAExporter.DateCommande != " ")
                                    //{
                                    //    CommandeAExporter.conditionLivraison = "";
                                    //}

                                        orderFileWriter.WriteLine("ORDHD1;" + CommandeAExporter.DateCommande + ";" + CommandeAExporter.conditionLivraison + ";;");

                                        CommandeAExporter.Lines = getLigneCommande(CommandeAExporter.NumCommande, recapLinesList_new);

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

                                        orderFileWriter.WriteLine("ORDLIN;" + CommandeAExporter.Lines[i].NumLigne + ";" + CommandeAExporter.Lines[i].codeArticle + ";GS1;" + CommandeAExporter.Lines[i].codeAcheteur + ";" + CommandeAExporter.Lines[i].codeFournis + ";;A;" + CommandeAExporter.Lines[i].descriptionArticle + ";" + CommandeAExporter.Lines[i].Quantite.Replace(",", ".") + ";LM;" + CommandeAExporter.Lines[i].MontantLigne.Replace(",", ".") + ";;;" + CommandeAExporter.Lines[i].PrixNetHT.Replace(",", ".") + ";;;LM;;;;" + ConvertDate(CommandeAExporter.Lines[i].DateLivraison) + ";");
                                        }
                                        orderFileWriter.WriteLine("ORDEND;" + CommandeAExporter.MontantTotal.ToString().Replace(",", ".") + ";");

                                        orderFileWriter.Close();
                                    }
                                }

                                //Vérifier si le fichier a bien été créé et écrit
                                if (File.Exists(exportPath + @"\" + fileName))
                                {
                                    if (new FileInfo(exportPath + @"\" + fileName).Length > 0)
                                    {
                                        veolog_file_check = true;

                                        //add to backup folder
                                        addFileToBackUp(path.path + @"\BackUp\" + exportTo, exportPath + @"\" + fileName, fileName, logFileWriter);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("File: " + fileName + " does not exist!!!");
                                    logFileWriter.WriteLine(DateTime.Now + " : File: " + fileName + " does not exist!!!");
                                    //Console.ReadLine();
                                }

                                //update veolog delivery date
                                if (veolog_file_check)
                                {
                                    try
                                    {
                                        string delivery_date_veolog = string.Format("{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Ajouter la date de livraision \""+ delivery_date_veolog + "\" de Veolog de la commande \"" + CommandeAExporter.NumCommande + "\".");

                                        logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : SQL ===> " + QueryHelper.updateVeologDeliveryDate(true, CommandeAExporter.NumCommande, delivery_date_veolog));
                                        OdbcCommand command1 = new OdbcCommand(QueryHelper.updateVeologDeliveryDate(true, CommandeAExporter.NumCommande, delivery_date_veolog), connexion);
                                        {
                                            using (IDataReader reader = command1.ExecuteReader())
                                            {
                                                logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Date de livraison veolog à jour !");
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
                                        logFileWriter.Close();
                                        recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export de la commande est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
                                        return recapLinesList_new;
                                    }


                                    //update order statut
                                    try
                                    {
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Changer le statut de la commande \"" + CommandeAExporter.NumCommande + "\".");

                                        logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : SQL ===> " + QueryHelper.changeOrderStatut(true, CommandeAExporter.NumCommande));
                                        OdbcCommand command1 = new OdbcCommand(QueryHelper.changeOrderStatut(true, CommandeAExporter.NumCommande), connexion);
                                        {
                                            using (IDataReader reader = command1.ExecuteReader())
                                            {
                                                while (reader.Read())
                                                {
                                                    //Statut Update
                                                }
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                                        logFileWriter.Flush();
                                        logFileWriter.Close();
                                        recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export de la commande est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
                                        return recapLinesList_new;
                                    }
                                }
                                logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : Commande exportée avec succés.");

                            //jamp:;

                                //logFileWriter.Close();
                                //Close();
                            }
                            catch (Exception ex)
                            {
                                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                                logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : ERREUR :: " + ex.Message);
                                logFileWriter.Flush();
                                logFileWriter.Close();
                                Console.WriteLine(DateTime.Now + " | ExportCommande() : ERREUR :: " + ex.Message);
                                recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export de la commande est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
                            }
                        }
                        else
                        {
                            Console.WriteLine("CommandeAExporter == null");
                        }

                        //Console.WriteLine(DateTime.Now + " | ExportCommande() : Connexion close.");
                        connexion.Close();
                    }
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : ExportCommande() |  ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : ExportCommande() |  Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : ExportCommande() |  Scan annulée");
                        logFileWriter.Flush();
                        logFileWriter.Close();
                        recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export de la commande est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
                        return recapLinesList_new;
                    }
                }

                logFileWriter.Flush();
            }
            logFileWriter.Close();
            return recapLinesList_new;
        }
        #endregion

        #region Méthodes diverses
        /// <summary>
        /// Chargement de la fenêtre
        /// </summary>
        /// <param name="e">paramètres de l'évènement</param>

        public static void addFileToBackUp(string backUpFolderPath, string sourceFilePath, string filename, StreamWriter writer)
        {
            writer.WriteLine("");
            //check if the backup folder exist
            if (!Directory.Exists(backUpFolderPath))
            {
                writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Create BackUp folder at \""+backUpFolderPath+"\"");
                Directory.CreateDirectory(backUpFolderPath);
            }

            //copy the file to the backup folder
            if (File.Exists(backUpFolderPath + @"\" + filename))
            {
                int version = 0;
                //Get all .csv files in the folder
                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);
                writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File \"" + backUpFolderPath + @"\" + filename + "\" exist so add version it");

                for(int x=0; x < fileListing.GetFiles("*.csv").Length; x++)
                {
                    string[] cutFileName = filename.Split('_');
                    string withouExtension = cutFileName[3].Split('.')[0];
                    string newFileName = cutFileName[0] + "_" + cutFileName[1] + "_" + cutFileName[2] + "_" + withouExtension;
                    FileInfo Filename = fileListing.GetFiles("*.csv")[x];

                    if ((Filename.Name).Contains(newFileName))
                    {
                        version++;
                        writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Version: " + version + " || (" + Filename.Name + ").Contains(" + newFileName + ")");
                    }
                }
                //File.Delete(destFilePath);
                string[] cutFileName_1 = filename.Split('.');
                string newFileName_1 = cutFileName_1[0] + "_v" + version + "." + cutFileName_1[1];
                writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Copy file \"" + sourceFilePath + "\" to \"" + backUpFolderPath + @"\" + newFileName_1 + "\"");
                File.Copy(sourceFilePath, backUpFolderPath + @"\" + newFileName_1);
            }
            else
            {
                writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Copy file \"" + sourceFilePath + "\" to \"" + backUpFolderPath + @"\" + filename + "\"");
                File.Copy(sourceFilePath, backUpFolderPath + @"\" + filename);
            }
            
            writer.WriteLine("");
            writer.Flush();
        }

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

        private List<OrderLine> getLigneCommande(string code, List<CustomMailRecapLines> recapLinesList_new)
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
                recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export de la commande est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
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
