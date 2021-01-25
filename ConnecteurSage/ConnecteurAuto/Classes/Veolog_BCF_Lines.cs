using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurAuto.Classes
{
    class Veolog_BCF_Lines
    {
        //Colis
        public string Type_Ligne;
        public string Numero_Ligne_Donneur_Ordre;
        public string Code_Article;
        public string Libelle_Article;
        public string Quantite;
        public string Numero_Lot;

        public Veolog_BCF_Lines()
        {

        }
        public Veolog_BCF_Lines(string Type_Ligne, string Numero_Ligne_Donneur_Ordre, string Code_Article, string Libelle_Article, string Quantite, string Numero_Lot)
        {
            this.Type_Ligne = Type_Ligne;
            this.Numero_Ligne_Donneur_Ordre = Numero_Ligne_Donneur_Ordre;
            this.Code_Article = Code_Article;
            this.Libelle_Article = Libelle_Article;
            this.Quantite = Quantite;
            this.Numero_Lot = Numero_Lot;
        }
    }
}
