using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System.Runtime.InteropServices;
using Alert_Mail.Classes;
using Alert_Mail.Classes.Custom;
using Database.Manager;

namespace AlertMail
{
    class Program
    {
        private static Database.Database db;
        private static string logDirectoryName_mail = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_AlertMail";
        private static string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            const int SW_SHOWMINIMIZED = 2;
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_SHOWMINIMIZED);

            // Hide or not the screen
            Database.Database db = new Database.Database();
            db.initTables();
            Database.Model.Settings _settings_ = db.settingsManager.get(db.connectionString, 1);
            int SW_HIDE_SHOW = _settings_.showWindow;

            // SW_HIDE = 0
            // SW_SHOW = 5
            if (SW_HIDE_SHOW == 5)
            {
                // Show
                ShowWindow(handle, 9);
                ShowWindow(handle, SW_HIDE_SHOW);
                int Height = Console.LargestWindowHeight - 20;
                int Width = Console.LargestWindowWidth - 20;
                Console.SetWindowSize(Width, Height);
                Console.SetWindowPosition(0, 0);
            }
            else if (SW_HIDE_SHOW == 0)
            {
                // Hide
                Console.SetWindowSize(1, 1);
                Console.SetWindowPosition(0, 0);
                ShowWindow(handle, SW_HIDE_SHOW);
            }

            db.alertMailLogManager.createTable(db.connectionString);

            db.alertMailLogManager.insert(db.connectionString, "");

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Gestion des notifications email
            ///
            Alert_Mail.Classes.ConfigurationSaveLoad file = new Alert_Mail.Classes.ConfigurationSaveLoad();
            if (file.isSettings())
            {
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Fichier SettingMail.json existe.");

                file.Load();
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Fichier SettingMail.json chargé.");
                Alert_Mail.Classes.ConfigurationEmail settings = file.configurationEmail;

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Email types
                /// 
                if (args.Length == 0)
                {
                    pendingFilesMail(settings, db); //Send a Mail every x time
                }
                else if (args.Length > 0 && args[0] == "All_Errors") //Send a Mail after the connecteur execution 
                {
                    errorMail(settings, db);
                }
                else if (args.Length > 0 && args[0] == "Error_Summary") //Send a Mail every x time
                {
                    summaryMail(file, settings, db);
                }
                else
                {
                    Console.WriteLine("Argument inconnue!!!");
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Argument inconnue!!!");
                }
            }
            else
            {
                Console.WriteLine("Aucune configuration e-mail trouvé!");
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Aucune configuration e-mail trouvé!");
            }
            db.alertMailLogManager.insert(db.connectionString, "");

