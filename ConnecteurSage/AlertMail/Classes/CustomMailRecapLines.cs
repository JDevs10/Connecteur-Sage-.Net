using System.Xml.Serialization;

namespace AlertMail.Classes
{
    public class CustomMailRecapLines
    {
        [XmlElement]
        public int LineNumber { set; get; }
        [XmlElement]
        public string DocumentReference { set; get; }
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

        public CustomMailRecapLines(string DocumentReference, string DocumentErrorMessage, string DocumentErrorMessageDebug, string DocumentErrorStackTraceDebug, string FileName, string FilePath)
        {
            this.DocumentReference = DocumentReference;
            this.DocumentErrorMessage = DocumentErrorMessage;
            this.DocumentErrorMessageDebug = DocumentErrorMessageDebug;
            this.DocumentErrorStackTraceDebug = DocumentErrorStackTraceDebug;
            this.FileName = FileName;
            this.FilePath = FilePath;
        }
    }
}