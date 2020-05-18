using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail.Classes.Custom
{
    public class CustomMailSuccess
    {
        public int ID { set; get; }
        public string MailType { set; get; }
        public string Client { set; get; }
        public string Subject { set; get; }
        public string DateTimeCreated { set; get; }
        public string DateTimeModified { set; get; }
        public string DateTimeDelivery { set; get; }
        public string DocumentReference { set; get; }
        public string NumCommande { set; get; }
        public string FileName { set; get; }
        public string FilePath { set; get; }
        public List<CustomMailSuccessLines> Lines { get; set; }

        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);


        public CustomMailSuccess() { }
        public CustomMailSuccess(string MailType, string Client, string Subject, string DateTimeCreated, string DateTimeModified, string DateTimeDelivery, string DocumentReference, string NumCommande, string FileName, string FilePath, List<CustomMailSuccessLines> Lines)
        {
            this.MailType = MailType;
            this.Client = Client;
            this.Subject = Subject;
            this.DateTimeCreated = DateTimeCreated;
            this.DateTimeModified = DateTimeModified;
            this.DateTimeDelivery = DateTimeDelivery;
            this.DocumentReference = DocumentReference;
            this.NumCommande = NumCommande;
            this.FileName = FileName;
            this.FilePath = FilePath;
            this.Lines = Lines;
        }
    }
}
