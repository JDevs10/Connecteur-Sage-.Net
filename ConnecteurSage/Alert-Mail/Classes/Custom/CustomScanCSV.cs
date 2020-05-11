using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Alert_Mail.Classes.Custom
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

    }
}
