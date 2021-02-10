using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.Classes
{
    public class DocumentAchatLine
    {
        public string AR_Ref { set; get; }
        public string AR_CODEBARRE { set; get; }
        public string DL_Qte { set; get; }
        public string DL_QtePL { set; get; }
        public string EU_Qte { set; get; }
        

        public DocumentAchatLine() { }
        public DocumentAchatLine(string AR_Ref_, string AR_CODEBARRE_, string DL_Qte_, string DL_QtePL_, string EU_Qte_) 
        {
            this.AR_Ref = AR_Ref;
            this.AR_CODEBARRE = AR_CODEBARRE_;
            this.DL_Qte = DL_Qte_;
            this.DL_QtePL = DL_QtePL_;
            this.EU_Qte = EU_Qte_;
        }
    }
}
