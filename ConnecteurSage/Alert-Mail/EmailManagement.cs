using Alert_Mail.Classes;
using Alert_Mail.Classes.Configuration;
using Alert_Mail.Classes.Custom;
using Alert_Mail.Utilities;
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
    public class EmailManagement
    {
        private string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public EmailManagement() { }


        public string getDNS()
        {
            Connexion.ConnexionSaveLoad settings = new Connexion.ConnexionSaveLoad();
            settings.Load();
            return settings.configurationConnexion.SQL.PREFIX;
        }

        public MailCustom generateMailBody(string type, Database.Database db)
        {
            db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody(type: " + type + ") | Creation d une instance.");

            if (type.Equals("client_error"))
            {
                CustomMailRecap recap_imp = null;
                CustomMailRecap recap_exp = null;
                ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new ConfigurationCustomMailSaveLoad();

                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | type => client_error");

                bool sendMailImp = false;
                string textImp = "";
                string textImp_ = "";
                bool sendMailExp = false;
                string textExp = "";
                string textExp_ = "";
                List<string> attachements = new List<string>();

                //check if the file exist
                if (configurationCustomMailSaveLoad.isSettings(configurationCustomMailSaveLoad.fileName_ERR_Imp))
                {
                    configurationCustomMailSaveLoad.Load(configurationCustomMailSaveLoad.fileName_ERR_Imp);
                    recap_imp = configurationCustomMailSaveLoad.customMailRecap;

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
                        if(recap_imp.Lines[i].DocumentErrorMessage != null && !recap_imp.Lines[i].DocumentErrorMessage.Equals(""))
                        {
                            textImp_ += (i + 1) + " -\t Le numéro du document \"" + recap_imp.Lines[i].DocumentReference + "\" de la commande \"" + recap_imp.Lines[i].NumCommande + "\",\n" +
                            " \t a une erreur : " + recap_imp.Lines[i].DocumentErrorMessage + "\n";
                        }
                    }

                    if (textImp_.Equals(""))
                    {
                        sendMailImp = false;
                    }
                    else
                    {
                        textImp += textImp_;
                    }
                }
                else
                {
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Fichier mail => " + configurationCustomMailSaveLoad.fileName_ERR_Imp + " n existe pas.");
                }

                //check if the file exist
                if (configurationCustomMailSaveLoad.isSettings(configurationCustomMailSaveLoad.fileName_ERR_Exp))
                {
                    configurationCustomMailSaveLoad.Load(configurationCustomMailSaveLoad.fileName_ERR_Exp);
                    recap_exp = configurationCustomMailSaveLoad.customMailRecap;


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
                        if (recap_exp.Lines[i].DocumentErrorMessage != null && !recap_exp.Lines[i].DocumentErrorMessage.Equals(""))
                        {
                            textExp_ += (i + 1) + " -\t Le numéro du document \"" + recap_exp.Lines[i].DocumentReference + "\" du fichier EDI : " + recap_exp.Lines[i].FileName + ", a une erreur : " + recap_exp.Lines[i].DocumentErrorMessage + "\n";
                        }
                    }

                    if (textExp_.Equals(""))
                    {
                        sendMailExp = false;
                    }
                    else
                    {
                        textExp += textExp_;
                    }
                }
                else
                {
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Fichier mail => " + configurationCustomMailSaveLoad.fileName_ERR_Exp + " n existe pas.");
                }

                //send the recap mail
                MailCustom xxx = null;
                if (sendMailImp & !sendMailExp)
                {
                    xxx = new MailCustom("[" + recap_imp.Client + "] " + recap_imp.Subject, "Bonjour, \n\n" + textImp + "\nCordialement,\nConnecteur SAGE [" + recap_imp.Client + "]. Version client.", null);
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Json : " + Newtonsoft.Json.JsonConvert.SerializeObject(xxx, Newtonsoft.Json.Formatting.Indented));
                    return xxx;
                }
                else if (sendMailExp & !sendMailImp)
                {
                    xxx = new MailCustom("[" + recap_exp.Client + "] " + recap_exp.Subject, "Bonjour, \n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + recap_exp.Client + "]. Version client.", null);
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Json : " + Newtonsoft.Json.JsonConvert.SerializeObject(xxx, Newtonsoft.Json.Formatting.Indented));
                    return xxx;
                }
                else if (sendMailImp && sendMailExp)
                {
                    xxx = new MailCustom("[" + getDNS() + "] " + recap_imp.Subject + " et " + recap_exp.Subject, "Bonjour, \n\n" + textImp + "\n\n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + getDNS() + "]. Version client.", null);
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Json : " + Newtonsoft.Json.JsonConvert.SerializeObject(xxx, Newtonsoft.Json.Formatting.Indented));
                    return xxx;
                }
                else
                {
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Rien à preparer.");
                    return xxx;
                }

            }
            else if (type.Equals("log"))
            {
                CustomMailRecap recap_imp = null;
                CustomMailRecap recap_exp = null;
                ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new ConfigurationCustomMailSaveLoad();

                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | type => log");

                bool sendMailImp = false;
                string textImp = "";
                bool sendMailExp = false;
                string textExp = "";
                List<string> attachements = new List<string>();

                //check if the file exist
                if (configurationCustomMailSaveLoad.isSettings(configurationCustomMailSaveLoad.fileName_ERR_Imp))
                {
                    configurationCustomMailSaveLoad.Load(configurationCustomMailSaveLoad.fileName_ERR_Imp);
                    recap_imp = configurationCustomMailSaveLoad.customMailRecap;

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
                else
                {
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Fichier mail => "+ configurationCustomMailSaveLoad.fileName_ERR_Imp +" n existe pas.");
                }

                //check if the file exist
                //check if the file exist
                if (configurationCustomMailSaveLoad.isSettings(configurationCustomMailSaveLoad.fileName_ERR_Exp))
                {
                    configurationCustomMailSaveLoad.Load(configurationCustomMailSaveLoad.fileName_ERR_Exp);
                    recap_exp = configurationCustomMailSaveLoad.customMailRecap;


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
                else
                {
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Fichier mail => " + configurationCustomMailSaveLoad.fileName_ERR_Exp + " n existe pas.");
                }

                //send the recap mail
                MailCustom xxx = null;
                if (sendMailImp & !sendMailExp)
                {
                    xxx = new MailCustom("[" + recap_imp.Client + "] " + recap_imp.Subject, "Bonjour Team BDC, \n\n" + textImp + "\nCordialement,\nConnecteur SAGE [" + recap_imp.Client + "]. Version équipe", recap_imp.Attachments);
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Json : " + Newtonsoft.Json.JsonConvert.SerializeObject(xxx, Newtonsoft.Json.Formatting.Indented));
                    return xxx;
                }
                else if (sendMailExp & !sendMailImp)
                {
                    xxx = new MailCustom("[" + recap_exp.Client + "] " + recap_exp.Subject, "Bonjour Team BDC, \n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + recap_exp.Client + "]. Version équipe", recap_exp.Attachments);
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Json : " + Newtonsoft.Json.JsonConvert.SerializeObject(xxx, Newtonsoft.Json.Formatting.Indented));
                    return xxx;
                }
                else if (sendMailImp && sendMailExp)
                {
                    xxx = new MailCustom("[" + getDNS() + "] " + recap_imp.Subject + " et " + recap_exp.Subject, "Bonjour, \n\n" + textImp + "\n\n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + getDNS() + "]. Version équipe", attachements);
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Json : " + Newtonsoft.Json.JsonConvert.SerializeObject(xxx, Newtonsoft.Json.Formatting.Indented));
                    return xxx;
                }
                else
                {
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Rien à preparer.");
                    return xxx;
                }
            }
            else
            {
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => generateMailBody() | Aucun type trouvé!");
                return null;
            }
        }

        public string getFileSize(long size)
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

        public void EnvoiMail(ConfigurationEmail mConfigurationEmail, string type, string subject, string body, List<string> attachements, Database.Database db)
        {
            try
            {
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Creation d une instance.");
                if (mConfigurationEmail.active){
                    // Objet mail
                    MailMessage msg = new MailMessage();

                    // Expéditeur (obligatoire). Notez qu'on peut spécifier le nom
                    msg.From = new MailAddress("edi@anexys.fr", "CONNECTEUR SAGE [" + getDNS() + "]");
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Utiliser l'adresse \"edi@anexys.fr\" sur le nom de CONNECTEUR SAGE [" + getDNS() + "] pour l'expéditeur.");

                    // Destinataires (il en faut au moins un)
                    if (type.Equals("client"))
                    {
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Mail type client.");
                        if (mConfigurationEmail.emailLists.activeClient && mConfigurationEmail.emailLists.emailClientList.Count > 0)
                        {
                            msg.CC.Add("edi@anexys.fr");
                            db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Envoie des mails client activé!");
                            for (int x = 0; x < mConfigurationEmail.emailLists.emailClientList.Count; x++)
                            {
                                if (!string.IsNullOrEmpty(mConfigurationEmail.emailLists.emailClientList[x]))
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine(DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Ajouter le l'adresse client \"" + mConfigurationEmail.emailLists.emailClientList[x] + "\" dans le mail.");
                                    msg.To.Add(mConfigurationEmail.emailLists.emailClientList[x]);
                                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Ajouter le l adresse client " + mConfigurationEmail.emailLists.emailClientList[x] + " dans le mail.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | L'envoi des mails client sont désactivé!");
                            db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | L envoi des mails client sont désactivé!");
                            return;
                        }

                    }
                    else if (type.Equals("log"))
                    {
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Mail type team.");
                        if (mConfigurationEmail.emailLists.activeTeam)
                        {
                            db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Envoie des mails team activé!");
                            for (int x = 0; x < mConfigurationEmail.emailLists.emailTeamList.Count; x++)
                            {
                                if (!string.IsNullOrEmpty(mConfigurationEmail.emailLists.emailTeamList[x]))
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine(DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Ajouter le l'adresse team \"" + mConfigurationEmail.emailLists.emailClientList[x] + "\" dans le mail.");
                                    msg.To.Add(mConfigurationEmail.emailLists.emailTeamList[x]);
                                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Ajouter le l adresse team " + mConfigurationEmail.emailLists.emailClientList[x] + " dans le mail.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("L'envoi des mails team sont désactivé!");
                            db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | L envoi des mails team sont désactivé!");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Mail type n'est pas client ou log");
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Mail type n est pas client ou log");
                        return;
                    }


                    // sujet du mail
                    msg.Subject = subject;
                    Console.WriteLine("Suject : " + subject);

                    // Texte du mail
                    msg.Body = body;
                    Console.WriteLine("body : " + body);

                    // Fichier joint si besoin (facultatif)
                    if (attachements != null && attachements.Count > 0)
                    {
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Nombre Piece Join : " + attachements.Count);
                        Console.WriteLine("attachements.Count : " + attachements.Count);
                        for (int i = 0; i < attachements.Count; i++)
                        {
                            Console.WriteLine("attachements : " + attachements[i]);
                            msg.Attachments.Add(new Attachment(attachements[i]));
                        }
                    }


                    SmtpClient smtpServer;

                    if (Utils.Decrypt(mConfigurationEmail.connexion.smtp) != "" && Utils.Decrypt(mConfigurationEmail.connexion.login) != "" && Utils.Decrypt(mConfigurationEmail.connexion.password) != "")
                    {
                        // Envoi du message SMTP
                        smtpServer = new SmtpClient(Utils.Decrypt(mConfigurationEmail.connexion.smtp), Convert.ToInt32(Utils.Decrypt(mConfigurationEmail.connexion.port)));
                        smtpServer.Credentials = new NetworkCredential(Utils.Decrypt(mConfigurationEmail.connexion.login), Utils.Decrypt(mConfigurationEmail.connexion.password));
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | smtp : " + Utils.Decrypt(mConfigurationEmail.connexion.smtp) + " | Port : " + Convert.ToInt32(Utils.Decrypt(mConfigurationEmail.connexion.port)));
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | login : " + Utils.Decrypt(mConfigurationEmail.connexion.login) + " | pwd : " + Utils.Decrypt(mConfigurationEmail.connexion.password));
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine(DateTime.Now + " : Les paramètres de mail ne sont pas correcte!");
                        db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Les paramètres de mail ne sont pas correcte!");
                        return;
                    }

                    // EnableSsl
                    smtpServer.EnableSsl = true;
                    //NetworkInformation s = new NetworkCredential();
                    //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    // Envoi du mail
                    smtpServer.Send(msg);

                    Console.WriteLine("");
                    Console.WriteLine(DateTime.Now + " : Envoi de Mail..OK");
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Envoi de Mail..OK");

                }
                else
                {
                    Console.WriteLine("La configuration des alert mail sont désactivé !");
                    db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | La configuration des alert mail sont désactivé !");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + " | *********** Exception EnvoiMail ***********");
                Console.WriteLine(DateTime.Now + " | Message : " + e.Message);
                Console.WriteLine(DateTime.Now + " | StackTrace : " + e.StackTrace);

                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " :: ############################### Exception EnvoiMail ###############################");
                db.alertMailLogManager.insert(db.connectionString, DateTime.Now + " : AlertMail :: Alert-Mail.dll => EnvoiMail() | Message : " + e.Message);
                db.alertMailLogManager.insert(db.connectionString, "");
            }
        }

    }
}
