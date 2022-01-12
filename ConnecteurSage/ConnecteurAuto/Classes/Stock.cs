using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurAuto.Classes
{
    public class Stock
    {
        public string libelle;
        public string reference;
        public string codebarre;
        public string stock;
        public string numerolot;
        public string lotqty;
        public string lotepuise;
        public string DE_No;
        public string DE_No_Name;

        public Stock()
        {
        }

        public Stock(string libelle, string reference, string codebarre, string stock, string numerolot, string lotqty, string lotepuise)
        {

            this.libelle = libelle;
            this.reference = reference;
            this.codebarre = codebarre;
            this.stock = stock;
            this.numerolot = numerolot;
            this.lotqty = lotqty;
            this.lotepuise = lotepuise;

        }
    }
}
