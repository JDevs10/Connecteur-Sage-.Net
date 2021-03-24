using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Export.Classes
{
    public class Article
    {
        public string AR_EdiCode { get; set; }
        public string AR_CodeBarre { get; set; }
        public string AR_Ref { get; set; }
        public string AR_Design { get; set; }
        public string AR_PoidsBrut { get; set; }
        public string AR_PoidsNet { get; set; }
        public string AR_PrixAch { get; set; }
        public string AR_PrixVen { get; set; }
        public string AR_PrixTTC { get; set; }
        public string AR_Pays { get; set; }

        public Article()
        {
        }

        public Article(string AR_EdiCode, string AR_CodeBarre, string AR_Ref, string AR_Design, string AR_PoidsBrut, string AR_PoidsNet, string AR_PrixAch, string AR_PrixVen, string AR_PrixTTC, string AR_Pays)
        {
            this.AR_EdiCode = AR_EdiCode;
            this.AR_CodeBarre = AR_CodeBarre;
            this.AR_Ref = AR_Ref;
            this.AR_Design = AR_Design;
            this.AR_PoidsBrut = AR_PoidsBrut;
            this.AR_PoidsNet = AR_PoidsNet;
            this.AR_PrixAch = AR_PrixAch;
            this.AR_PrixVen = AR_PrixVen;
            this.AR_PrixTTC = AR_PrixTTC;
            this.AR_Pays = AR_Pays;
        }
    }

}
