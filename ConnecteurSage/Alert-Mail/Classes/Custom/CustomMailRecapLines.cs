using System.Xml.Serialization;

namespace Alert_Mail.Classes.Custom
{
    public class CustomMailRecapLines
    {
        public int LineNumber { set; get; }
        public string DocumentReference { set; get; }
        public string NumCommande { set; get; }
        public string DocumentErrorMessage { set; get; }
        public string DocumentErrorMessageDebug { set; get; }
        public string DocumentErrorStackTraceDebug { set; get; }
        public string FileName { set; get; }
        public string FilePath { set; get; }
        public int Increment { set; get; }


        public CustomMailRecapLines(){ }
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