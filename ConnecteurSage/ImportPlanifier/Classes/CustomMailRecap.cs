using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ImportPlanifier.Classes
{
    public class CustomMailRecap
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
        public List<CustomMailRecapLines> Lines { get; set; }
        [XmlElement]
        public List<string> Attachments { get; set; }

        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public CustomMailRecap()
        {
        }

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

        public void saveInfo(CustomMailRecap mCustomMailRecap, string fileName)
        {
            try
            {
                var myfile = File.Create(pathModule + @"\"+ fileName);
                XmlSerializer xml = new XmlSerializer(typeof(CustomMailRecap));
                xml.Serialize(myfile, mCustomMailRecap);
                myfile.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("********** Erreur SaveInfo(CustomMailRecap)  **********");
                Console.WriteLine("" + ex.Message);
                Console.WriteLine("" + ex.StackTrace);
            }
        }
        public void Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(CustomMailRecap));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\" + fileName);
                CustomMailRecap mCustomMailRecap = new CustomMailRecap();
                mCustomMailRecap = (CustomMailRecap)reader.Deserialize(file);

                this.MailType = mCustomMailRecap.MailType;
                this.Client = mCustomMailRecap.Client;
                this.Subject = mCustomMailRecap.Subject;
                this.DateTimeCreated = mCustomMailRecap.DateTimeCreated;
                this.DateTimeModified = mCustomMailRecap.DateTimeModified;
                this.Lines = mCustomMailRecap.Lines;
                this.Attachments = mCustomMailRecap.Attachments;
                file.Close();
            }
        }
    }

    public class CustomMailRecapLines
    {
        [XmlElement]
        public int LineNumber { set; get; }
        [XmlElement]
        public string DocumentReference { set; get; }
        [XmlElement]
        public string NumCommande { set; get; }
        [XmlElement]
        public string DocumentErrorMessage { set; get; }
        [XmlElement]
        public string DocumentErrorMessageDebug { set; get; }
        [XmlElement]
        public string DocumentErrorStackTraceDebug { set; get; }
        [XmlElement]
        public string FileName { set; get; }
        [XmlElement]
        public string FilePath { set; get; }
        [XmlElement]
        public int Increment { set; get; }

        public CustomMailRecapLines()
        {
        }

        public CustomMailRecapLines(string DocumentReference, string NumCommande, string DocumentErrorMessage, string DocumentErrorMessageDebug, string DocumentErrorStackTraceDebug, string FileName, string FilePath)
        {
            this.DocumentReference = DocumentReference;
            this.NumCommande = NumCommande;
            this.DocumentErrorMessage = DocumentErrorMessage;
            this.DocumentErrorMessageDebug = DocumentErrorMessageDebug;
            this.DocumentErrorStackTraceDebug = DocumentErrorStackTraceDebug;
            this.FileName = FileName;
            this.FilePath = FilePath;
        }
    }
}
