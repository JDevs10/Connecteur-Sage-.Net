using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Import.Classes
{
    public class DocumentVenteLine
    {
        public string DO_Date { set; get; }
        public string DO_DateLivr { set; get; }
        public string DL_Ligne { set; get; }
        public string AR_Ref { set; get; }
        public string DL_Design { set; get; }
        public string DL_Qte { set; get; }
        public string DL_QteBC { set; get; }
        public string DL_QteBL { set; get; }
        public string DL_QtePL { set; get; }
        public string EU_Qte { set; get; }
        public string DL_PoidsNet { set; get; }
        public string DL_PoidsBrut { set; get; }
        public string DL_Remise01REM_Valeur { set; get; }
        public string DL_Remise01REM_Type { set; get; }
        public string DL_Remise03REM_Valeur { set; get; }
        public string DL_Remise03REM_Type { set; get; }

        /* Type de la remise :
         * 0 à 2
            0 = Montant
            1 = Pourcentage
            2 = Quantité
         * */

        public string DL_PrixUnitaire { set; get; } // Prix unitaire HT

        public string DL_Taxe1 { set; get; }
        public string DL_CodeTaxe1 { set; get; }
        public string DL_Taxe2 { set; get; }
        public string DL_CodeTaxe2 { set; get; }
        public string DL_Taxe3 { set; get; }
        public string DL_CodeTaxe3 { set; get; }
        public string DL_TypeTaxe1 { set; get; }
        public string DL_TypeTaxe2 { set; get; }
        public string DL_TypeTaxe3 { set; get; }
        /* Type taxe :
         * 0 à 4
            0 = TVA/débit
            1 = TVA/Encaissement
            2 = TP/HT
            3 = TP/TTC
            4 = TP/Poids
         * 
         * */
        public string DL_MontantHT { set; get; }
        public string DL_MontantTTC { set; get; }
        public string DL_NoColis { set; get; }

        public string FNT_MontantHT { set; get; } // Montant HT net de la ligne

        public string FNT_MontantTaxes { set; get; } // Montant total des taxes de la ligne
        public string FNT_MontantTTC { set; get; } // Montant TTC de la ligne
        public string FNT_PrixUNet { set; get; } // Prix unitaire net de la ligne
        public string FNT_PrixUNetTTC { set; get; } //Prix unitaire net TTC de la ligne
        public string FNT_RemiseGlobale { set; get; } //Montant de la remise

        public string AR_CODEBARRE { set; get; } // CODE BARRE

        public DocumentVenteLine()
        {

        }

        public DocumentVenteLine(
            string DO_Date,
            string DO_DateLivr,
        string DL_Ligne,
        string AR_Ref,
        string DL_Design,
        string DL_Qte,
        string DL_QteBC,
        string DL_QteBL,
        string DL_QtePL,
        string EU_Qte,
        string DL_PoidsNet,
        string DL_PoidsBrut,
        string DL_Remise01REM_Valeur,
        string DL_Remise01REM_Type,
        string DL_Remise03REM_Valeur,
        string DL_Remise03REM_Type,
        string DL_PrixUnitaire,
        string DL_CodeTaxe1,
        string DL_Taxe1,
        string DL_CodeTaxe2,
        string DL_Taxe2,
        string DL_CodeTaxe3,
        string DL_Taxe3,
        string DL_TypeTaxe1,
        string DL_TypeTaxe2,
        string DL_TypeTaxe3,
        string DL_MontantHT,
        string DL_MontantTTC,
        string DL_NoColis,
        string FNT_MontantHT,
        string FNT_MontantTaxes,
        string FNT_MontantTTC,
        string FNT_PrixUNet,
        string FNT_PrixUNetTTC,
        string FNT_RemiseGlobale,
            string AR_CODEBARRE)
        {
            this.DO_Date = DO_Date;
            this.DO_DateLivr = DO_DateLivr;
            this.DL_Ligne = DL_Ligne;
            this.AR_Ref = AR_Ref;
            this.DL_Design = DL_Design;
            this.DL_Qte = DL_Qte;
            this.DL_QteBC = DL_QteBC;
            this.DL_QteBL = DL_QteBL;
            this.DL_QtePL = DL_QtePL;
            this.EU_Qte = EU_Qte;
            this.DL_PoidsNet = DL_PoidsNet;
            this.DL_PoidsBrut = DL_PoidsBrut;
            this.DL_Remise01REM_Valeur = DL_Remise01REM_Valeur;
            this.DL_Remise01REM_Type = DL_Remise01REM_Type;
            this.DL_Remise03REM_Valeur = DL_Remise03REM_Valeur;
            this.DL_Remise03REM_Type = DL_Remise03REM_Type;
            this.DL_PrixUnitaire = DL_PrixUnitaire;
            this.DL_CodeTaxe1 = DL_CodeTaxe1;
            this.DL_Taxe1 = DL_Taxe1;
            this.DL_CodeTaxe2 = DL_CodeTaxe2;
            this.DL_Taxe2 = DL_Taxe2;
            this.DL_CodeTaxe3 = DL_CodeTaxe3;
            this.DL_Taxe3 = DL_Taxe3;
            this.DL_TypeTaxe1 = DL_TypeTaxe1;
            this.DL_TypeTaxe2 = DL_TypeTaxe2;
            this.DL_TypeTaxe3 = DL_TypeTaxe3;
            this.DL_MontantHT = DL_MontantHT;
            this.DL_MontantTTC = DL_MontantTTC;
            this.DL_NoColis = DL_NoColis;
            this.FNT_MontantHT = FNT_MontantHT;
            this.FNT_MontantTaxes = FNT_MontantTaxes;
            this.FNT_MontantTTC = FNT_MontantTTC;
            this.FNT_PrixUNet = FNT_PrixUNet;
            this.FNT_PrixUNetTTC = FNT_PrixUNetTTC;
            this.FNT_RemiseGlobale = FNT_RemiseGlobale;
            this.AR_CODEBARRE = AR_CODEBARRE;

        }
    }
}
