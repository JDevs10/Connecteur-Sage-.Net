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
        private static string logFileName_mail;
        private static string logDirectoryName_mail = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_AlertMail";
        private static string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            //Create log file
            logFileName_mail = logDirectoryName_mail + @"\" + string.Format("LOG_Import_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile_mail = File.Create(logFileName_mail);
            using (StreamWriter logFileWriter_mail = new StreamWriter(logFile_mail))
            {
                // Load intro
                Connecteur_Info.Custom.Batch_Intro intro_ = new Connecteur_Info.Custom.Batch_Intro();
                intro_.intro(logFileWriter_mail);

                logFileWriter_mail.WriteLine("");
                try
                {
                    int SW;
                    Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();

                    if (settings.isSettings())
                    {
                        logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: Fichier init.json existe.");

                        settings.Load();
                        SW = settings.configurationGeneral.general.showWindow;
                        logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: Fichier init.json chargé.");
                    }
                    else
                    {
                        SW = 5;
                        logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: Fichier init.json n'existe pas.");
                        logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: SW = 5.");
                    }
                    logFileWriter_mail.WriteLine("");
                    logFileWriter_mail.Flush();

                    // hide or show the running software window
                    var handle = GetConsoleWindow();
                    ShowWindow(handle, SW);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mode débogage 2 : " + ex.Message);
                    logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: ##### [ERROR] Window Mode ##################################################");
                    logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: Message => "+ex.Message);
                    logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: StackTrace => " + ex.StackTrace);
                    logFileWriter_mail.WriteLine("");
                    logFileWriter_mail.Flush();
                    return;
                }

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Gestion des notifications email
                ///
                Alert_Mail.Classes.ConfigurationSaveLoad file = new Alert_Mail.Classes.ConfigurationSaveLoad();
                if (file.isSettings())
                {
                    logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: Fichier SettingMail.json existe.");

                    file.Load();
                    logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: Fichier SettingMail.json chargé.");
                    Alert_Mail.Classes.ConfigurationEmail settings = file.configurationEmail;

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /// Email types
                    /// 
                    if (args.Length == 0)
                    {
                        pendingFilesMail(settings, logFileWriter_mail); //Send a Mail every x time
                    }
                    else if (args.Length > 0 && args[0] == "Import_Mail_11") //Send a Mail after each document import
                    {
                        importMail(settings, 11, logFileWriter_mail);
                    }
                    else if (args.Length > 0 && args[0] == "Import_Mail_22") //Send a Mail after all documents imported 
                    {
                        importMail(settings, 22, logFileWriter_mail);
                    }
                    else if (args.Length > 0 && args[0] == "Export_Mail_11") //Send a Mail after each document export
                    {
                        exportMail(settings, 11, logFileWriter_mail);
                    }
                    else if (args.Length > 0 && args[0] == "Export_Mail_22") //Send a Mail after all documents exported 
                    {
                        exportMail(settings, 22, logFileWriter_mail);
                    }
                    else if (args.Length > 0 && args[0] == "All_Errors") //Send a Mail after the connecteur execution 
                    {
                        errorMail(settings, logFileWriter_mail);
                    }
                    else if (args.Length > 0 && args[0] == "Error_Summary") //Send a Mail every x time
                    {
                        summaryMail(file, settings, logFileWriter_mail);
                    }
                    else
                    {
                        Console.WriteLine("Argument inconnue!!!");
                        logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: Argument inconnue!!!");
                    }
                }
                else
                {
                    Console.WriteLine("Aucune configuration e-mail trouvé!");
                    logFileWriter_mail.WriteLine(DateTime.Now + " : AlertMail :: Aucune configuration e-mail trouvé!");
                }
                logFileWriter_mail.WriteLine("");
                logFileWriter_mail.Flush();
                logFileWriter_mail.Close();

                //Console.ReadLine();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// _x_ argument (11 or 22) depends, if its after each import "11" or at the end of all import "22"
        /// 
        private static void importMail(ConfigurationEmail settings, int _x_, StreamWriter writer)
        {
            if (settings.emailImport.active)
            {
                if (settings.emailImport.eachDocument && _x_ == 11)
                {

                }
                else if (settings.emailImport.atTheEnd && _x_ == 22)
                {
                    Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();
                    try
                    {
                        // get and load the file
                        Alert_Mail.Classes.ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new Alert_Mail.Classes.ConfigurationCustomMailSaveLoad();
                        configurationCustomMailSaveLoad.Load(configurationCustomMailSaveLoad.fileName_SUC_Imp, new List<CustomMailSuccess>());

                        // Mapping
                        MailCustom mMailCustom_client = null;
                        MailCustom mMailCustom_team = null;
                        try
                        {
                            mMailCustom_client = emailManagement.generateMailBody("client_import", writer);
                            mMailCustom_team = emailManagement.generateMailBody("team_import", writer);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " | Main() : *********** Exception generateMailBody() ***********");
                            Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                            Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);

                            writer.WriteLine(DateTime.Now + " :: ############################### Exception generateMailBody() ###############################");
                            writer.WriteLine(DateTime.Now + " : AlertMail :: importMail() | Message : " + ex.Message);
                            writer.WriteLine(DateTime.Now + " : AlertMail :: importMail() | StackTrace : " + ex.StackTrace);
                            writer.WriteLine("");
                            writer.Flush();
                            Console.WriteLine("");
                            mMailCustom_client = null;
                            mMailCustom_team = null;
                        }


                        if (settings.emailImport.informClient)
                        {
                            try
                            {
                                Console.WriteLine("Envoi de mail client en cours....");
                                emailManagement.EnvoiMail(settings, "client", mMailCustom_client.subject, mMailCustom_client.body, null,writer);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Envoi Mail log ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);


                                writer.WriteLine(DateTime.Now + " :: ############################### Exception Envoi Mail log ###############################");
                                writer.WriteLine(DateTime.Now + " : AlertMail :: importMail() | Message : " + ex.Message);
                                writer.WriteLine(DateTime.Now + " : AlertMail :: importMail() | StackTrace : " + ex.StackTrace);
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Envoyer un mail d'import au client est désactivé");
                        }

                        if (settings.emailImport.informTeam)
                        {
                            try
                            {
                                Console.WriteLine("Envoi de mail log en cours....");
                                emailManagement.EnvoiMail(settings, "log", mMailCustom_team.subject, mMailCustom_team.body, null,writer);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Envoi Mail log ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);


                                writer.WriteLine(DateTime.Now + " :: ############################### Exception Envoi Mail log ###############################");
                                writer.WriteLine(DateTime.Now + " : AlertMail :: importMail() | Message : " + ex.Message);
                                writer.WriteLine(DateTime.Now + " : AlertMail :: importMail() | StackTrace : " + ex.StackTrace);
                                writer.WriteLine("");
                                writer.Flush();
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Envoyer un mail d'import à l'équipe est désactivé");
                        }


                        ConfigurationCustomMailSaveLoad xxx = new ConfigurationCustomMailSaveLoad();

                        if (File.Exists(xxx.fileName_SUC_Imp))
                        {
                            File.Delete(xxx.fileName_SUC_Imp);
                        }

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Main EndSoftwareExe ***********");
                        Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                        Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);


                        writer.WriteLine(DateTime.Now + " :: ############################### Exception Main EndSoftwareExe ###############################");
                        writer.WriteLine(DateTime.Now + " : AlertMail :: importMail() | Message : " + ex.Message);
                        writer.WriteLine(DateTime.Now + " : AlertMail :: importMail() | StackTrace : " + ex.StackTrace);
                    }
                }
                else
                {
                    Console.WriteLine("Argument settings.emailImport.xxxxx n'est pas valide...");
                }
            }
            else
            {
                Console.WriteLine("Import mail est désactivé...");
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// _x_ argument (11 or 22) depends, if its after each export "11" or at the end of all export "22"
        /// 
        private static void exportMail(ConfigurationEmail settings, int _x_, StreamWriter writer)
        {
            Console.WriteLine("Export mail...");

            if (settings.emailExport.active)
            {
                writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Mail export Activé!");
                if (settings.emailExport.eachDocument && _x_ == 11)
                {
                    writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Mail export à chaque fichier n'est pas développé.");
                    writer.WriteLine("");
                    writer.Flush();
                }
                else if (settings.emailExport.atTheEnd && _x_ == 22)
                {
                    writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Mail export vers la fin du process.");
                    Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();
                    try
                    {
                        // get and load the file
                        Alert_Mail.Classes.ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new Alert_Mail.Classes.ConfigurationCustomMailSaveLoad();
                        configurationCustomMailSaveLoad.Load(configurationCustomMailSaveLoad.fileName_SUC_Exp, new List<CustomMailSuccess>());

                        writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Chargement du fichier "+configurationCustomMailSaveLoad.fileName_SUC_Exp+" avec sucess.");
                        writer.WriteLine("");
                        writer.Flush();

                        // Mapping
                        MailCustom mMailCustom_client = null;
                        MailCustom mMailCustom_team = null;
                        try
                        {
                            mMailCustom_client = emailManagement.generateMailBody("client_export", writer);
                            mMailCustom_team = emailManagement.generateMailBody("team_export", writer);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " | Main() : *********** Exception generateMailBody() ***********");
                            Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                            Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);


                            writer.WriteLine(DateTime.Now + " :: ############################### Exception generateMailBody() ###############################");
                            writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Message : " + ex.Message);
                            writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | StackTrace : " + ex.StackTrace);
                            writer.WriteLine("");
                            writer.Flush();
                            Console.WriteLine("");
                            mMailCustom_client = null;
                            mMailCustom_team = null;
                        }


                        if (settings.emailExport.informClient)
                        {
                            try
                            {
                                Console.WriteLine("Envoi de mail client en cours....");
                                writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Envoi de mail client en cours....");
                                emailManagement.EnvoiMail(settings, "client", mMailCustom_client.subject, mMailCustom_client.body, null, writer);
                                writer.WriteLine("");
                                writer.Flush();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Envoi Mail log ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);


                                writer.WriteLine(DateTime.Now + " :: ############################### Exception Envoi Mail log ###############################");
                                writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Message : " + ex.Message);
                                writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | StackTrace : " + ex.StackTrace);
                                writer.WriteLine("");
                                writer.Flush();
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Envoyer un mail d'import au client est désactivé");
                            writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Envoyer un mail d'import au client est désactivé");
                            writer.WriteLine("");
                            writer.Flush();
                        }

                        if (settings.emailExport.informTeam)
                        {
                            try
                            {
                                Console.WriteLine("Envoi de mail log en cours....");
                                writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Envoi de mail log en cours....");
                                emailManagement.EnvoiMail(settings, "log", mMailCustom_team.subject, mMailCustom_team.body, null, writer);
                                writer.WriteLine("");
                                writer.Flush();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Envoi Mail log ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);


                                writer.WriteLine(DateTime.Now + " :: ############################### Exception Envoi Mail log ###############################");
                                writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Message : " + ex.Message);
                                writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | StackTrace : " + ex.StackTrace);
                                writer.WriteLine("");
                                writer.Flush();
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Envoyer un mail d'import à l'équipe est désactivé");
                            writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Envoyer un mail d'import à l'équipe est désactivé");
                            writer.WriteLine("");
                            writer.Flush();
                        }


                        ConfigurationCustomMailSaveLoad xxx = new ConfigurationCustomMailSaveLoad();
                        if (File.Exists(xxx.fileName_SUC_Exp))
                        {
                            File.Delete(xxx.fileName_SUC_Exp);
                            writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Fichier "+xxx.fileName_SUC_Exp+" est supprimé!");
                            writer.WriteLine("");
                            writer.Flush();
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Main EndSoftwareExe ***********");
                        Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                        Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);


                        writer.WriteLine(DateTime.Now + " :: ############################### Exception Main EndSoftwareExe ###############################");
                        writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Message : " + ex.Message);
                        writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | StackTrace : " + ex.StackTrace);
                        writer.WriteLine("");
                        writer.Flush();
                    }
                }
                else
                {
                    Console.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Argument settings.emailExport.xxxxx n'est pas valide...");
                    writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Argument settings.emailExport.xxxxx n'est pas valide...");
                    writer.WriteLine("");
                    writer.Flush();
                }
            }
            else
            {
                Console.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Export mail est désactivé...");
                writer.WriteLine(DateTime.Now + " : AlertMail :: exportMail() | Export mail est désactivé...");
                writer.WriteLine("");
                writer.Flush();
            }
        }

        private static void errorMail(ConfigurationEmail settings, StreamWriter writer)
        {
            Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();
            try
            {
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | EndSoftwareExe...");
                writer.Flush();
                Console.WriteLine("EndSoftwareExe...");

                if (settings.emailError.active)
                {
                    if (settings.emailLists.emailClientList.Count == 0 && settings.emailLists.emailTeamList.Count == 0)
                    {
                        Console.WriteLine("Aucun address mail enregistré!");
                        writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | EAucun address mail enregistré!");
                        writer.Flush();
                        return;
                    }

                    MailCustom mMailCustom_client = null;
                    MailCustom mMailCustom_log = null;
                    try
                    {
                        mMailCustom_client = emailManagement.generateMailBody("client_error", writer);
                        mMailCustom_log = emailManagement.generateMailBody("log", writer);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("");
                        Console.WriteLine(DateTime.Now + " | Main() : *********** Exception generateMailBody() ***********");
                        Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                        Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);

                        writer.WriteLine(DateTime.Now + " :: ############################### Exception generateMailBody() ###############################");
                        writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Message : " + ex.Message);
                        writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | StackTrace : " + ex.StackTrace);
                        writer.WriteLine("");
                        writer.Flush();

                        Console.WriteLine("");
                        mMailCustom_client = null;
                        mMailCustom_log = null;
                    }
                    
                    writer.Flush();

                    //Envoi
                    if (settings.emailError.informClient)
                    {
                        if (mMailCustom_client != null)
                        {
                            try
                            {
                                Console.WriteLine("Envoi de mail client en cours....");
                                writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Envoi de mail client en cours....");
                                writer.Flush();
                                emailManagement.EnvoiMail(settings, "client", mMailCustom_client.subject, mMailCustom_client.body, mMailCustom_client.attachements, writer);   //cheminLogFile
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Envoi Mail Client ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);

                                writer.WriteLine(DateTime.Now + " :: ############################### Exception Envoi Mail Client ###############################");
                                writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Message : " + ex.Message);
                                writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | StackTrace : " + ex.StackTrace);
                                writer.WriteLine("");
                                writer.Flush();
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Nothing to send for the client");
                            writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Rien à envoie vers le client");
                            writer.Flush();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Send mail to client is desable");
                        writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Envoie mail client est désactivité");
                        writer.Flush();
                    }

                    if (settings.emailError.informTeam)
                    {
                        if (mMailCustom_client != null)
                        {
                            try
                            {
                                Console.WriteLine("Envoi de mail log en cours....");
                                writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Envoi de mail log en cours....");
                                writer.Flush();
                                emailManagement.EnvoiMail(settings, "log", mMailCustom_log.subject, mMailCustom_log.body, mMailCustom_log.attachements, writer);   //cheminLogFile
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Envoi Mail log ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);

                                writer.WriteLine(DateTime.Now + " :: ############################### Exception Envoi Mail log ###############################");
                                writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Message : " + ex.Message);
                                writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | StackTrace : " + ex.StackTrace);
                                writer.WriteLine("");
                                writer.Flush();
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No log to send for the team");
                            writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Rien à envoyer au team");
                            writer.Flush();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Send log mail to team is desable");
                        writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Envoie mail log au Team");
                        writer.Flush();
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
                    writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Les mails d'erreur sont désactivé!");
                    writer.Flush();
                }

                writer.WriteLine("");
                writer.Flush();

                //stop at the end of the program
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Main EndSoftwareExe ***********");
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);


                writer.WriteLine(DateTime.Now + " :: ############################### Exception Main EndSoftwareExe ###############################");
                writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | Message : " + ex.Message);
                writer.WriteLine(DateTime.Now + " : AlertMail :: errorMail() | StackTrace : " + ex.StackTrace);
                writer.WriteLine("");
                writer.Flush();
            }
        }

        private static void summaryMail(ConfigurationSaveLoad file, ConfigurationEmail settings, StreamWriter writer)
        {
            Console.WriteLine("Résumer mail...");
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Creation d'une instance.");
            writer.Flush();

            try
            {
                if (settings.emailSummary.active)
                {
                    Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();

                    string directoryName_tmpFile = Directory.GetCurrentDirectory() + @"\" + "tmp";
                    if (!Directory.Exists(directoryName_tmpFile))
                    {
                        Directory.CreateDirectory(directoryName_tmpFile);
                        writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Ce répertoire "+ directoryName_tmpFile + " n'existe pas , alors créer le.");
                    }

                    DirectoryInfo fileListing1 = new DirectoryInfo(directoryName_tmpFile);
                    FileInfo[] allFiles_error = fileListing1.GetFiles("*.csv");

                    writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Il y a " + allFiles_error.Length + " fichier(s) dans le répertoire.");

                    string[,] errorFilesFileNameList_ = new string[allFiles_error.Length, 2];
                    List<string> errorFilesFileNameList = new List<string>();
                    List<string> errorFilesFileList = new List<string>();

                    DateTime today = DateTime.Now;
                    DateTime lastActivated = settings.emailSummary.lastActivated;
                    TimeSpan ts = today - lastActivated;

                    Console.WriteLine("Today : " + today);
                    Console.WriteLine("LastActivated : " + lastActivated);
                    Console.WriteLine("Ago : " + ts);

                    writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Date aujourd'hui => " + today);
                    writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Dernière fois lancé => " + lastActivated);
                    writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Tempsde lancement depuis la dernière fois => "+ts.TotalHours+" heure(s)");
                    writer.Flush();

                    if (ts.TotalHours >= Convert.ToDouble(settings.emailSummary.hours))
                    {
                        Console.WriteLine("Envoi un Mail Résumer en cours...");
                        writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Envoi un Mail Résumer en cours...");

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
                                    emailManagement.EnvoiMail(settings, "log", "Résumer [" + dns + "]", "Bonjour Team BDC,\n\n" + infoBody + "\n\nCordialement,\nConnecteur SAGE [" + dns + "].", attachements, writer);
                                    
                                    writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Mail Envoyé => Résumer [" + dns + "]");
                                    writer.Flush();
                                }

                                Console.WriteLine("No Mail_Recap.ml File!");

                                // the DateTime
                                settings.emailSummary.lastActivated = DateTime.Now;
                                file.configurationEmail = settings;
                                file.saveInfo();
                                writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Reset lastActivated to " + settings.emailSummary.lastActivated);
                                writer.Flush();
                            }
                            //cMail.remaningTicks = cMail.totalTicks;
                            //cMail.password = Utils.Encrypt(cMail.password);
                            //new ConfSendMail().saveInfo(cMail);

                        }
                    }
                    else
                    {
                        Console.WriteLine("Envoi des mails de résumer sont désactivé!");
                        writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Envoi des mails de résumer sont désactivé!");
                        writer.Flush();
                    }

                    //stop at the end of the program

                }//Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Main CheckErrorFiles ***********");
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);

                writer.WriteLine(DateTime.Now + " :: ############################### Exception EnvoiMail ###############################");
                writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | Message : " + ex.Message);
                writer.WriteLine(DateTime.Now + " : AlertMail :: summaryMail() | StackTrace : " + ex.StackTrace);
                writer.WriteLine("");
                writer.Flush();
            }
        }

        private static void pendingFilesMail(ConfigurationEmail settings, StreamWriter writer)
        {
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Creation d'une instance.");
            if (settings.emailPendingFiles.active)
            {
                Console.WriteLine("Analyser le dossier CSV....");
                writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Analyser le dossier CSV....");

                string directoryName_csv = Directory.GetCurrentDirectory() + @"\" + "CSV";
                if (!Directory.Exists(directoryName_csv))
                {
                    Directory.CreateDirectory(directoryName_csv);
                    writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Le répertoire " + directoryName_csv + " n'existe pas, alors créer le.");
                }

                Alert_Mail.EmailManagement emailManagement = new Alert_Mail.EmailManagement();

                DirectoryInfo fileListing1 = new DirectoryInfo(directoryName_csv);
                FileInfo[] allFiles_csv = fileListing1.GetFiles("*.csv");

                List<string> fileNameAttachmentList = new List<string>();

                writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Dans le répertoire " + directoryName_csv+" il y a "+allFiles_csv.Length+" en attante....");

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

                        Console.WriteLine("Today : " + today);
                        Console.WriteLine("File DateTime : " + fileDateTime);
                        Console.WriteLine("Ago : " + ts);

                        writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Index " + (x+1)+"/"+allFiles_csv.Length);
                        writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Fichier => " + allFiles_csv[0].Name);
                        writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Fichier DateTime => " + fileDateTime);
                        writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Fichier Existance => " + ts.TotalHours + " Heures");
                        writer.Flush();

                        int lines = 0;
                        if (ts.TotalHours >= Convert.ToDouble(settings.emailPendingFiles.hours))
                        {
                            ok = true;
                            fileNameAttachmentList.Add(allFiles_csv[x].FullName);
                            body += "\t - " + (x + 1) + " Nom : " + allFiles_csv[x].Name + " , Date de creation : " + string.Format("{0:yyyy/MM/dd HH:mm}", allFiles_csv[x].CreationTime) + ", Date d'aujourd'huid : " + string.Format("{0:yyyy/MM/dd HH:mm}", today) + " , la durée d'existance : " + string.Format("{0}", ts) + " heure(s). \n";
                            lines++;
                        }
                        writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Nombre de fichier => " + lines);
                        writer.Flush();
                    }

                    if (ok)
                    {
                        header = "Bonjour\n\nIl y a " + fileNameAttachmentList.Count + " fichier(s) en attente depuis " + string.Format("{0:dddd, d MMMM , yyyy à HH:mm}", today.AddHours(-settings.emailPendingFiles.hours)) + " : \n";
                        Alert_Mail.EmailManagement manager = new Alert_Mail.EmailManagement();

                        manager.EnvoiMail(settings, "log", "[" + emailManagement.getDNS() + "] Scan du dossier CSV", header + body + "\nCordialement,\nConnecteur SAGE version équipe.", fileNameAttachmentList, writer);
                        writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Mail envoyé!");
                        writer.WriteLine("");
                    }
                    else
                    {
                        Console.WriteLine("Aucun mail envoyé.");
                        writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Aucun mail envoyé.");
                    }

                }
                else
                {
                    Console.WriteLine("Aucun fichier EDI trouvé !");
                    writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Aucun fichier EDI trouvé !");
                }
            }
            else
            {
                Console.WriteLine("Les mails des fichiers sont en attente");
                writer.WriteLine(DateTime.Now + " : AlertMail :: pendingFilesMail() | Les mails des fichiers sont en attente");
            }
            writer.WriteLine("");
            writer.Flush();
        }
    }
}
