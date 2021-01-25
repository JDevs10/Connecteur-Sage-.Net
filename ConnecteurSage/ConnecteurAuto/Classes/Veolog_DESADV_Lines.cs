using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurAuto.Classes
{
    public class Veolog_DESADV_Lines
    {
        //Ligne
        public string Numero_Ligne_Order;
        public string Code_Article;
        public string Quantite_Colis;
        public string Numero_Lot;

        public Veolog_DESADV_Lines()
        {

        }
        public Veolog_DESADV_Lines(string Numero_Ligne_Order, string Code_Article, string Quantite_Colis, string Numero_Lot)
        {
            this.Numero_Ligne_Order = Numero_Ligne_Order;
            this.Code_Article = Code_Article;
            this.Quantite_Colis = Quantite_Colis;
            this.Numero_Lot = Numero_Lot;
        }
    }
}
