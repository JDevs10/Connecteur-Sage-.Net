using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurAuto.Classes
{
    class DocumentVente
    {
        public string DO_Piece { set; get; }
        public string DO_TIERS { set; get; }
        public string DO_date { set; get; }
        public string DO_dateLivr { set; get; }
        public string DO_devise { set; get; }
        public string LI_No { set; get; }

        public string DO_Statut { set; get; }
        /* Statut du document :
           0 à 2
           0 = Saisie
           1 = Confirmé
           2 = Accepté
        */
        public string DO_COORD01 { set; get; }

        public string DO_taxe1 { set; get; }
        public string DO_taxe2 { set; get; }
        public string DO_taxe3 { set; get; }

        public string DO_TypeTaxe1 { set; get; }
        public string DO_TypeTaxe2 { set; get; }
        public string DO_TypeTaxe3 { set; get; }

        /* Type de taxe frais :
         * 0 à 4
            0 = TVA/Débit
            1 = TVA/Encaissement
            2 = TP/HT
            3 = TP/TTC
            4 = TP/Poids
         */

        public string FNT_MontantEcheance { set; get; }
        public string FNT_MontantTotalTaxes { set; get; }
        public string FNT_NetAPayer { set; get; }

        public string FNT_PoidsBrut { set; get; }
        public string FNT_PoidsNet { set; get; }

        public string FNT_Escompte { set; get; }
        public string FNT_TotalHT { set; get; }
        public string FNT_TotalHTNet { set; get; }

        public string FNT_TotalTTC { set; get; }

        public string LI_ADRESSE { set; get; }
        public string LI_CODEPOSTAL { set; get; }
        public string LI_CODEREGION { set; get; }
        public string LI_COMPLEMENT { set; get; }
        public string LI_VILLE { set; get; }
        public string LI_PAYS { set; get; }
        public string C_MODE { set; get; }
        public string DO_MOTIF { set; get; }
        public string LI_Intitule { set; get; }
        public string do_txescompte { set; get; }
        public string ca_num { set; get; }

        public List<DocumentVenteLine> lines { set; get; }

        public DocumentVente()
        {

        }

        public DocumentVente(string DO_Piece,
        string DO_TIERS,
        string DO_date,
        string DO_dateLivr,
        string DO_devise,
        string LI_No,
        string DO_Statut,
        string DO_taxe1,
        string DO_taxe2,
        string DO_taxe3,
        string DO_TypeTaxe1,
        string DO_TypeTaxe2,
        string DO_TypeTaxe3,
        string FNT_MontantEcheance,
        string FNT_MontantTotalTaxes,
        string FNT_NetAPayer,
        string FNT_PoidsBrut,
        string FNT_PoidsNet,
        string FNT_Escompte,
        string FNT_TotalHT,
        string FNT_TotalHTNet,
        string FNT_TotalTTC,
        string LI_ADRESSE,
        string LI_CODEPOSTAL,
        string LI_CODEREGION,
        string LI_COMPLEMENT,
        string LI_VILLE,
        string LI_PAYS,
        string C_MODE,
        string DO_COORD01,
        string LI_Intitule,
        string do_motif,
        string do_txescompte,
        string ca_num)
        {
            this.DO_Piece = DO_Piece;     
            this.DO_TIERS = DO_TIERS;
            this.DO_date = DO_date;
            this.DO_dateLivr = DO_dateLivr;
            this.DO_devise = DO_devise;
            this.LI_No = LI_No;
            this.DO_Statut = DO_Statut;
            this.DO_taxe1 = DO_taxe1;
            this.DO_taxe2 = DO_taxe2;
            this.DO_taxe3 = DO_taxe3;
            this.DO_TypeTaxe1 = DO_TypeTaxe1;
            this.DO_TypeTaxe2 = DO_TypeTaxe2;
            this.DO_TypeTaxe3 = DO_TypeTaxe3;
            this.FNT_MontantEcheance = FNT_MontantEcheance;
            this.FNT_MontantTotalTaxes = FNT_MontantTotalTaxes;
            this.FNT_NetAPayer = FNT_NetAPayer;
            this.FNT_PoidsBrut = FNT_PoidsBrut;
            this.FNT_PoidsNet = FNT_PoidsNet;
            this.FNT_Escompte = FNT_Escompte;
            this.FNT_TotalHT = FNT_TotalHT;
            this.FNT_TotalHTNet = FNT_TotalHTNet;
            this.FNT_TotalTTC = FNT_TotalTTC;
            this.LI_ADRESSE = LI_ADRESSE;
            this.LI_CODEPOSTAL = LI_CODEPOSTAL;
            this.LI_CODEREGION = LI_CODEREGION;
            this.LI_COMPLEMENT = LI_COMPLEMENT;
            this.LI_VILLE = LI_VILLE;
            this.LI_PAYS = LI_PAYS;
            this.C_MODE = C_MODE;
            this.DO_COORD01 = DO_COORD01;
            this.LI_Intitule = LI_Intitule;
            this.DO_MOTIF = do_motif;
            this.do_txescompte = do_txescompte;
            this.ca_num = ca_num;
        }

//        public DocumentVente(string DO_Piece,
//string DO_TIERS,
//string DO_date,
//string DO_dateLivr,
//string DO_devise,
//string LI_No,
//string DO_Statut,
//string DO_taxe1,
//string DO_taxe2,
//string DO_taxe3,
//string DO_TypeTaxe1,
//string DO_TypeTaxe2,
//string DO_TypeTaxe3,
//string FNT_MontantEcheance,
//string FNT_MontantTotalTaxes,
//string FNT_NetAPayer,
//string FNT_PoidsBrut,
//string FNT_PoidsNet,
//string FNT_Escompte,
//string FNT_TotalHT,
//string FNT_TotalHTNet,
//string FNT_TotalTTC,
//string LI_ADRESSE,
//string LI_CODEPOSTAL,
//string LI_CODEREGION,
//string LI_COMPLEMENT,
//string LI_VILLE,
//string LI_PAYS,
//string C_MODE)
//        {
//            this.DO_Piece = DO_Piece;
//            this.DO_TIERS = DO_TIERS;
//            this.DO_date = DO_date;
//            this.DO_dateLivr = DO_dateLivr;
//            this.DO_devise = DO_devise;
//            this.LI_No = LI_No;
//            this.DO_Statut = DO_Statut;
//            this.DO_taxe1 = DO_taxe1;
//            this.DO_taxe2 = DO_taxe2;
//            this.DO_taxe3 = DO_taxe3;
//            this.DO_TypeTaxe1 = DO_TypeTaxe1;
//            this.DO_TypeTaxe2 = DO_TypeTaxe2;
//            this.DO_TypeTaxe3 = DO_TypeTaxe3;
//            this.FNT_MontantEcheance = FNT_MontantEcheance;
//            this.FNT_MontantTotalTaxes = FNT_MontantTotalTaxes;
//            this.FNT_NetAPayer = FNT_NetAPayer;
//            this.FNT_PoidsBrut = FNT_PoidsBrut;
//            this.FNT_PoidsNet = FNT_PoidsNet;
//            this.FNT_Escompte = FNT_Escompte;
//            this.FNT_TotalHT = FNT_TotalHT;
//            this.FNT_TotalHTNet = FNT_TotalHTNet;
//            this.FNT_TotalTTC = FNT_TotalTTC;
//            this.LI_ADRESSE = LI_ADRESSE;
//            this.LI_CODEPOSTAL = LI_CODEPOSTAL;
//            this.LI_CODEREGION = LI_CODEREGION;
//            this.LI_COMPLEMENT = LI_COMPLEMENT;
//            this.LI_VILLE = LI_VILLE;
//            this.LI_PAYS = LI_PAYS;
//            this.C_MODE = C_MODE;
//        }
    }
}
