using Config_Import.Classes.Custom_Doc.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_Import.Classes.Custom_Doc
{
    public class Doc_Ventes
    {
        public Commande Commande;
        public DSADV DSADV;
        public Facture Facture;

        public Doc_Ventes() { }
        public Doc_Ventes(Commande Commande_, DSADV DSADV_, Facture Facture_)
        {
            this.Commande = Commande_;
            this.DSADV = DSADV_;
            this.Facture = Facture_;
        }
    }
}
