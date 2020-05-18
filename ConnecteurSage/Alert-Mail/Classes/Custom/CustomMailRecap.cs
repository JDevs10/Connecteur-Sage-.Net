using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Alert_Mail.Classes.Custom
{
    public class CustomMailRecap
    {
        public string MailType { set; get; }
        public string Client { set; get; }
        public string Subject { set; get; }
        public string DateTimeCreated { set; get; }
        public string DateTimeModified { set; get; }
        public List<CustomMailRecapLines> Lines { get; set; }
        public List<string> Attachments { get; set; }

        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);


        public CustomMailRecap(){ }
        public CustomMailRecap(string MailType, string Client, string Subject, string DateTimeCreated, string DateTimeModified, List<CustomMailRecapLines> Lines, List<string> Attachments)
        {
            this.MailType = MailType;
            this.Client = Client;
            this.Subject = Subject;
            this.DateTimeCreated = DateTimeCreated;
            this.DateTimeModified = DateTimeModified;
            this.Lines = Lines;
            this.Attachments = Attachments;
        }

    }
}
