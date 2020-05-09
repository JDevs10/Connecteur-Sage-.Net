using AlertMail.Classes;
using AlertMail.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail
{
    public class Class1
    {
        private static string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public static MailCustom generateMailBody(string type)
        {
            if (type.Equals("client"))
            {
                ConfigurationDNS dns = new ConfigurationDNS();
                dns.Load();
                CustomMailRecap recap_imp = null;
                CustomMailRecap recap_exp = null;
                bool sendMailImp = false;
                string textImp = "";
                bool sendMailExp = false;
                string textExp = "";
                List<string> attachements = new List<string>();

                //check if the file exist
                if (File.Exists("Mail_IMP.ml"))
                {
                    recap_imp = new CustomMailRecap();
                    recap_imp.Lines = new List<CustomMailRecapLines>();
                    recap_imp.Load("Mail_IMP.ml");



                    //make the following body message
                    if (recap_imp.Lines.Count == 0)
                    {
                        sendMailImp = false;
                    }
                    else if (recap_imp.Lines.Count == 1)
                    {
                        sendMailImp = true;
                        textImp += "L'import d'un document commercial a échoué. Voici un résumé du document en erreur :\n";
                    }
                    else if (recap_imp.Lines.Count > 1)
                    {
                        sendMailImp = true;
                        textImp += "L'import de plusieurs documents commerciaux ont échoué. Voici le résumé de chaque document en erreur :\n";
                    }
                    for (int i = 0; i < recap_imp.Lines.Count; i++)
                    {
                        textImp += (i + 1) + " -\t Le numéro du document \"" + recap_imp.Lines[i].DocumentReference + "\" de la commande \"" + recap_imp.Lines[i].NumCommande + "\",\n" +
                            " \t a une erreur : " + recap_imp.Lines[i].DocumentErrorMessage + "\n";
                    }
                }

                //check if the file exist
                if (File.Exists("Mail_EXP.ml"))
                {
                    recap_exp = new CustomMailRecap();
                    recap_exp.Lines = new List<CustomMailRecapLines>();
                    recap_exp.Load("Mail_EXP.ml");

                    //make the following body message
                    if (recap_exp.Lines.Count == 0)
                    {
                        sendMailExp = false;
                    }
                    else if (recap_exp.Lines.Count == 1)
                    {
                        sendMailExp = true;
                        textExp += "L'import d'un document commercial a échoué. Voici un résumé du document :\n";
                    }
                    else if (recap_exp.Lines.Count > 1)
                    {
                        sendMailExp = true;
                        textExp += "L'import de plusieurs documents commerciaux a échoué. Voici le résumé détaillé :\n";
                    }
                    for (int i = 0; i < recap_exp.Lines.Count; i++)
                    {
                        textExp += (i + 1) + " -\t Le numéro du document \"" + recap_exp.Lines[i].DocumentReference + "\" du fichier EDI : " + recap_exp.Lines[i].FileName + ", a une erreur : " + recap_exp.Lines[i].DocumentErrorMessage + "\n";
                    }
                }

                //send the recap mail
                if (sendMailImp & !sendMailExp)
                {
                    return new MailCustom("[" + recap_imp.Client + "] " + recap_imp.Subject, "Bonjour, \n\n" + textImp + "\nCordialement,\nConnecteur SAGE [" + recap_imp.Client + "]. Version client.", null);
                }
                else if (sendMailExp & !sendMailImp)
                {
                    return new MailCustom("[" + recap_exp.Client + "] " + recap_exp.Subject, "Bonjour, \n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + recap_exp.Client + "]. Version client.", null);
                }
                else if (sendMailImp && sendMailExp)
                {
                    return new MailCustom("[" + dns.Prefix + "] " + recap_imp.Subject + " et " + recap_exp.Subject, "Bonjour, \n\n" + textImp + "\n\n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + dns.Prefix + "]. Version client.", null);
                }
                else
                {
                    return null;
                }

            }
            else if (type.Equals("log"))
            {
                ConfigurationDNS dns = new ConfigurationDNS();
                dns.Load();
                CustomMailRecap recap_imp = null;
                CustomMailRecap recap_exp = null;
                bool sendMailImp = false;
                string textImp = "";
                bool sendMailExp = false;
                string textExp = "";
                List<string> attachements = new List<string>();

                //check if the file exist
                if (File.Exists("Mail_IMP.ml"))
                {
                    recap_imp = new CustomMailRecap();
                    recap_imp.Lines = new List<CustomMailRecapLines>();
                    recap_imp.Load("Mail_IMP.ml");

                    //make the following body message
                    if (recap_imp.Lines.Count == 0)
                    {
                        sendMailImp = false;
                    }
                    else if (recap_imp.Lines.Count == 1)
                    {
                        sendMailImp = true;
                        textImp += "L'import d'un document commercial a échoué. Voici un résumé du document en erreur :\n";
                    }
                    else if (recap_imp.Lines.Count > 1)
                    {
                        sendMailImp = true;
                        textImp += "L'import de plusieurs documents commerciaux a échoué. Voici le résumé de chaque document en erreur :\n";
                    }
                    for (int i = 0; i < recap_imp.Lines.Count; i++)
                    {
                        textImp += (i + 1) + " -\t Le numéro du document \"" + recap_imp.Lines[i].DocumentReference + "\"\nNom du fichier : " + recap_imp.Lines[i].FileName + "\nMessage erreur : " + recap_imp.Lines[i].DocumentErrorMessageDebug + "\nStackTrace: " + recap_imp.Lines[i].DocumentErrorStackTraceDebug + "\nL'erreur peut etre trouvé dans " + recap_imp.Lines[i].FilePath + "\n\n";
                    }
                    if (sendMailImp)
                    {
                        attachements.AddRange(recap_imp.Attachments);
                    }
                }

                //check if the file exist
                if (File.Exists("Mail_EXP.ml"))
                {
                    attachements.Clear();
                    recap_exp = new CustomMailRecap();
                    recap_exp.Lines = new List<CustomMailRecapLines>();
                    recap_exp.Load("Mail_EXP.ml");

                    //make the following body message
                    if (recap_exp.Lines.Count == 0)
                    {
                        sendMailExp = false;
                    }
                    else if (recap_exp.Lines.Count == 1)
                    {
                        sendMailExp = true;
                        textExp += "L'import d'un document commercial a échoué. Voici un résumé du document :\n";
                    }
                    else if (recap_exp.Lines.Count > 1)
                    {
                        sendMailExp = true;
                        textExp += "L'import de plusieurs documents commerciaux a échoué. Voici le résumé de chaque document :\n";
                    }
                    for (int i = 0; i < recap_exp.Lines.Count; i++)
                    {
                        textExp += (i + 1) + " -\t Le numéro du document \"" + recap_exp.Lines[i].DocumentReference + "\"\nNom du fichier : " + recap_exp.Lines[i].FileName + "\nMessage erreur : " + recap_exp.Lines[i].DocumentErrorMessageDebug + "\nStackTrace: " + recap_exp.Lines[i].DocumentErrorStackTraceDebug + "\nL'erreur peut etre trouvé dans " + recap_exp.Lines[i].FilePath + "\n\n";
                    }
                    if (sendMailExp)
                    {
                        attachements.AddRange(recap_exp.Attachments);
                    }
                }

                //send the recap mail
                if (sendMailImp & !sendMailExp)
                {
                    return new MailCustom("[" + recap_imp.Client + "] " + recap_imp.Subject, "Bonjour Team BDC, \n\n" + textImp + "\nCordialement,\nConnecteur SAGE [" + recap_imp.Client + "]. Version équipe", recap_imp.Attachments);
                }
                else if (sendMailExp & !sendMailImp)
                {
                    return new MailCustom("[" + recap_exp.Client + "] " + recap_exp.Subject, "Bonjour Team BDC, \n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + recap_exp.Client + "]. Version équipe", recap_exp.Attachments);
                }
                else if (sendMailImp && sendMailExp)
                {
                    return new MailCustom("[" + dns.Prefix + "] " + recap_imp.Subject + " et " + recap_exp.Subject, "Bonjour, \n\n" + textImp + "\n\n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + dns.Prefix + "]. Version équipe", attachements);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static string getFileSize(long size)
        {
            string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
            int cpt = 0;

            if (size == 0)
            {
                return string.Format("{0:n1}{1}", size, suffixes[0]);
            }

            while ((size / 1024) >= 1)
            {
                size = size / 1024;
                cpt++;
            }

            return string.Format("{0:n1}{1}", size, suffixes[cpt]);
        }

        /*
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
                Console.WriteLine(DateTime.Now + " | Main() : *********** Exception getInfoMail ***********");
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.Message);
                Console.WriteLine(DateTime.Now + " | Main() : " + ex.StackTrace);
                return null;
            }
        }

        public static void EnvoiMail(ConfSendMail confMail, string type, string subject, string body, List<string> attachements)
        {
            try
            {
                ConfigurationDNS dns = new ConfigurationDNS();
                dns.Load();

                // Objet mail
                MailMessage msg = new MailMessage();

                // Expéditeur (obligatoire). Notez qu'on peut spécifier le nom
                msg.From = new MailAddress(confMail.login, "CONNECTEUR SAGE [" + dns.Prefix + "]");

                // Destinataires (il en faut au moins un)
                if (type.Equals("client"))
                {
                    if (confMail.dest1_enable && !string.IsNullOrEmpty(confMail.dest1))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("client dest1 : " + confMail.dest1 + " | CC : edi@anexys.fr");
                        msg.To.Add(new MailAddress(confMail.dest1, confMail.dest1));
                        msg.CC.Add(new MailAddress("edi@anexys.fr", "edi@anexys.fr"));
                    }
                    else
                    {
                        Console.WriteLine("Sending mail to client is either desable or not configured!");
                        return;
                    }
                }
                else if (type.Equals("log"))
                {
                    if (confMail.dest2_enable && !string.IsNullOrEmpty(confMail.dest2))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("log dest2 : " + confMail.dest2 + " | CC : jl@anexys.fr");
                        msg.To.Add(new MailAddress(confMail.dest2, confMail.dest2));
                        msg.CC.Add(new MailAddress("jl@anexys.fr", "jl@anexys.fr"));
                    }
                    else
                    {
                        Console.WriteLine("Sending mail to team is either desable or not configured!");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Sending mail type is not client nor log");
                    return;
                }


                msg.Subject = subject;

                Console.WriteLine("Suject : " + subject);
                // Texte du mail (facultatif)
                msg.Body = body;
                Console.WriteLine("body : " + body);

                // Fichier joint si besoin (facultatif)
                if (attachements != null && attachements.Count > 0)
                {
                    Console.WriteLine("attachements.Count : " + attachements.Count);
                    for (int i = 0; i < attachements.Count; i++)
                    {
                        Console.WriteLine("attachements : " + attachements[i]);
                        msg.Attachments.Add(new Attachment(attachements[i]));
                    }
                }

                SmtpClient client;

                if (confMail.smtp != "" && confMail.login != "" && confMail.password != "")
                {
                    // Envoi du message SMTP
                    client = new SmtpClient(confMail.smtp, confMail.port);
                    client.Credentials = new NetworkCredential(confMail.login, Utils.Decrypt(confMail.password));
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

                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Envoi de Mail..OK");

            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + " | *********** Exception EnvoiMail ***********");
                Console.WriteLine(DateTime.Now + " | Message : " + e.Message);
                Console.WriteLine(DateTime.Now + " | StackTrace : " + e.StackTrace);
                //Console.ReadLine();
            }
        }
        */

    }
}
