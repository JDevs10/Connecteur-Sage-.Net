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
                            if(cMail.remaningTicks == 0)
                            {
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
                    Console.WriteLine(DateTime.Now + " | Main() : *********** Exception Main ***********");
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
            MailCustom mailResult = null;
            bool sendMailImp = false;
            string textImp = "";
            bool sendMailExp = false;
            string textExp = "";
            List<string> attachements = new List<string>();

            //check if the file exist
            if (File.Exists(localPath + @"\Mail_IMP.ml"))
            {
                //read all the lines in the file
                List<string> lines = File.ReadAllLines(localPath + @"\Mail_IMP.ml").ToList();
                string client = "";
                string subject = "";
                string date = "";
                string time = "";
                int numOfDocs = 0;

                //get the number of lines, the client in the header and the mail subject in the header
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Split(';')[0] == "Mail_IMP")
                    {
                        client = lines[i].Split(';')[1];
                        subject = lines[i].Split(';')[2];
                        date = lines[i].Split(';')[3];
                        time = lines[i].Split(';')[4];
                    }
                    if (lines[i].Split(';')[0] == "L")
                    {
                        numOfDocs++;
                    }
                }

                //make the following body message
                if (numOfDocs == 0)
                {
                    sendMailImp = false;
                }
                else if (numOfDocs == 1)
                {
                    sendMailImp = true;
                    textImp += "L'import d'un document commercial durant l'import a échoué. Voici un résumer du document échoué :\n\n";
                }
                else if (numOfDocs > 1)
                {
                    sendMailImp = true;
                    textImp += "L'import de plusieurs documents commerciaux durant l'import ont échoué. Voici le résumer de chaque document échoué :\n\n";
                }

                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Split(';')[0] == "L")
                    {
                        string line = lines[i].Split(';')[1];
                        string reference_doc = lines[i].Split(';')[2];
                        string msg_erreur_doc = lines[i].Split(';')[3];
                        string logFilePath = lines[i].Split(';')[4];
                        if (!attachements.Contains(logFilePath))
                        {
                            Console.WriteLine("Attachement : " + logFilePath);
                            attachements.Add(logFilePath);
                        }
                        textImp += line + "- Le numéro du document \"" + reference_doc + "\"\nMessage erreur : " + msg_erreur_doc + "\nL'erreur peut etre trouvé dans " + logFilePath + "\n\n";
                    }
                }
            }

            //check if the file exist
            if (File.Exists(localPath + @"\Mail_EXP.ml"))
            {
                //read all the lines in the file
                List<string> lines = File.ReadAllLines(localPath + @"\Mail_EXP.ml").ToList();
                string client = "";
                string subject = "";
                string date = "";
                string time = "";
                int numOfDocs = 0;

                //get the number of lines, the client in the header and the mail subject in the header
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Split(';')[0] == "Mail_EXP")
                    {
                        client = lines[i].Split(';')[1];
                        subject = lines[i].Split(';')[2];
                        date = lines[i].Split(';')[3];
                        time = lines[i].Split(';')[4];
                    }
                    if (lines[i].Split(';')[0] == "L")
                    {
                        numOfDocs++;
                    }
                }

                //make the following body message
                if (numOfDocs == 0)
                {
                    sendMailExp = false;
                }
                else if (numOfDocs == 1)
                {
                    sendMailExp = true;
                    textExp += "L'export d'un document commercial durant l'export a échoué. Voici un résumer du document échoué :\n\n";
                }
                else if (numOfDocs > 1)
                {
                    sendMailExp = true;
                    textExp += "L'export de plusieurs documents commerciaux durant l'export ont échoué. Voici le résumer de chaque document échoué :\n\n";
                }

                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Split(';')[0] == "L")
                    {
                        string line = lines[i].Split(';')[1];
                        string reference_doc = lines[i].Split(';')[2];
                        string msg_erreur_doc = lines[i].Split(';')[3];
                        string logFilePath = lines[i].Split(';')[4];
                        if (!attachements.Contains(logFilePath))
                        {
                            Console.WriteLine("Attachement : " + logFilePath);
                            attachements.Add(logFilePath);
                        }
                        textExp += line + "- Le numéro du document \"" + reference_doc + "\"\nMessage erreur : " + msg_erreur_doc + "\nL'erreur peut etre trouvé dans " + logFilePath + "\n\n";
                    }
                }
            }

            //send the recap mail
            if (sendMailImp & !sendMailExp)
            {
                return new MailCustom("Erreur Import [Connecteur Sage]", "Bonjour Team BDC, \n\n" + textImp + "\nCordialement,\nConnecteur SAGE.", attachements);
            }
            else if (sendMailExp & !sendMailImp)
            {
                return new MailCustom("Erreur Export [Connecteur Sage]", "Bonjour Team BDC, \n\n" + textExp + "\nCordialement,\nConnecteur SAGE.", attachements);
            }
            else if (sendMailImp && sendMailExp)
            {
                return new MailCustom("Erreur Import et Export [Connecteur Sage]", "Bonjour, \n\n" + textImp + "\n\n\n" + textExp + "\nCordialement,\nConnecteur SAGE.", attachements);
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
                    for(int i=0; i < attachements.Count; i++)
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
                Console.ReadLine();
            }
        }
    }
}
