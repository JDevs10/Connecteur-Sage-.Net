using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace importPlanifier.Classes
{
    public class ConfSendMail
    {
        [XmlElement]
        public string smtp { get; set; }
        [XmlElement]
        public int port { get; set; }
        [XmlElement]
        public string login { get; set; }
        [XmlElement]
        public string password { get; set; }
        [XmlElement]
        public string dest1 { get; set; }
        [XmlElement]
        public string dest2 { get; set; }
        [XmlElement]
        public string dest3 { get; set; }
        [XmlElement]
        public Boolean active { get; set; }
        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConfSendMail()
        {

        }

        public ConfSendMail(string smtp, int port, string login, string password, string dest1, string dest2, string dest3, Boolean active)
        {
            this.smtp = smtp;
            this.port = port;
            this.login = login;
            this.password = password;
            this.dest1 = dest1;
            this.dest2 = dest2;
            this.dest3 = dest3;
            this.active = active;
        }

        public void Load()
        {
            if (File.Exists("SettingMail.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfSendMail));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingMail.xml");
                ConfSendMail mail = new ConfSendMail();
                mail = (ConfSendMail)reader.Deserialize(file);

                this.smtp = mail.smtp;
                this.port = mail.port;
                this.login = mail.login;
                this.password = mail.password;
                this.dest1 = mail.dest1;
                this.dest2 = mail.dest2;
                this.dest3 = mail.dest3;
                this.active = mail.active;
                file.Close();
            }
        }

        public void saveInfo(ConfSendMail mailSettings)
        {
            try
            {
                var myfile = File.Create(pathModule + @"\SettingMail.xml");
                XmlSerializer xml = new XmlSerializer(typeof(ConfSendMail));
                xml.Serialize(myfile, mailSettings);
                myfile.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
            }
        }
    }
}
