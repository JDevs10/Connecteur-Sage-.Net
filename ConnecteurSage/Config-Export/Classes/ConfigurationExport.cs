using Config_Export.Classes.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_Export.Classes
{
    public class ConfigurationExport
    {
        public Commande Commande;
        public DSADV DSADV;
        public Facture Facture;
        public Stock Stock;

        public ConfigurationExport() { }
        public ConfigurationExport(Commande Commande_, DSADV DSADV_, Facture Facture_, Stock Stock_)
        {
            this.Commande = Commande_;
            this.DSADV = DSADV_;
            this.Facture = Facture_;
            this.Stock = Stock_;
        }
    }
}
