using AlertMail.Classes;
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
using AlertMail.Helpers;

namespace AlertMail
{
    class Program
    {
        private static string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        static void Main(string[] args)
        {
            if (args[0] == "EndSoftwareExe")
            {
                try
                {
                    Console.WriteLine("EndSoftwareExe...");
                    ConfSendMail cMail = getInfoMail();
                    if (cMail != null)
                    {
                        if (cMail.active)
                        {
                            if (cMail.dest1 == "" && cMail.dest2 == "" && cMail.dest3 == "")
                            {
                                Console.WriteLine("Send Mail..Erreur Adresse de distinataire");
                            }

                            MailCustom mMailCustom = null;
                            try
                            {
                                mMailCustom = generateMailBody();
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception generateMailBody() ***********");
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);
                                Console.WriteLine("");
                                mMailCustom = null;
                            }

                            //Envoi
                            if(mMailCustom != null)
                            {
                                Console.WriteLine("Envoi de mail en cours....");
                                EnvoiMail(cMail, mMailCustom.subject, mMailCustom.body, mMailCustom.attachements);   //cheminLogFile

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
                            Console.WriteLine("Envoie de mail est déactivé!!!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Aucune Configuration Mail enregisté!");
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
            else if (args[0] == "CheckErrorFiles")
            {
                Console.WriteLine("CheckErrorFiles...");
                //Get all .csv files in the folder
                Classes.Path path = new Classes.Path();
                path.Load();
                string directoryName_ErrorFile = Directory.GetCurrentDirectory() + @"\" + "Error File";
                if (!Directory.Exists(path.path))
                {
                    Directory.CreateDirectory(path.path);
                }
                if (!Directory.Exists(directoryName_ErrorFile))
                {
                    Directory.CreateDirectory(directoryName_ErrorFile);
                }

                DirectoryInfo fileListing = new DirectoryInfo(path.path);
                FileInfo[] allFiles_path = fileListing.GetFiles("*.csv");

                DirectoryInfo fileListing1 = new DirectoryInfo(directoryName_ErrorFile);
                FileInfo[] allFiles_error = fileListing1.GetFiles("*.csv");

                List<string> mailRecapFileNameList = new List<string>();
                List<string> errorFilesFileNameList = new List<string>();

                try
                {
                    ConfSendMail cMail = getInfoMail();
                    if (cMail != null)
                    {
                        if (cMail.active)
                        {
                            if (cMail.remaningTicks == 0)
                            {
                                Console.WriteLine("Envoi de mail en cours V2....");
                                if (File.Exists("Mail_Recap.ml"))
                                {
                                    if (cMail.dest1 == "" && cMail.dest2 == "" && cMail.dest3 == "")
                                    {
                                        Console.WriteLine("Send Mail..Erreur Adresse de distinataire");
                                    }

                                    CustomMailRecap recap = new CustomMailRecap();
                                    recap.Load("Mail_Recap.ml");
                                    string infoBody_end = "";
                                    string infoBody1 = "";
                                    string infoBody2 = "";
                                    string infoBodyHeader1 = "";
                                    string infoBodyHeader2 = "";
                                    
                                    //import csv folder
                                    if (allFiles_path.Length > 0)
                                    {
                                        infoBodyHeader1 += "Il y a " + allFiles_path.Length + " fichier(s) qui n'ont pas été traités et sont dans le répertoire '" + path.path + "' :\n";

                                        for (int x = 0; x < allFiles_path.Length; x++)
                                        {
                                            infoBody1 += "\tNom du fichier : " + allFiles_path[x].Name + "\n";
                                        }
                                    }

                                    //import file error
                                    if (allFiles_error.Length > 0)
                                    {
                                        infoBodyHeader2 += "Il y a " + allFiles_error.Length + " fichier(s) qui sont tombé en erreur lors de l'import, qui sont dans le le répertoire '" + directoryName_ErrorFile + "' :\n";

                                        for (int y = 0; y < recap.Lines.Count; y++)
                                        {
                                            infoBody2 += recap.Lines[y].LineNumber + " -\t Le fichier EDI : " + recap.Lines[y].FileName + "\nLe numéro du document \"" + recap.Lines[y].DocumentReference + "\"\nMessage erreur : " + recap.Lines[y].DocumentErrorMessage + "\nStackTrace: " + recap.Lines[y].DocumentErrorStackTrace + "\nL'erreur peut etre trouvé dans " + recap.Lines[y].FilePath + "\n\n";
                                        }

                                        for (int x = 0; x < allFiles_error.Length; x++)
                                        {
                                            infoBody2 += "\t - " + allFiles_error[x].Name + "\n";
                                        }

                                        /*
                                        CustomMailRecap mailRecap = new CustomMailRecap();
                                        mailRecap.Load("Mail_Recap.ml");

                                        for (int x = 0; x < allFiles_error.Length; x++)
                                        {
                                            errorFilesFileNameList.Add(allFiles_error[x].Name);
                                        }

                                        int a = 0;
                                        List<string> unknownFile = new List<string>();
                                        for (int y = 0; y < mailRecap.Lines.Count; y++)
                                        {
                                            Console.WriteLine(y + " - FileInfo : " + errorFilesFileNameList[y]);
                                            if (errorFilesFileNameList.Contains(mailRecap.Lines[y].FileName))
                                            {
                                                Console.WriteLine("y: " + y + " || FileInfo : " + allFiles_error[y].Name + " == Mail Recap : " + allFiles_error[y].Name);
                                                infoBody2 += (y + 1) + " -\t Le numéro du document \"" + mailRecap.Lines[y].DocumentReference + "\"\nNom du fichier : " + mailRecap.Lines[y].FileName + "\nMessage erreur : " + mailRecap.Lines[y].DocumentErrorMessage + "\nStackTrace: " + mailRecap.Lines[y].DocumentErrorStackTrace + "\nL'erreur peut etre trouvé dans " + mailRecap.Lines[y].FilePath + "\n\n";
                                                a++;
                                            }
                                            else
                                            {
                                                unknownFile.Add(errorFilesFileNameList[y]);
                                            }
                                        }

                                        if (errorFilesFileNameList.Count > a)
                                        {
                                            infoBody2 += "Voici d'autre fichier en erreur, ils n'ont pas de log générés à partir du connecteur :\n";
                                            for (int x = 0; x < allFiles_path.Length; x++)
                                            {
                                                infoBody1 += "\tNom du fichier : " + allFiles_path[x].Name + "\n";
                                            }
                                        }
                                        */
                                    }

                                    if (allFiles_path.Length > 0 & allFiles_error.Length == 0)
                                    {
                                        infoBody_end += "Bonjour Team BDC, \n\nVoici un récapitulatif des documents. \n" + infoBodyHeader1 + infoBody1;
                                    }
                                    else if (allFiles_error.Length > 0 & allFiles_path.Length == 0)
                                    {
                                        infoBody_end += "Bonjour Team BDC, \n\nVoici un récapitulatif des documents. \n" + infoBodyHeader2 + infoBody2;
                                    }
                                    else if (allFiles_path.Length > 0 && allFiles_error.Length > 0)
                                    {
                                        infoBody_end += "Bonjour Team BDC, \n\nVoici un récapitulatif des documents. \n" + infoBodyHeader1 + infoBody1 + "\n" + infoBodyHeader2 + infoBody2;
                                    }
                                    else
                                    {
                                        infoBody_end += "Bonjour Team BDC,\n\nIl n'y a pas de fichier EDI en attente ou en erreur.\n";
                                    }

                                    EnvoiMail(cMail, recap.Subject, infoBody_end + "\nCordialement,\nConnecteur SAGE.", recap.Attachments);

                                    cMail.remaningTicks = cMail.totalTicks;
                                    cMail.password = Utils.Encrypt(cMail.password);
                                    cMail.saveInfo(cMail);

                                    //delete recap file
                                    if (File.Exists("Mail_Recap.ml"))
                                    {
                                        File.Delete("Mail_Recap.ml");
                                    }
                                }
                                else
                                {
                                    //import file error
                                    if (allFiles_error.Length > 0)
                                    {
                                        ConfigurationDNS dns = new ConfigurationDNS();
                                        dns.LoadSQL();
                                        string infoBody = "Bonjour Team BDC,\n\nIl y a " + allFiles_error.Length + " fichier(s) qui sont dans le le répertoire erreur '" + directoryName_ErrorFile + "' :\n";

                                        for (int x = 0; x < allFiles_error.Length; x++)
                                        {
                                            infoBody += "\t - " + allFiles_error[x].Name + "\n";
                                        }

                                        EnvoiMail(cMail, "Erreur ["+dns.Prefix+"]", infoBody + "\n\nCordialement,\nConnecteur SAGE.", null);

                                        cMail.remaningTicks = cMail.totalTicks;
                                        cMail.password = Utils.Encrypt(cMail.password);
                                        cMail.saveInfo(cMail);
                                    }

                                    Console.WriteLine("No Mail_Recap.ml File!");
                                }
                            }
                            else
                            {
                                cMail.remaningTicks = cMail.remaningTicks - 1;
                                cMail.password = Utils.Encrypt(cMail.password);
                                cMail.saveInfo(cMail);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Envoie de mail est déactivé!!!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Aucune Configuration Mail enregisté!");
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
            else
            {
                Console.WriteLine("Argument inconnue!!!");
            }
            //Console.ReadLine();
        }

        public static MailCustom generateMailBody()
        {
            ConfigurationDNS dns = new ConfigurationDNS();
            dns.LoadSQL();
            CustomMailRecap recap_imp = null;
            CustomMailRecap recap_exp = null;
            //MailCustom mailResult = null;
            bool sendMailImp = false;
            string textImp = "";
            bool sendMailExp = false;
            string textExp = "";
            List<string> attachements = new List<string>();

            //check if the file exist
            if (File.Exists("Mail_IMP.ml"))
            {
                recap_imp = new CustomMailRecap();
                recap_imp.Load("Mail_IMP.ml");

                //make the following body message
                if (recap_imp.Lines.Count == 0)
                {
                    sendMailImp = false;
                }
                else if (recap_imp.Lines.Count == 1)
                {
                    sendMailImp = true;
                    textImp += "L'import d'un document commercial durant l'import a échoué. Voici un résumer du document échoué :\n";
                }
                else if (recap_imp.Lines.Count > 1)
                {
                    sendMailImp = true;
                    textImp += "L'import de plusieurs documents commerciaux durant l'import ont échoué. Voici le résumer de chaque document échoué :\n";
                }
                for (int i = 0; i < recap_imp.Lines.Count; i++)
                {
                    textImp += (i+1) + " -\t Le numéro du document \"" + recap_imp.Lines[i].DocumentReference + "\"\nNom du fichier : "+recap_imp.Lines[i].FileName+"\nMessage erreur : " + recap_imp.Lines[i].DocumentErrorMessage + "\nStackTrace: "+ recap_imp.Lines[i] .DocumentErrorStackTrace+ "\nL'erreur peut etre trouvé dans " + recap_imp.Lines[i].FilePath + "\n\n";
                    if (!attachements.Contains(recap_imp.Lines[i].FilePath))
                    {
                        attachements.Add(recap_imp.Lines[i].FilePath);
                    }
                }
                recap_imp.Attachments = attachements;
                attachements.Clear();
            }

            //check if the file exist
            if (File.Exists("Mail_EXP.ml"))
            {
                attachements.Clear();
                recap_exp = new CustomMailRecap();
                recap_exp.Load("Mail_EXP.ml");

                //make the following body message
                if (recap_exp.Lines.Count == 0)
                {
                    sendMailImp = false;
                }
                else if (recap_exp.Lines.Count == 1)
                {
                    sendMailImp = true;
                    textImp += "L'import d'un document commercial durant l'import a échoué. Voici un résumer du document échoué :\n";
                }
                else if (recap_exp.Lines.Count > 1)
                {
                    sendMailImp = true;
                    textImp += "L'import de plusieurs documents commerciaux durant l'import ont échoué. Voici le résumer de chaque document échoué :\n";
                }
                for (int i = 0; i < recap_exp.Lines.Count; i++)
                {
                    textImp += (i + 1) + " -\t Le numéro du document \"" + recap_exp.Lines[i].DocumentReference + "\"\nNom du fichier : " + recap_exp.Lines[i].FileName + "\nMessage erreur : " + recap_exp.Lines[i].DocumentErrorMessage + "\nStackTrace: " + recap_exp.Lines[i].DocumentErrorStackTrace + "\nL'erreur peut etre trouvé dans " + recap_exp.Lines[i].FilePath + "\n\n";
                    if (!attachements.Contains(recap_exp.Lines[i].FilePath))
                    {
                        attachements.Add(recap_exp.Lines[i].FilePath);
                    }
                }
                recap_exp.Attachments = attachements;
                attachements.Clear();
            }

            //send the recap mail
            if (sendMailImp & !sendMailExp)
            {
                return new MailCustom("["+dns.Prefix+"] " + recap_imp.Subject, "Bonjour Team BDC, \n\n" + textImp + "\nCordialement,\nConnecteur SAGE.", recap_imp.Attachments);
            }
            else if (sendMailExp & !sendMailImp)
            {
                return new MailCustom("[" + dns.Prefix + "] " + recap_exp.Subject, "Bonjour Team BDC, \n\n" + textExp + "\nCordialement,\nConnecteur SAGE.", recap_exp.Attachments);
            }
            else if (sendMailImp && sendMailExp)
            {
                attachements.Clear();
                attachements.AddRange(recap_imp.Attachments);
                attachements.AddRange(recap_exp.Attachments);
                return new MailCustom("[" + dns.Prefix + "] " + recap_imp.Subject + " et "+ recap_exp.Subject, "Bonjour, \n\n" + textImp + "\n\n\n" + textExp + "\nCordialement,\nConnecteur SAGE.", attachements);
            }
            else
            {
                return null;
            }
        }

        public static ConfSendMail getInfoMail()
        {
            try
            {
                if (File.Exists(localPath + @"\SettingMail.xml"))
                {
                    ConfSendMail mail = new ConfSendMail();
                    mail.Load();
                    return mail;
                }
                else
                {
                    Console.WriteLine("SettingMail.xml not found!");
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " : Erreur[43] - " + ex.Message);

                return null;
            }
        }

        public static void EnvoiMail(ConfSendMail confMail, string subject, string body, List<string> attachements)
        {
            try
            {
                /*
                MailMessage msg = new MailMessage();
                msg.To.Add(confMail.dest1);
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = false;
                msg.From = new MailAddress(confMail.login);
                SmtpClient smtp = new SmtpClient(confMail.smtp);
                smtp.Port = confMail.port;
                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(confMail.login, confMail.password);
                smtp.Send(msg);
                */

                // Objet mail
                MailMessage msg = new MailMessage();

                // Expéditeur (obligatoire). Notez qu'on peut spécifier le nom
                msg.From = new MailAddress(confMail.login, "CONNECTEUR SAGE");

                // Destinataires (il en faut au moins un)
                if (!string.IsNullOrEmpty(confMail.dest1))
                {
                    msg.To.Add(new MailAddress(confMail.dest1, confMail.dest1));
                }
                if (!string.IsNullOrEmpty(confMail.dest2))
                {
                    msg.To.Add(new MailAddress(confMail.dest2, confMail.dest2));
                }
                if (!string.IsNullOrEmpty(confMail.dest3))
                {
                    msg.To.Add(new MailAddress(confMail.dest3, confMail.dest3));
                }
                msg.Subject = subject;

                // Texte du mail (facultatif)
                msg.Body = body;

                // Fichier joint si besoin (facultatif)
                if (attachements != null && attachements.Count > 0)
                {
                    for (int i = 0; i < attachements.Count; i++)
                    {
                        msg.Attachments.Add(new Attachment(attachements[i]));
                    }
                }

                SmtpClient client;

                if (confMail.smtp != "" && confMail.login != "" && confMail.password != "")
                {
                    // Envoi du message SMTP
                    client = new SmtpClient(confMail.smtp, confMail.port);
                    client.Credentials = new NetworkCredential(confMail.login, confMail.password);
                }
                else
                {
                    client = new SmtpClient("smtp.gmail.com", 587);
                    client.Credentials = new NetworkCredential("connecteur.sage@gmail.com", "@Amyaj2013");
                }

                client.EnableSsl = true;
                //NetworkInformation s = new NetworkCredential();
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                // Envoi du mail
                client.Send(msg);

                Console.WriteLine(DateTime.Now + " : Envoi de Mail..OK");

            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + " : " + e.Message);
                //Console.ReadLine();
            }
        }
    }
}
