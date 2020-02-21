using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AlertMail.Classes
{
    public class CustomScanCSV
    {
        [XmlElement]
        public string MailType { set; get; }
        [XmlElement]
        public string Client { set; get; }
        [XmlElement]
        public string Subject { set; get; }
        [XmlElement]
        public string DateTimeCreated { set; get; }
        [XmlElement]
        public string DateTimeModified { set; get; }
        [XmlElement]
        public List<string> FileName { get; set; }
        [XmlElement]
        public List<string> Attachments { get; set; }

        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);


        public CustomScanCSV()
        {
        }

        public CustomScanCSV(string MailType, string Client, string Subject, string DateTimeCreated, string DateTimeModified, List<string> FileName, List<string> Attachments)
        {
            this.MailType = MailType;
            this.Client = Client;
            this.Subject = Subject;
            this.DateTimeCreated = DateTimeCreated;
            this.DateTimeModified = DateTimeModified;
            this.FileName = FileName;
            this.Attachments = Attachments;
        }

        public void saveInfo(CustomScanCSV mCustomMailRecap, string fileName)
        {
            try
            {
                var myfile = File.Create(pathModule + @"\" + fileName);
                XmlSerializer xml = new XmlSerializer(typeof(CustomScanCSV));
                xml.Serialize(myfile, mCustomMailRecap);
                myfile.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
            }
        }
        public void Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(CustomScanCSV));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\" + fileName);
                CustomScanCSV mCustom = new CustomScanCSV();
                mCustom = (CustomScanCSV)reader.Deserialize(file);

                this.MailType = mCustom.MailType;
                this.Client = mCustom.Client;
                this.Subject = mCustom.Subject;
                this.DateTimeCreated = mCustom.DateTimeCreated;
                this.DateTimeModified = mCustom.DateTimeModified;
                this.FileName = mCustom.FileName;
                this.Attachments = mCustom.Attachments;
                file.Close();
            }
        }
    }
}
