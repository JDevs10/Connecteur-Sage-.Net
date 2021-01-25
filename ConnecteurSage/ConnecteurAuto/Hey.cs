using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ConnecteurAuto
{
    public class Hey
    {
        public void hello()
        {
            Database.Database db = new Database.Database();
            db.initTables();

            Database.Model.Settings settings = db.settingsManager.get(db.connectionString, 1);
            string path_Dump_File = settings.EXE_Folder + @"\" + "Dump_File";

            if (!Directory.Exists(path_Dump_File))
            {
                //Create log directory
                Directory.CreateDirectory(path_Dump_File);
            }

            //string hello = path_Dump_File + @"\" + string.Format("testing_JL_1.txt", DateTime.Now);
            string hello = path_Dump_File + string.Format(@"\testing_JL_1.txt", DateTime.Now); 
            var logFile_import = File.Create(hello);

            using (StreamWriter writer = new StreamWriter(logFile_import))
            {
                writer.WriteLine("Hello");
                writer.WriteLine("Dir 1 : " + Directory.GetCurrentDirectory());
                writer.WriteLine("Dir 2 : " + path_Dump_File);
                writer.WriteLine("file : " + hello);
                writer.Flush();
                writer.Close();
            }

            try
            {
                // Objet mail
                MailMessage msg = new MailMessage();

                // Expéditeur (obligatoire). Notez qu'on peut spécifier le nom
                msg.From = new MailAddress("jl@anexys.fr", "J-L CONNECTEUR SAGE CFCI-TW");
                msg.To.Add(new MailAddress("jl@anexys.fr", "jl@anexys.fr"));
                msg.Subject = "Testing Connecteur";
                msg.Body = "Hello testing";

                // Fichier joint si besoin (facultatif)
                msg.Attachments.Add(new Attachment(hello));

                SmtpClient client;

                // Envoi du message SMTP
                client = new SmtpClient("smtp.gmail.com", 587);
                client.Credentials = new NetworkCredential("jl@anexys.fr", "anexys1,");
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
            }

        }
    }
}
