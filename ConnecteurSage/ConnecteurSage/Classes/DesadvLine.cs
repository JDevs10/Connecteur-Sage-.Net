using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurSage.Classes
{
    public class DesadvLine
    {

        public string libelle;
        public string barcode;
        public string qtecommandee;
        public string qtyexpediee;
        public string entrepot;
        public string poidproduit;
        public string volumeproduit;
        public string position;

        public DesadvLine()
        {
        }

        public DesadvLine(string libelle, string barcode, string qtecommandee, string qtyexpediee, string entrepot, string poidproduit, string volumeproduit, string position)
        {
            this.libelle = libelle;
            this.barcode = barcode;
            this.qtecommandee = qtecommandee;
            this.qtyexpediee = qtyexpediee;
            this.entrepot = entrepot;
            this.poidproduit = poidproduit;
            this.volumeproduit = volumeproduit;
            this.position = position;
        }

    }
}
