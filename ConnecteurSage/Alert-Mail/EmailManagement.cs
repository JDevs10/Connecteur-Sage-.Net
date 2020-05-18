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

        public void createInportMailFile(StreamWriter writer, List<CustomMailSuccess> successList) 
        {
            if(successList.Count > 0)
            {
                Alert_Mail.Classes.ConfigurationSaveLoad configurationSaveLoad1 = new Alert_Mail.Classes.ConfigurationSaveLoad();
                if (configurationSaveLoad1.isSettings())
                {
                    configurationSaveLoad1.Load();

                    if (configurationSaveLoad1.configurationEmail.active)
                    {
                        if (configurationSaveLoad1.configurationEmail.emailImport.active)
                        {
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | ################################ Alert Mail Import ###############################");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | Il y a " + successList.Count + " documents qui sont importé.");

                            if (configurationSaveLoad1.configurationEmail.emailImport.eachDocument)
                            {
                                Connexion.ConnexionSaveLoad connexionSaveLoad = new Connexion.ConnexionSaveLoad();
                                connexionSaveLoad.Load();

                                // Inform the client
                                if (configurationSaveLoad1.configurationEmail.emailImport.informClient)
                                {
                                    writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | Informer le client à chaque import de document.");

                                    string header = "Bonjour, \n";
                                    string body = "La commande : " + successList[0].NumCommande + " de " + successList[0].Client + " est importée.\nLa livraison est prévue pour le " + successList[0].DateTimeDelivery + ".\n";
                                    string footer = "\nCordialement,\nConnecteur SAGE ["+ connexionSaveLoad .configurationConnexion.SQL.PREFIX+ "]. Version client";

                                    for (int i = 0; i < successList[0].Lines.Count; i++)
                                    {
                                        body += (i+1)+"-\tL'article : " + successList[0].Lines[i].ProductName + " dont le CodeBare : " + successList[0].Lines[i].ProductRef + ", le prix TTC : " + successList[0].Lines[i].ProductPriceTTC + " et la quantité commandée : " + successList[0].Lines[i].ProductQte + "\n";
                                    }

                                    // send mail
                                    EnvoiMail(configurationSaveLoad1.configurationEmail, "client", successList[0].Subject, header + body + footer, null);
                                    writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | email envoyé au client.");
                                }
                                
                                // Inform the team
                                if (configurationSaveLoad1.configurationEmail.emailImport.informTeam)
                                {
                                    writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | Informer l'équipe à chaque import de document.");

                                    string header = "Bonjour, \n";
                                    string body = "La commande : " + successList[0].NumCommande + " de " + successList[0].Client + " est importée.\nLa livraison est prévue pour le " + successList[0].DateTimeDelivery + ".\n";
                                    string footer = "\nCordialement,\nConnecteur SAGE [" + connexionSaveLoad.configurationConnexion.SQL.PREFIX + "]. Version équipe";

                                    for (int i = 0; i < successList[0].Lines.Count; i++)
                                    {
                                        body += (i + 1)+"-\tL'article : " + successList[0].Lines[i].ProductName + " dont le CodeBare : " + successList[0].Lines[i].ProductRef + ", le prix TTC : " + successList[0].Lines[i].ProductPriceTTC + " et la quantité commandée : " + successList[0].Lines[i].ProductQte + "\n";
                                    }

                                    // send mail
                                    EnvoiMail(configurationSaveLoad1.configurationEmail, "client", successList[0].Subject, header + body + footer, null);
                                    writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | email envoyé à l'équipe.");
                                }

                                // clear the list after each successful import
                                successList.Clear();
                            }
                            else if (configurationSaveLoad1.configurationEmail.emailImport.atTheEnd)
                            {
                                writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | Informer le client et ou l'équipe à la fin de l'execution.");

                                Alert_Mail.Classes.ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new Alert_Mail.Classes.ConfigurationCustomMailSaveLoad();
                                configurationCustomMailSaveLoad.saveInfo(configurationCustomMailSaveLoad.fileName_SUC_Imp, successList);

                                writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | Le fichier " + configurationCustomMailSaveLoad.fileName_SUC_Imp + " est créé!");

                                // clear the list after each successful import
                                successList.Clear();
                            }
                            writer.WriteLine("");
                            writer.Flush();
                        }
                        else
                        {
                            writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | Notification mail import est désactivé!");
                            writer.WriteLine("");
                        }
                    }
                    else
                    {
                        writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | AlertMail est désactivé!");
                        writer.WriteLine("");
                    }
                }
                else
                {
                    writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createInportMailFile() | Aucune configuration d'AlertMail trouvé!");
                    writer.WriteLine("");
                }
            }
            writer.Flush();
        }

        public void createExportMailFile(StreamWriter writer, List<CustomMailRecapLines> successLinesList, List<CustomMailSuccess> successList)
        {
            if(successLinesList.Count > 0)
            {
                Alert_Mail.Classes.ConfigurationSaveLoad configurationSaveLoad1 = new Alert_Mail.Classes.ConfigurationSaveLoad();
                if (configurationSaveLoad1.isSettings())
                {
                    configurationSaveLoad1.Load();

                    if (configurationSaveLoad1.configurationEmail.active)
                    {
                        if (configurationSaveLoad1.configurationEmail.emailExport.active)
                        {
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | ################################ Alert Mail Export ###############################");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | Il y a " + successLinesList.Count + " documents qui sont exporté!");

                            if (configurationSaveLoad1.configurationEmail.emailExport.eachDocument)
                            {
                                Connexion.ConnexionSaveLoad connexionSaveLoad = new Connexion.ConnexionSaveLoad();
                                connexionSaveLoad.Load();

                                // Inform the client
                                if (configurationSaveLoad1.configurationEmail.emailExport.informClient)
                                {
                                    writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | Informer le client à chaque export de document.");

                                    string header = "Bonjour, \n";
                                    string body = "Le document : " + successList[0].NumCommande + " est exportée.\n";
                                    string footer = "\nCordialement,\nConnecteur SAGE [" + connexionSaveLoad.configurationConnexion.SQL.PREFIX + "]. Version client";

                                    for (int i = 0; i < successList[0].Lines.Count; i++)
                                    {
                                        body += (i + 1) + "-\tL'article : " + successList[0].Lines[i].ProductName + " dont le CodeBare : " + successList[0].Lines[i].ProductRef + ", le prix TTC : " + successList[0].Lines[i].ProductPriceTTC + " et la quantité : " + successList[0].Lines[i].ProductQte + "\n";
                                    }

                                    // send mail
                                    EnvoiMail(configurationSaveLoad1.configurationEmail, "client", successList[0].Subject, header + body + footer, null);
                                    writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | email envoyé au client.");
                                }

                                // Inform the team
                                if (configurationSaveLoad1.configurationEmail.emailImport.informTeam)
                                {
                                    writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | Informer l'équipe à chaque export de document.");

                                    string header = "Bonjour, \n";
                                    string body = "Le document : " + successList[0].NumCommande + " est exportée.\n";
                                    string footer = "\nCordialement,\nConnecteur SAGE [" + connexionSaveLoad.configurationConnexion.SQL.PREFIX + "]. Version équipe";

                                    for (int i = 0; i < successList[0].Lines.Count; i++)
                                    {
                                        body += (i + 1) + "-\tL'article : " + successList[0].Lines[i].ProductName + " dont le CodeBare : " + successList[0].Lines[i].ProductRef + ", le prix TTC : " + successList[0].Lines[i].ProductPriceTTC + " et la quantité : " + successList[0].Lines[i].ProductQte + "\n";
                                    }

                                    // send mail
                                    EnvoiMail(configurationSaveLoad1.configurationEmail, "client", successList[0].Subject, header + body + footer, null);
                                    writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | email envoyé à l'équipe.");
                                }

                                // clear the list after each successful import
                                successList.Clear();
                            }
                            else if (configurationSaveLoad1.configurationEmail.emailExport.atTheEnd)
                            {
                                writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | Informer le client et ou l'équipe à la fin de l'execution.");

                                Alert_Mail.Classes.ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new Alert_Mail.Classes.ConfigurationCustomMailSaveLoad();
                                configurationCustomMailSaveLoad.saveInfo(configurationCustomMailSaveLoad.fileName_SUC_Exp, successList);

                                writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | Le fichier " + configurationCustomMailSaveLoad.fileName_SUC_Exp + " est créé!");

                            }
                            writer.WriteLine("");
                            writer.Flush();
                            successLinesList.Clear();
                        }
                        else
                        {
                            writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | Notification mail export est désactivé!");
                            writer.WriteLine("");
                        }
                    }
                    else
                    {
                        writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | AlertMail est désactivé!");
                        writer.WriteLine("");
                    }
                }
                else
                {
                    writer.WriteLine(DateTime.Now + " : Alert-Mail => EmailManagement => createExportMailFile() | Aucune configuration d'AlertMail trouvé!");
                    writer.WriteLine("");
                }
            }
        }

        public MailCustom generateMailBody(string type)
        {
            if (type.Equals("client_import"))
            {
                List<CustomMailSuccess> success_imp_list = null;
                ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new ConfigurationCustomMailSaveLoad();

                bool sendMailImp = false;
                string textImp = "";
                string textImp_2 = "";

                //check if the file exist
                if (configurationCustomMailSaveLoad.isSettings(configurationCustomMailSaveLoad.fileName_SUC_Imp))
                {
                    configurationCustomMailSaveLoad.Load(configurationCustomMailSaveLoad.fileName_SUC_Imp, new List<CustomMailSuccess>());
                    success_imp_list = configurationCustomMailSaveLoad.customMailSuccessList;

                    //make the following body message
                    if (success_imp_list.Count == 0)
                    {
                        sendMailImp = false;
                    }
                    else if (success_imp_list.Count == 1)
                    {
                        sendMailImp = true;
                        textImp += "L'import du document commercial avec succès : \n";
                    }
                    else if (success_imp_list.Count > 1)
                    {
                        sendMailImp = true;
                        textImp += "L'import de plusieurs documents commerciaux avec succès. Voici le résumé :";
                    }

                    for (int i = 0; i < success_imp_list.Count; i++)
                    {
                        textImp += "\n"+(i + 1) + " - Le numéro du document \"" + success_imp_list[i].DocumentReference + "\" de la commande \"" + success_imp_list[i].NumCommande + "\",\n";

                        for(int x = 0; x < success_imp_list[i].Lines.Count; x++)
                        {
                            textImp_2 += "\t - " + success_imp_list[i].Lines[x].ProductQte + " " + success_imp_list[i].Lines[x].ProductName + " dont le CodeBarre " + success_imp_list[i].Lines[x].ProductRef + " au TTC : " + success_imp_list[i].Lines[x].ProductPriceTTC + "\n";
                        }
                        textImp += textImp_2;
                        textImp_2 = "";
                    }
                }

                //send the recap mail
                if (sendMailImp)
                {
                    return new MailCustom("[" + getDNS() + "] " + success_imp_list[0].Subject, "Bonjour, \n\n" + textImp + "\nCordialement,\nConnecteur SAGE [" + getDNS() + "]. Version client.", null);
                }
                else
                {
                    return null;
                }

            }
            else if (type.Equals("team_import")) /// need to to do
            {
                List<CustomMailSuccess> success_imp_list = null;
                ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new ConfigurationCustomMailSaveLoad();

                bool sendMailImp = false;
                string textImp = "";
                string textImp_2 = "";

                //check if the file exist
                if (configurationCustomMailSaveLoad.isSettings(configurationCustomMailSaveLoad.fileName_SUC_Imp))
                {
                    configurationCustomMailSaveLoad.Load(configurationCustomMailSaveLoad.fileName_SUC_Imp, new List<CustomMailSuccess>());
                    success_imp_list = configurationCustomMailSaveLoad.customMailSuccessList;

                    //make the following body message
                    if (success_imp_list.Count == 0)
                    {
                        sendMailImp = false;
                    }
                    else if (success_imp_list.Count == 1)
                    {
                        sendMailImp = true;
                        textImp += "L'import du document commercial avec succès : \n";
                    }
                    else if (success_imp_list.Count > 1)
                    {
                        sendMailImp = true;
                        textImp += "L'import de plusieurs documents commerciaux avec succès. Voici le résumé :";
                    }

                    for (int i = 0; i < success_imp_list.Count; i++)
                    {
                        textImp += "\n" + (i + 1) + " - Le numéro du document \"" + success_imp_list[i].DocumentReference + "\" de la commande \"" + success_imp_list[i].NumCommande + "\",\n";

                        for (int x = 0; x < success_imp_list[i].Lines.Count; x++)
                        {
                            textImp_2 += "\t - " + success_imp_list[i].Lines[x].ProductQte + " " + success_imp_list[i].Lines[x].ProductName + " dont le CodeBarre " + success_imp_list[i].Lines[x].ProductRef + " au TTC : " + success_imp_list[i].Lines[x].ProductPriceTTC + "\n";
                        }
                        textImp += textImp_2;
                        textImp_2 = "";
                    }
                }

                //send the recap mail
                if (sendMailImp)
                {
                    return new MailCustom("[" + getDNS() + "] " + success_imp_list[0].Subject, "Bonjour Team, \n\n" + textImp + "\nCordialement,\nConnecteur SAGE [" + getDNS() + "]. Version équipe.", null);
                }
                else
                {
                    return null;
                }

            }
            else if (type.Equals("client_export"))
            {
                List<CustomMailSuccess> success_exp_list = null;
                ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new ConfigurationCustomMailSaveLoad();

                bool sendMailExp = false;
                string textExp = "";
                string textExp_2 = "";

                //check if the file exist
                if (configurationCustomMailSaveLoad.isSettings(configurationCustomMailSaveLoad.fileName_SUC_Exp))
                {
                    configurationCustomMailSaveLoad.Load(configurationCustomMailSaveLoad.fileName_SUC_Exp, new List<CustomMailSuccess>());
                    success_exp_list = configurationCustomMailSaveLoad.customMailSuccessList;


                    //make the following body message
                    if (success_exp_list.Count == 0)
                    {
                        sendMailExp = false;
                    }
                    else if (success_exp_list.Count == 1)
                    {
                        sendMailExp = true;
                        textExp += "L'import d'un document commercial avec succès :\n";
                    }
                    else if (success_exp_list.Count > 1)
                    {
                        sendMailExp = true;
                        textExp += "L'import de plusieurs documents commerciaux avec succès. Voici le résumé détaillé :\n";
                    }

                    for (int i = 0; i < success_exp_list.Count; i++)
                    {
                        textExp += "\n" + (i + 1) + " - Le numéro du document \"" + success_exp_list[i].DocumentReference + "\" de la commande \"" + success_exp_list[i].NumCommande + "\",\n";

                        for (int x = 0; x < success_exp_list[i].Lines.Count; x++)
                        {
                            textExp_2 += "\t - " + success_exp_list[i].Lines[x].ProductQte + " " + success_exp_list[i].Lines[x].ProductName + " dont le CodeBarre " + success_exp_list[i].Lines[x].ProductRef + " au TTC : " + success_exp_list[i].Lines[x].ProductPriceTTC + "\n";
                        }
                        textExp += textExp_2;
                        textExp_2 = "";
                    }
                }

                if (sendMailExp)
                {
                    return new MailCustom("[" + getDNS() + "] " + success_exp_list[0].Subject, "Bonjour, \n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + getDNS() + "]. Version client.", null);
                }
                else
                {
                    return null;
                }

            }
            else if (type.Equals("team_export"))
            {
                List<CustomMailSuccess> success_exp_list = null;
                ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new ConfigurationCustomMailSaveLoad();

                bool sendMailExp = false;
                string textExp = "";
                string textExp_2 = "";

                //check if the file exist
                if (configurationCustomMailSaveLoad.isSettings(configurationCustomMailSaveLoad.fileName_SUC_Exp))
                {
                    configurationCustomMailSaveLoad.Load(configurationCustomMailSaveLoad.fileName_SUC_Exp, new List<CustomMailSuccess>());
                    success_exp_list = configurationCustomMailSaveLoad.customMailSuccessList;


                    //make the following body message
                    if (success_exp_list.Count == 0)
                    {
                        sendMailExp = false;
                    }
                    else if (success_exp_list.Count == 1)
                    {
                        sendMailExp = true;
                        textExp += "L'import d'un document commercial avec succès :\n";
                    }
                    else if (success_exp_list.Count > 1)
                    {
                        sendMailExp = true;
                        textExp += "L'import de plusieurs documents commerciaux avec succès. Voici le résumé détaillé :\n";
                    }

                    for (int i = 0; i < success_exp_list.Count; i++)
                    {
                        textExp += "\n" + (i + 1) + " - Le numéro du document \"" + success_exp_list[i].DocumentReference + "\" de la commande \"" + success_exp_list[i].NumCommande + "\",\n";

                        for (int x = 0; x < success_exp_list[i].Lines.Count; x++)
                        {
                            textExp_2 += "\t - " + success_exp_list[i].Lines[x].ProductQte + " " + success_exp_list[i].Lines[x].ProductName + " dont le CodeBarre " + success_exp_list[i].Lines[x].ProductRef + " au TTC : " + success_exp_list[i].Lines[x].ProductPriceTTC + "\n";
                        }
                        textExp += textExp_2;
                        textExp_2 = "";
                    }

                }

                if (sendMailExp)
                {
                    return new MailCustom("[" + getDNS() + "] " + success_exp_list[0].Subject, "Bonjour Team, \n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + getDNS() + "]. Version équipe.", null);
                }
                else
                {
                    return null;
                }

            }
            else if (type.Equals("client_error"))
            {
                CustomMailRecap recap_imp = null;
                CustomMailRecap recap_exp = null;
                ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new ConfigurationCustomMailSaveLoad();

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
                    return new MailCustom("[" + getDNS() + "] " + recap_imp.Subject + " et " + recap_exp.Subject, "Bonjour, \n\n" + textImp + "\n\n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + getDNS() + "]. Version client.", null);
                }
                else
                {
                    return null;
                }

            }
            else if (type.Equals("log"))
            {
                CustomMailRecap recap_imp = null;
                CustomMailRecap recap_exp = null;
                ConfigurationCustomMailSaveLoad configurationCustomMailSaveLoad = new ConfigurationCustomMailSaveLoad();

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
                    return new MailCustom("[" + getDNS() + "] " + recap_imp.Subject + " et " + recap_exp.Subject, "Bonjour, \n\n" + textImp + "\n\n\n" + textExp + "\nCordialement,\nConnecteur SAGE [" + getDNS() + "]. Version équipe", attachements);
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

        public void EnvoiMail(ConfigurationEmail mConfigurationEmail, string type, string subject, string body, List<string> attachements)
        {
            try
            {
                if (mConfigurationEmail.active)
                {
                    // Objet mail
                    MailMessage msg = new MailMessage();

                    // Expéditeur (obligatoire). Notez qu'on peut spécifier le nom
                    msg.From = new MailAddress(Utils.Decrypt(mConfigurationEmail.connexion.login), "CONNECTEUR SAGE [" + getDNS() + "]");

                    Console.WriteLine("emailClientList : " + mConfigurationEmail.emailLists.emailClientList[0]);

                    // Destinataires (il en faut au moins un)
                    if (type.Equals("client"))
                    {
                        if (mConfigurationEmail.emailLists.activeClient)
                        {
                            for (int x = 0; x < mConfigurationEmail.emailLists.emailClientList.Count; x++)
                            {
                                if (!string.IsNullOrEmpty(mConfigurationEmail.emailLists.emailClientList[x]))
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine("client : " + mConfigurationEmail.emailLists.emailClientList[x]);
                                    msg.To.Add(new MailAddress(mConfigurationEmail.emailLists.emailClientList[x], mConfigurationEmail.emailLists.emailClientList[x]));

                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("L'envoi des mails client sont désactivé!");
                            return;
                        }
                        
                    }
                    else if (type.Equals("log"))
                    {
                        if (mConfigurationEmail.emailLists.activeTeam)
                        {
                            for (int x = 0; x < mConfigurationEmail.emailLists.emailTeamList.Count; x++)
                            {
                                Console.WriteLine("team : " + mConfigurationEmail.emailLists.emailTeamList[x]);
                                if (!string.IsNullOrEmpty(mConfigurationEmail.emailLists.emailTeamList[x]))
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine("team : " + mConfigurationEmail.emailLists.emailTeamList[x]);
                                    msg.To.Add(new MailAddress(mConfigurationEmail.emailLists.emailTeamList[x], mConfigurationEmail.emailLists.emailTeamList[x]));

                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("L'envoi des mails team sont désactivé!");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sending mail type is not client nor log");
                        return;
                    }


                    msg.Subject = "TEST : " + subject;

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

                    if (Utils.Decrypt(mConfigurationEmail.connexion.smtp) != "" && Utils.Decrypt(mConfigurationEmail.connexion.login) != "" && Utils.Decrypt(mConfigurationEmail.connexion.password) != "")
                    {
                        // Envoi du message SMTP
                        client = new SmtpClient(Utils.Decrypt(mConfigurationEmail.connexion.smtp), Convert.ToInt32(Utils.Decrypt(mConfigurationEmail.connexion.port)));
                        client.Credentials = new NetworkCredential(Utils.Decrypt(mConfigurationEmail.connexion.login), Utils.Decrypt(mConfigurationEmail.connexion.password));
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine(DateTime.Now + " : Les paramètres de connexion ne sont pas correcte!");
                        return;
                    }

                    client.EnableSsl = true;
                    //NetworkInformation s = new NetworkCredential();
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    // Envoi du mail
                    client.Send(msg);

                    Console.WriteLine("");
                    Console.WriteLine(DateTime.Now + " : Envoi de Mail..OK");
                }
                else
                {
                    Console.WriteLine("La configuration des alert mail sont désactivé !");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + " | *********** Exception EnvoiMail ***********");
                Console.WriteLine(DateTime.Now + " | Message : " + e.Message);
                Console.WriteLine(DateTime.Now + " | StackTrace : " + e.StackTrace);
                //Console.ReadLine();
            }
        }

    }
}
