using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurSage.Classes
{
    public class TVA
    {
        public string TA_Intitule { get; set; }
        public string TA_Taux { get; set; }
        public string CG_Num { get; set; }
        public string TA_Code { get; set; }
        public string cbMarq { get; set; }


        public TVA(string TA_Intitule, string TA_Taux, string CG_Num, string TA_Code, string cbMarq)
        {
            this.TA_Intitule = TA_Intitule;
            this.TA_Taux = TA_Taux;
            this.CG_Num = CG_Num;
            this.TA_Code = TA_Code;
            this.cbMarq = cbMarq;
        }
    }
}