            //Console.ReadLine();
        }

        private static void errorMail(ConfigurationEmail settings, Database.Database db)
        {
            Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();
            try
            {
                db.alertMailLogManager.insert(db.connectionString, "");
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | EndSoftwareExe...");
                Console.WriteLine("EndSoftwareExe...");

                if (settings.emailError.active)
                {
                    if (settings.emailLists.emailClientList.Count == 0 && settings.emailLists.emailTeamList.Count == 0)
                    {
                        Console.WriteLine("Aucun address mail enregistré!");
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | EAucun address mail enregistré!");
                        return;
                    }

                    MailCustom mMailCustom_client = null;
                    MailCustom mMailCustom_log = null;
                    try
                    {
                        mMailCustom_client = emailManagement.generateMailBody("client_error", db);
                        mMailCustom_log = emailManagement.generateMailBody("log", db);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("");
                        Console.WriteLine(DateTime.Now + " | Main() : *********** Exception generateMailBody() ***********");
                        Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                        Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);

                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " :: ############################### Exception generateMailBody() ###############################");
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Message : " + ex.Message);
                        //db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | StackTrace : " + ex.StackTrace);
                        db.alertMailLogManager.insert(db.connectionString, "");

                        Console.WriteLine("");
                        mMailCustom_client = null;
                        mMailCustom_log = null;
                    }

                    //Envoi
                    if (settings.emailError.informClient)
                    {
                        if (mMailCustom_client != null)
                        {
                            try
                            {
                                Console.WriteLine("Envoi de mail client en cours....");
                                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Envoi de mail client en cours....");
                                emailManagement.EnvoiMail(settings, "client", mMailCustom_client.subject, mMailCustom_client.body, mMailCustom_client.attachements, db);   //cheminLogFile
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Envoi Mail Error Client ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);

                                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " :: ############################### Exception Envoi Mail Error Client ###############################");
                                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Message : " + ex.Message);
                                //db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | StackTrace : " + ex.StackTrace);
                                db.alertMailLogManager.insert(db.connectionString, "");
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Nothing to send for the client");
                            db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Rien à envoie vers le client");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Send mail to client is desable");
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Envoie mail client est désactivité");
                    }

                    if (settings.emailError.informTeam)
                    {
                        if (mMailCustom_log != null)
                        {
                            try
                            {
                                Console.WriteLine("Envoi de mail log en cours....");
                                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Envoi de mail log en cours....");
                                emailManagement.EnvoiMail(settings, "log", mMailCustom_log.subject, mMailCustom_log.body, mMailCustom_log.attachements, db);   //cheminLogFile
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Envoi Mail Error Team log ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);

                                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " :: ############################### Exception Envoi Mail Error Team log ###############################");
                                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Message : " + ex.Message);
                                //db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | StackTrace : " + ex.StackTrace);
                                db.alertMailLogManager.insert(db.connectionString, "");
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No log to send for the team");
                            db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Rien à envoyer au team");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Send log mail to team is desable");
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Envoie mail log au Team");
                    }

                    if (mMailCustom_client != null && mMailCustom_log != null)
                    {
                        ConfigurationCustomMailSaveLoad xxx = new ConfigurationCustomMailSaveLoad();

                        if (File.Exists(xxx.fileName_ERR_Imp))
                        {
                            File.Delete(xxx.fileName_ERR_Imp);
                        }
                        if (File.Exists(xxx.fileName_ERR_Exp))
                        {
                            File.Delete(xxx.fileName_ERR_Exp);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Les mails d'erreur sont désactivé!");
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Les mails d erreur sont désactivé!");
                }

                db.alertMailLogManager.insert(db.connectionString, "");

                //stop at the end of the program
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Main EndSoftwareExe ***********");
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);


                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " :: ############################### Exception Main EndSoftwareExe ###############################");
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | Message : " + ex.Message);
               // db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: errorMail() | StackTrace : " + ex.StackTrace);
                db.alertMailLogManager.insert(db.connectionString, "");
            }
        }

        private static void summaryMail(ConfigurationSaveLoad file, ConfigurationEmail settings, Database.Database db)
        {
            Console.WriteLine("Résumer mail...");
            db.alertMailLogManager.insert(db.connectionString, "");
            db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Creation d une instance.");

            try
            {
                if (settings.emailSummary.active)
                {
                    Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();

                    string directoryName_tmpFile = Directory.GetCurrentDirectory() + @"\" + "tmp";
                    if (!Directory.Exists(directoryName_tmpFile))
                    {
                        Directory.CreateDirectory(directoryName_tmpFile);
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Ce répertoire "+ directoryName_tmpFile + " n existe pas , alors créer le.");
                    }

                    DirectoryInfo fileListing1 = new DirectoryInfo(directoryName_tmpFile);
                    FileInfo[] allFiles_error = fileListing1.GetFiles("*.csv");

                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Il y a " + allFiles_error.Length + " fichier(s) dans le répertoire.");

                    string[,] errorFilesFileNameList_ = new string[allFiles_error.Length, 2];
                    List<string> errorFilesFileNameList = new List<string>();
                    List<string> errorFilesFileList = new List<string>();

                    DateTime today = DateTime.Now;
                    DateTime lastActivated = settings.emailSummary.lastActivated;
                    TimeSpan ts = today - lastActivated;

                    Console.WriteLine("Today : " + today);
                    Console.WriteLine("LastActivated : " + lastActivated);
                    Console.WriteLine("Ago : " + ts);

                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Date aujourd hui => " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", today));
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Dernière fois lancé => " + lastActivated);
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Temps de lancement depuis la dernière fois => "+ts.TotalHours+" heure(s)");

                    if (ts.TotalHours >= Convert.ToDouble(settings.emailSummary.hours))
                    {
                        Console.WriteLine("Envoi un Mail Résumer en cours...");
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Envoi un Mail Résumer en cours...");

                        ConfigurationCustomMailSaveLoad mConfigurationCustomMailSaveLoad = new ConfigurationCustomMailSaveLoad();

                        if (mConfigurationCustomMailSaveLoad.isSettings(mConfigurationCustomMailSaveLoad.fileName_ERR_recap))
                        {
                            // uncomment the Recap file creation too!!!!!
                            /*
                            if (settings.emailLists.emailClientList.Count == 0 && settings.emailLists.emailTeamList.Count == 0)
                            {
                                Console.WriteLine("Aucun address mail enregistré!");
                            }

                            CustomMailRecap recap = new CustomMailRecap();
                            //recap.Load("Mail_Recap.ml");
                            string infoBody_end = "";
                            string infoBody2 = "";
                            string infoBodyHeader2 = "";

                            List<string> attachements = new List<string>();

                            //import file error
                            if (allFiles_error.Length > 0)
                            {
                                mConfigurationCustomMailSaveLoad.Load(mConfigurationCustomMailSaveLoad.fileName_ERR_recap);
                                CustomMailRecap mailRecap = mConfigurationCustomMailSaveLoad.customMailRecap;
                                
                                //mailRecap.Lines = new List<CustomMailRecapLines>();

                                if(mailRecap.Lines.Count > 0)
                                {
                                    infoBodyHeader2 += "Il y a " + allFiles_error.Length + " fichier(s) qui sont tombé en erreur lors de l'import. Ils qui sont dans le le répertoire '" + directoryName_ErrorFile + "' :\n";
                                }

                                for (int x = 0; x < mailRecap.Lines.Count; x++)
                                {
                                    errorFilesFileNameList_[x, 0] = mailRecap.Lines[x].NumCommande;
                                    errorFilesFileNameList_[x, 1] = mailRecap.Lines[x].FilePath;

                                    errorFilesFileNameList.Add(mailRecap.Lines[x].NumCommande);
                                    errorFilesFileList.Add(mailRecap.Lines[x].FilePath);
                                }

                                Console.WriteLine("mailRecap.Lines.Count : " + mailRecap.Lines.Count);

                                int a = 0;
                                List<string> unknownFileName = new List<string>();
                                List<string> unknownFileEDI = new List<string>();
                                for (int y = 0; y < mailRecap.Lines.Count; y++)
                                {
                                    Console.WriteLine(y + " - FileNameList : " + errorFilesFileNameList[y] + "\nNumCommande : " + mailRecap.Lines[y].NumCommande);
                                    if (errorFilesFileNameList.Contains(mailRecap.Lines[y].NumCommande))
                                    {
                                        Console.WriteLine("y: " + y + " || FileInfo : " + allFiles_error[y].Name + " == Mail Recap : " + allFiles_error[y].Name);
                                        infoBody2 += (y + 1) + " -\t Le numéro du document \"" + mailRecap.Lines[y].DocumentReference + "\"\nNom du fichier : " + mailRecap.Lines[y].FileName + "\nMessage erreur : " + mailRecap.Lines[y].DocumentErrorMessageDebug + "\nStackTrace: " + mailRecap.Lines[y].DocumentErrorStackTraceDebug + "\nL'erreur peut etre trouvé dans " + mailRecap.Lines[y].FilePath + "\n\n";
                                        a++;

                                        if (!attachements.Contains(mailRecap.Lines[y].FilePath))
                                        {
                                            attachements.Add(mailRecap.Lines[y].FilePath);
                                        }
                                    }
                                    else
                                    {
                                        unknownFileName.Add(errorFilesFileNameList_[y, 0]);
                                        unknownFileEDI.Add(errorFilesFileNameList_[y, 1]);
                                    }
                                }

                                for (int x = 0; x < allFiles_error.Length; x++)
                                {
                                    infoBody2 += "\tNom du fichier : \"" + allFiles_error[x].Name + "\", à " + emailManagement.getFileSize(allFiles_error[x].Length) + "\n";
                                    attachements.Add(allFiles_error[x].FullName);
                                }

                                Console.WriteLine("Size 1 : " + attachements.Count);

                                if (a == 0)
                                {
                                    infoBodyHeader2 = "";
                                }
                            }

                            if (allFiles_error.Length > 0 && infoBody2.Length > 0)
                            {
                                infoBody_end += "Bonjour Team BDC, \n\nVoici un récapitulatif des documents. \n" + infoBodyHeader2 + infoBody2;

                                emailManagement.EnvoiMail(settings, "log", "Résumer [" + recap.Client + "] " + recap.Subject, infoBody_end + "\nCordialement,\nConnecteur SAGE [" + recap.Client + "].", attachements);

                                //delete recap file
                                
                                if (File.Exists(mConfigurationCustomMailSaveLoad.fileName_ERR_recap))
                                {
                                    File.Delete(mConfigurationCustomMailSaveLoad.fileName_ERR_recap);
                                }
                            }

                            // the DateTime
                            settings.emailSummary.lastActivated = DateTime.Now;
                            file.configurationEmail = settings;
                            file.saveInfo();
                            */
                        }
                        else
                        {
                            string infoBody = "";
                            List<string> attachements = new List<string>();
                            string dns = emailManagement.getDNS();

                            //import file error
                            if (allFiles_error.Length > 0)
                            {
                                infoBody = "Il y a " + allFiles_error.Length + " fichier(s) qui sont dans le répertoire erreur '" + directoryName_tmpFile + "' :\n";

                                for (int x = 0; x < allFiles_error.Length; x++)
                                {
                                    infoBody += "\tNom du fichier : \"" + allFiles_error[x].Name + "\", à " + emailManagement.getFileSize(allFiles_error[x].Length) + "\n Date de création " + string.Format("{0:yyyy/MM/dd HH:mm}", allFiles_error[x].CreationTime) + " et date de modification " + string.Format("{0:yyyy/MM/dd HH:mm}", allFiles_error[x].LastWriteTime) + ".\n";

                                    if (!attachements.Contains(allFiles_error[x].FullName))
                                    {
                                        attachements.Add(allFiles_error[x].FullName);

                                    }
                                }

                                if (allFiles_error.Length > 0)
                                {
                                    emailManagement.EnvoiMail(settings, "log", "Résumer [" + dns + "]", "Bonjour Team BDC,\n\n" + infoBody + "\n\nCordialement,\nConnecteur SAGE [" + dns + "].", attachements, db);

                                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Mail Envoyé => Résumer [" + dns + "]");
                                }

                                Console.WriteLine("No Mail_Recap.ml File!");

                                // the DateTime
                                settings.emailSummary.lastActivated = DateTime.Now;
                                file.configurationEmail = settings;
                                file.saveInfo();
                                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Reset lastActivated to " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", settings.emailSummary.lastActivated));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Envoi des mails de résumer sont désactivé!");
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Envoi des mails de résumer sont désactivé!");
                    }

                    //stop at the end of the program

                }//Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Main CheckErrorFiles ***********");
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);

                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " :: ############################### Exception EnvoiMail ###############################");
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | Message : " + ex.Message);
                //db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: summaryMail() | StackTrace : " + ex.StackTrace);
                db.alertMailLogManager.insert(db.connectionString, "");
            }
        }

        private static void pendingFilesMail(ConfigurationEmail settings, Database.Database db)
        {
            db.alertMailLogManager.insert(db.connectionString, "");
            db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Creation d une instance.");
            if (settings.emailPendingFiles.active)
            {
                Console.WriteLine("Analyser le dossier CSV....");
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Analyser le dossier CSV....");

                string directoryName_csv = Directory.GetCurrentDirectory() + @"\" + "CSV";
                if (!Directory.Exists(directoryName_csv))
                {
                    Directory.CreateDirectory(directoryName_csv);
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Le répertoire " + directoryName_csv + " n existe pas, alors créer le.");
                }

                Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();

                DirectoryInfo fileListing1 = new DirectoryInfo(directoryName_csv);
                FileInfo[] allFiles_csv = fileListing1.GetFiles("*.csv");

                List<string> fileNameAttachmentList = new List<string>();

                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Dans le répertoire " + directoryName_csv+" il y a "+allFiles_csv.Length+" en attante....");

                if (allFiles_csv.Length > 0)
                {
                    string header = "";
                    string body = "";
                    bool ok = false;
                    DateTime today = DateTime.Now;

                    for (int x = 0; x < allFiles_csv.Length; x++)
                    {
                        FileInfo csv_file = allFiles_csv[x];
                        DateTime fileDateTime = File.GetCreationTime(csv_file.FullName);
                        TimeSpan ts = today - fileDateTime;

                        Console.WriteLine("Today : " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", today));
                        Console.WriteLine("File DateTime : " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", fileDateTime));
                        Console.WriteLine("Ago : " + ts);

                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Index " + (x+1)+"/"+allFiles_csv.Length);
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Fichier => " + allFiles_csv[0].Name);
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Fichier DateTime => " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", fileDateTime));
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Fichier Existance => " + ts.TotalHours + " Heures");

                        int lines = 0;
                        if (ts.TotalHours >= Convert.ToDouble(settings.emailPendingFiles.hours))
                        {
                            ok = true;
                            fileNameAttachmentList.Add(allFiles_csv[x].FullName);
                            body += "\t - " + (x + 1) + " Nom : " + allFiles_csv[x].Name + " , Date de creation : " + string.Format("{0:yyyy/MM/dd HH:mm}", allFiles_csv[x].CreationTime) + ", Date d'aujourd'huid : " + string.Format("{0:yyyy/MM/dd HH:mm}", today) + " , la durée d'existance : " + string.Format("{0}", ts) + " heure(s). \n";
                            lines++;
                        }
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Nombre de fichier => " + lines);
                    }

                    if (ok)
                    {
                        header = "Bonjour\n\nIl y a " + fileNameAttachmentList.Count + " fichier(s) en attente depuis " + string.Format("{0:dddd, d MMMM , yyyy à HH:mm}", today.AddHours(-settings.emailPendingFiles.hours)) + " : \n";
                        Alert_Mail.EmailManagement manager = new Alert_Mail.EmailManagement();

                        manager.EnvoiMail(settings, "log", "[" + emailManagement.getDNS() + "] Scan du dossier CSV", header + body + "\nCordialement,\nConnecteur SAGE version équipe.", fileNameAttachmentList, db);
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Mail envoyé!");
                    }
                    else
                    {
                        Console.WriteLine("Aucun mail envoyé.");
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Aucun mail envoyé.");
                    }

                }
                else
                {
                    Console.WriteLine("Aucun fichier EDI trouvé !");
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Aucun fichier EDI trouvé !");
                }
            }
            else
            {
                Console.WriteLine("Les mails des fichiers sont en attente");
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: pendingFilesMail() | Les mails des fichiers sont en attente");
            }
            db.alertMailLogManager.insert(db.connectionString, "");
        }
    }
}
