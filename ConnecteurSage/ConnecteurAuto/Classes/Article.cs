using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurAuto.Classes
{
    public class Article
    {
        public string AR_REF { get; set; }
        public string AR_CodeBarre { get; set; }
        public string gamme1 { get; set; }
        public string gamme2 { get; set; }
        public string AR_SuiviStock { get; set; }
        public string AR_Nomencl { get; set; }
        public string AR_StockId { get; set; }
        public string AR_REFCompose { get; set; }
        public string RP_CODEDEFAUT { get; set; }
        public string AR_PRIXVEN { get; set; }
        public string AR_POIDSBRUT { get; set; }
        public string AR_POIDSNET { get; set; }
        public string AR_UnitePoids { get; set; }
        public string AR_DESIGN { get; set; }
        public Conditionnement Conditionnement = null;
        public string DL_CodeTaxe1 { get; set; }
        public string DL_Taxe1 { get; set; }
        public string DL_CodeTaxe2 { get; set; }
        public string DL_Taxe2 { get; set; }
        public string DL_CodeTaxe3 { get; set; }
        public string DL_Taxe3 { get; set; }
        public string DL_PrixUnitaire_salePriceHT { get; set; } // Prix HT
        public string DL_PUTTC { get; set; }    // Prix TTC

        public Article()
        {
        }

        public Article(string AR_REF, string AR_CodeBarre, string AR_SuiviStock, string gamme1, string gamme2, string AR_Nomencl, string RP_CODEDEFAUT, string AR_PRIXVEN, string AR_POIDSBRUT, string AR_POIDSNET, string AR_UnitePoids, string AR_DESIGN, string DL_PrixUnitaire_salePriceHT)
        {
            this.AR_REF = AR_REF;
            this.AR_CodeBarre = AR_CodeBarre;
            this.gamme1 = gamme1;
            this.gamme2 = gamme2;
            this.AR_SuiviStock = AR_SuiviStock;
            this.AR_Nomencl = AR_Nomencl;
            this.AR_REFCompose = "";
            this.RP_CODEDEFAUT = RP_CODEDEFAUT;
            this.AR_PRIXVEN = AR_PRIXVEN;
            this.AR_POIDSBRUT = AR_POIDSBRUT;
            this.AR_UnitePoids = AR_UnitePoids;
            this.AR_POIDSNET = AR_POIDSNET;
            this.AR_DESIGN = AR_DESIGN;
            this.DL_PrixUnitaire_salePriceHT = DL_PrixUnitaire_salePriceHT;
        }
    }

    public class Conditionnement
    {
        // Conditionnement ===>
        public string EC_ENUMERE { get; set; }
        public string EC_QUANTITE { get; set; }
        // <===

        public Conditionnement(string EC_ENUMERE, string EC_QUANTITE)
        {
            this.EC_ENUMERE = EC_ENUMERE;
            this.EC_QUANTITE = EC_QUANTITE;
        }

        public Conditionnement()
        {
            // TODO: Complete member initialization
        }
    }
}
