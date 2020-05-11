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

namespace AlertMail
{
    class Program
    {
        private static string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            try
            {
                int SW;
                Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();
                
                if (settings.isSettings())
                {
                    settings.Load();
                    SW = settings.configurationGeneral.general.showWindow;
                }
                else
                {
                    SW = 5;
                }

                // hide or show the running software window
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mode débogage 2 : " + ex.Message);
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Gestion des notifications email
            ///
            Alert_Mail.Classes.ConfigurationSaveLoad file = new Alert_Mail.Classes.ConfigurationSaveLoad();
            if (file.isSettings())
            {
                file.Load();
                Alert_Mail.Classes.ConfigurationEmail settings = file.configurationEmail;

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Email types
                /// 
                if (args.Length == 0)
                {
                    pendingFilesMail(settings);
                }
                else if (args.Length > 0 && args[0] == "Import_Mail_11") //Send a Mail after the connector execution 
                {
                    importMail(settings, 11);
                }
                else if (args.Length > 0 && args[0] == "Import_Mail_22") //Send a Mail after the connector execution 
                {
                    importMail(settings, 22);
                }
                else if (args.Length > 0 && args[0] == "Export_Mail_11") //Send a Mail after the connector execution 
                {
                    exportMail(settings, 11);
                }
                else if (args.Length > 0 && args[0] == "Export_Mail_22") //Send a Mail after the connector execution 
                {
                    exportMail(settings, 22);
                }
                else if (args.Length > 0 && args[0] == "All_Errors") //Send a Mail after the connector execution 
                {
                    errorMail(settings);
                }
                else if (args.Length > 0 && args[0] == "Error_Summary")
                {
                    summaryMail(file, settings);
                }
                else
                {
                    Console.WriteLine("Argument inconnue!!!");
                }
            }
            else
            {
                Console.WriteLine("Aucune configuration e-mail trouvé!");
            }

            
            //Console.ReadLine();
        }


        private static void importMail(ConfigurationEmail settings, int _x_)
        {
            Console.WriteLine("Import mail...");

            if (settings.emailImport.active)
            {
                if (settings.emailImport.eachDocument && _x_ == 11)
                {

                }else if (settings.emailImport.atTheEnd && _x_ == 11)
                {

                }
                else
                {

                }
            }
            else
            {
                Console.WriteLine("Import mail est désactivé...");
            }
        }

        private static void exportMail(ConfigurationEmail settings, int _x_)
        {
            Console.WriteLine("Export mail...");

            if (settings.emailExport.active)
            {
                if (settings.emailExport.eachDocument && _x_ == 11)
                {

                }
                else if (settings.emailExport.atTheEnd && _x_ == 11)
                {

                }
                else
                {

                }
            }
            else
            {
                Console.WriteLine("Export mail est désactivé...");
            }
        }

        private static void errorMail(ConfigurationEmail settings)
        {
            Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();
            try
            {
                Console.WriteLine("EndSoftwareExe...");

                if (settings.emailError.active)
                {

                    if (settings.emailLists.emailClientList.Count == 0 && settings.emailLists.emailTeamList.Count == 0)
                    {
                        Console.WriteLine("Aucun address mail enregistré!");
                    }

                    MailCustom mMailCustom_client = null;
                    MailCustom mMailCustom_log = null;
                    try
                    {
                        mMailCustom_client = emailManagement.generateMailBody("client");
                        mMailCustom_log = emailManagement.generateMailBody("log");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("");
                        Console.WriteLine(DateTime.Now + " | Main() : *********** Exception generateMailBody() ***********");
                        Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                        Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);
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
                                emailManagement.EnvoiMail(settings, "client", mMailCustom_client.subject, mMailCustom_client.body, mMailCustom_client.attachements);   //cheminLogFile
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Envoi Mail Client ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Nothing to send for the client");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Send mail to client is desable");
                    }

                    if (settings.emailError.informTeam)
                    {
                        if (mMailCustom_client != null)
                        {
                            try
                            {
                                Console.WriteLine("Envoi de mail log en cours....");
                                emailManagement.EnvoiMail(settings, "log", mMailCustom_log.subject, mMailCustom_log.body, mMailCustom_log.attachements);   //cheminLogFile
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Envoi Mail log ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No log to send for the team");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Send log mail to team is desable");
                    }

                    if (mMailCustom_client != null && mMailCustom_log != null)
                    {
                        if (File.Exists("Mail_IMP.ml"))
                        {
                            File.Delete("Mail_IMP.ml");
                        }
                        if (File.Exists("Mail_EXP.ml"))
                        {
                            File.Delete("Mail_EXP.ml");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Les mails d'erreur sont désactivé!");
                }


                //stop at the end of the program
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Main EndSoftwareExe ***********");
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);
            }
        }

