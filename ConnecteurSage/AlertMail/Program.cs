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

                            MailCustom mMailCustom = generateMailBody();

                            //Envoi
                            Console.WriteLine("Envoi de mail en cours....");
                            EnvoiMail(cMail, mMailCustom.subject, mMailCustom.body, mMailCustom.attachements);   //cheminLogFile
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
                    Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Main ***********");
                    Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                }
            }
            else if (args[0] == "CheckErrorFiles")
            {
                Console.WriteLine("CheckErrorFiles...");
                //Get all .csv files in the folder
                string directoryName_ErrorFile = Directory.GetCurrentDirectory() + @"\" + "Error File";
                if (!Directory.Exists(directoryName_ErrorFile))
                {
                    Directory.CreateDirectory(directoryName_ErrorFile);
                }
                DirectoryInfo fileListing = new DirectoryInfo(directoryName_ErrorFile);
                FileInfo[] allFiles = fileListing.GetFiles("*.csv");

                try
                {
                    ConfSendMail cMail = getInfoMail();
                    if (cMail != null)
                    {
                        if (cMail.active)
                        {
                            if (cMail.remaningTicks == 0)
                            {
                                /*
                                if (cMail.dest1 == "" && cMail.dest2 == "" && cMail.dest3 == "")
                                {
                                    Console.WriteLine("Send Mail..Erreur Adresse de distinataire");
                                }

                                string filenames = "";
                                for (int index = 0; index < allFiles.Length; index++)
                                {
                                    filenames += allFiles[index].Name + "\n";
                                }
                                //Envoi
                                Console.WriteLine("Envoi de mail en cours....");
                                EnvoiMail(cMail, "Document(s) d'import en Erreur!!!",
                                    "Bonjour Team BDC, \n\n" +
                                    "Il y a " + allFiles.Length + " fichier(s) qui n'ont pas été traités.Voici les noms de fichiers du répertoire '" + directoryName_ErrorFile + "' non traités:\n" +
                                    filenames +
                                    "\nCordialement,\nConnecteur SAGE.", null);
                                cMail.remaningTicks = cMail.totalTicks;
                                cMail.saveInfo(cMail);
                                */

                                Console.WriteLine("Envoi de mail en cours V2....");
                                if (File.Exists("Mail_Recap.ml"))
                                {
                                    if (cMail.dest1 == "" && cMail.dest2 == "" && cMail.dest3 == "")
                                    {
                                        Console.WriteLine("Send Mail..Erreur Adresse de distinataire");
                                    }

                                    CustomMailRecap recap = new CustomMailRecap();
                                    recap.Load("Mail_Recap.ml");
                                    string infoBody = "";

                                    /*
                                    for (int x = 0; x < recap.Lines.Count; x++)
                                    {
                                        infoBody += recap.Lines[x].LineNumber + "*\tLe document \"" + recap.Lines[x].DocumentReference + "\" est tombé en erreur " + recap.Lines[x].Increment + " fois. Avec une erreur \"" + recap.Lines[x].DocumentErrorMessage + "\".\n" +
                                                    "\tStackTrace : " + recap.Lines[x].DocumentErrorStackTrace + "\n\n";
                                    }
                                    */

                                    for (int x = 0; x < allFiles.Length; x++)
                                    {
                                        for(int y = 0; y < allFiles.Length; y++)
                                        {
                                            if (allFiles[x].Name.Contains(recap.Lines[y].FilePath))
                                            {
                                                infoBody += recap.Lines[y].LineNumber + "*\tLe document \"" + recap.Lines[y].DocumentReference + "\" est tombé en erreur " + recap.Lines[y].Increment + " fois. Avec une erreur \"" + recap.Lines[y].DocumentErrorMessage + "\".\n" +
                                                    "\tStackTrace : " + recap.Lines[y].DocumentErrorStackTrace + "\n\n";
                                                break;
                                            }
                                        }
                                    }

                                    EnvoiMail(cMail, recap.Subject,
                                    "Bonjour Team BDC, \n\n" +
                                    "Voici un récapitulatif des documents toujours en erreur. \nIl y a " + recap.Lines.Count + " fichier(s) qui n'ont pas été traités et sont dans le répertoire 'Error File' :\n" +
                                    infoBody +
                                    "\nCordialement,\nConnecteur SAGE.", recap.Attachments);

                                    cMail.remaningTicks = cMail.totalTicks;
                                    cMail.saveInfo(cMail);
                                }
                                else
                                {
                                    Console.WriteLine("No Mail_Recap.ml File!");
                                }
                            }
                            else
                            {
                                cMail.remaningTicks = cMail.remaningTicks - 1;
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
                }
            }
            else
            {
                Console.WriteLine("Argument inconnue!!!");
            }
        }

        public static MailCustom generateMailBody()
        {
            CustomMailRecap recap_imp = null;
            CustomMailRecap recap_exp = null;
            MailCustom mailResult = null;
            bool sendMailImp = false;
            string textImp = "";
            bool sendMailExp = false;
            string textExp = "";
            List<string> attachements = new List<string>();

            //check if the file exist
            if (File.Exists(localPath + @"\Mail_IMP.ml"))
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
                    textImp += recap_imp.Lines[i].LineNumber + "-\t Le numéro du document \"" + recap_imp.Lines[i].DocumentReference + "\"\nMessage erreur : " + recap_imp.Lines[i].DocumentErrorMessage + "\nStackTrace: "+ recap_imp.Lines[i] .DocumentErrorStackTrace+ "\nL'erreur peut etre trouvé dans " + recap_imp.Lines[i].FilePath + "\n\n";
                }
            }

            //check if the file exist
            if (File.Exists(localPath + @"\Mail_EXP.ml"))
            {
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
                    textImp += recap_exp.Lines[i].Increment + "-\t Le numéro du document \"" + recap_exp.Lines[i].DocumentReference + "\"\nMessage erreur : " + recap_exp.Lines[i].DocumentErrorMessage + "\nStackTrace: " + recap_exp.Lines[i].DocumentErrorStackTrace + "\nL'erreur peut etre trouvé dans " + recap_exp.Lines[i].FilePath + "\n\n";
                }
            }

            //send the recap mail
            if (sendMailImp & !sendMailExp)
            {
                return new MailCustom(recap_imp.Subject, "Bonjour Team BDC, \n\n" + textImp + "\nCordialement,\nConnecteur SAGE.", recap_imp.Attachments);
            }
            else if (sendMailExp & !sendMailImp)
            {
                return new MailCustom(recap_exp.Subject, "Bonjour Team BDC, \n\n" + textExp + "\nCordialement,\nConnecteur SAGE.", recap_exp.Attachments);
            }
            else if (sendMailImp && sendMailExp)
            {
                return new MailCustom(recap_imp.Subject + " et "+ recap_exp.Subject, "Bonjour, \n\n" + textImp + "\n\n\n" + textExp + "\nCordialement,\nConnecteur SAGE.", recap_exp.Attachments);
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