        private static void summaryMail(ConfigurationSaveLoad file, ConfigurationEmail settings)
        {
            Console.WriteLine("Résumer mail...");

            try
            {
                if (settings.emailSummary.active)
                {
                    Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();

                    string directoryName_ErrorFile = Directory.GetCurrentDirectory() + @"\" + "Error File";
                    if (!Directory.Exists(directoryName_ErrorFile))
                    {
                        Directory.CreateDirectory(directoryName_ErrorFile);
                    }

                    DirectoryInfo fileListing1 = new DirectoryInfo(directoryName_ErrorFile);
                    FileInfo[] allFiles_error = fileListing1.GetFiles("*.csv");

                    string[,] errorFilesFileNameList_ = new string[allFiles_error.Length, 2];
                    List<string> errorFilesFileNameList = new List<string>();
                    List<string> errorFilesFileList = new List<string>();

                    DateTime today = DateTime.Now;
                    DateTime lastActivated = settings.emailSummary.lastActivated;
                    TimeSpan ts = today - lastActivated;

                    if (ts.TotalHours >= Convert.ToDouble(settings.emailSummary.hours))
                    {
                        Console.WriteLine("Envoi un Mail Résumer en cours...");

                        if (File.Exists("Mail_Recap.ml"))
                        {
                            if (settings.emailLists.emailClientList.Count == 0 && settings.emailLists.emailTeamList.Count == 0)
                            {
                                Console.WriteLine("Aucun address mail enregistré!");
                            }

                            CustomMailRecap recap = new CustomMailRecap();
                            recap.Load("Mail_Recap.ml");
                            string infoBody_end = "";
                            string infoBody2 = "";
                            string infoBodyHeader2 = "";

                            List<string> attachements = new List<string>();

                            //import file error
                            if (allFiles_error.Length > 0)
                            {
                                CustomMailRecap mailRecap = new CustomMailRecap();
                                mailRecap.Lines = new List<CustomMailRecapLines>();
                                mailRecap.Load("Mail_Recap.ml");

                                infoBodyHeader2 += "Il y a " + allFiles_error.Length + " fichier(s) qui sont tombé en erreur lors de l'import, qui sont dans le le répertoire '" + directoryName_ErrorFile + "' :\n";

                                for (int x = 0; x < allFiles_error.Length; x++)
                                {
                                    errorFilesFileNameList_[x, 0] = allFiles_error[x].Name;
                                    errorFilesFileNameList_[x, 1] = allFiles_error[x].FullName;

                                    errorFilesFileNameList.Add(allFiles_error[x].Name);
                                    errorFilesFileList.Add(allFiles_error[x].FullName);
                                }

                                int a = 0;
                                List<string> unknownFileName = new List<string>();
                                List<string> unknownFileEDI = new List<string>();
                                for (int y = 0; y < mailRecap.Lines.Count; y++)
                                {
                                    Console.WriteLine(y + " - FileInfo : " + errorFilesFileNameList[y]);
                                    if (errorFilesFileNameList.Contains(mailRecap.Lines[y].FileName))
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

                                List<string> mail_r = new List<string>();
                                for (int x = 0; x < mailRecap.Lines.Count; x++)
                                {
                                    mail_r.Add(mailRecap.Lines[x].FileName);
                                }

                                infoBody2 += "Voici d'autre fichier en erreur, ils n'ont pas de log générés à partir du connecteur :\n";
                                for (int x = 0; x < allFiles_error.Length; x++)
                                {
                                    if (!mail_r.Contains(allFiles_error[x].Name))
                                    {
                                        Console.WriteLine("x: " + x + " || allFiles_error : " + allFiles_error[x].Name);
                                        infoBody2 += "\tNom du fichier : \"" + allFiles_error[x].Name + "\", à " + emailManagement.getFileSize(allFiles_error[x].Length) + "\n";
                                        attachements.Add(allFiles_error[x].FullName);
                                    }
                                }

                                Console.WriteLine("Size 1 : " + attachements.Count);

                                if (a == 0)
                                {
                                    infoBodyHeader2 = "";
                                }
                            }

                            if (settings.emailSummary.active)
                            {
                                Console.WriteLine("Size 2 : " + allFiles_error.Length);
                                Console.WriteLine("Size 3 : " + infoBody2.Length);
                                if (allFiles_error.Length > 0 && infoBody2.Length > 0)
                                {
                                    infoBody_end += "Bonjour Team BDC, \n\nVoici un récapitulatif des documents. \n" + infoBodyHeader2 + infoBody2;

                                    emailManagement.EnvoiMail(settings, "log", "Résumer [" + recap.Client + "] " + recap.Subject, infoBody_end + "\nCordialement,\nConnecteur SAGE [" + recap.Client + "].", attachements);

                                    //delete recap file
                                    if (File.Exists("Mail_Recap.ml"))
                                    {
                                        File.Delete("Mail_Recap.ml");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Send log mail to team desable");
                            }

                            // the DateTime
                            settings.emailSummary.lastActivated = DateTime.Now;
                            file.configurationEmail = settings;
                            file.saveInfo();
                        }
                        else
                        {
                            string infoBody = "";
                            List<string> attachements = new List<string>();
                            string dns = emailManagement.getDNS();

                            if (settings.emailSummary.active)
                            {
                                //import file error
                                if (allFiles_error.Length > 0)
                                {
                                    infoBody = "Il y a " + allFiles_error.Length + " fichier(s) qui sont dans le le répertoire erreur '" + directoryName_ErrorFile + "' :\n";

                                    for (int x = 0; x < allFiles_error.Length; x++)
                                    {
                                        infoBody += "\tNom du fichier : \"" + allFiles_error[x].Name + "\", à " + emailManagement.getFileSize(allFiles_error[x].Length) + "\n Date de création " + string.Format("{0:yyyy/MM/dd HH:mm}", allFiles_error[x].CreationTime) + " et date de modification " + string.Format("{0:yyyy/MM/dd HH:mm}", allFiles_error[x].LastWriteTime) + ".\n";

                                        if (!attachements.Contains(allFiles_error[x].FullName))
                                        {
                                            attachements.Add(allFiles_error[x].FullName);
                                        }
                                    }
                                }

                                if (allFiles_error.Length > 0)
                                {
                                    emailManagement.EnvoiMail(settings, "log", "Résumer [" + dns + "]", "Bonjour Team BDC,\n\n" + infoBody + "\n\nCordialement,\nConnecteur SAGE [" + dns + "].", attachements);
                                }

                                Console.WriteLine("No Mail_Recap.ml File!");
                            }
                            else
                            {
                                Console.WriteLine("Send log mail to team desable");
                            }

                            // the DateTime
                            settings.emailSummary.lastActivated = DateTime.Now;
                            file.configurationEmail = settings;
                            file.saveInfo();
                        }
                        //cMail.remaningTicks = cMail.totalTicks;
                        //cMail.password = Utils.Encrypt(cMail.password);
                        //new ConfSendMail().saveInfo(cMail);
                    }
                }
                else
                {
                    Console.WriteLine("Envoi des mails de résumer sont désactivé!");
                }

                //stop at the end of the program
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Main CheckErrorFiles ***********");
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);
            }
        }

        private static void pendingFilesMail(ConfigurationEmail settings)
        {
            if (settings.emailPendingFiles.active)
            {
                Console.WriteLine("Analyser le dossier CSV....");

                string directoryName_csv = Directory.GetCurrentDirectory() + @"\" + "CSV";
                if (!Directory.Exists(directoryName_csv))
                {
                    Directory.CreateDirectory(directoryName_csv);
                }

                Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();

                DirectoryInfo fileListing1 = new DirectoryInfo(directoryName_csv);
                FileInfo[] allFiles_csv = fileListing1.GetFiles("*.csv");

                List<string> fileNameAttachmentList = new List<string>();

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

                        if (ts.TotalHours >= Convert.ToDouble(settings.emailPendingFiles.hours))
                        {
                            ok = true;
                            fileNameAttachmentList.Add(allFiles_csv[x].FullName);
                            body += "\t - " + (x + 1) + " Nom : " + allFiles_csv[x].Name + " , Date de creation : " + string.Format("{0:yyyy/MM/dd HH:mm}", allFiles_csv[x].CreationTime) + ", Date d'aujourd'huid : " + string.Format("{0:yyyy/MM/dd HH:mm}", today) + " , la durée d'existance : " + string.Format("{0}", ts) + " heure(s). \n";
                        }
                    }

                    if (ok)
                    {
                        header = "Bonjour\n\nIl y a " + fileNameAttachmentList.Count + " fichier(s) en attente depuis " + string.Format("{0:dddd, d MMMM , yyyy à HH:mm}", today.AddHours(-settings.emailPendingFiles.hours)) + " : \n";
                        Alert_Mail.EmailManagement manager = new Alert_Mail.EmailManagement();

                        manager.EnvoiMail(settings, "log", "[" + emailManagement.getDNS() + "] Scan du dossier CSV", header + body + "\nCordialement,\nConnecteur SAGE version équipe.", fileNameAttachmentList);
                    }
                    else
                    {
                        Console.WriteLine("Aucun mail envoyé.");
                    }

                }
                else
                {
                    Console.WriteLine("Aucun fichier EDI trouvé !");
                }
            }
            else
            {
                Console.WriteLine("Les mails des fichiers en attente sont");
            }

        }
    }
}
