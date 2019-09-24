using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurSage.Classes
{
    public class Client
    {
        #region Propriétés

        public string CT_Num { get; set; }

        public string CG_NumPrinc { get; set; }

        public string CT_NumPayeur { get; set; }

        public string N_Condition { get; set; }

        public string N_Devise { get; set; }

        public string N_Expedition { get; set; }

        public string CT_Langue { get; set; }

        public string CT_Facture { get; set; }

        public string N_Period { get; set; }

        public string N_CatTarif { get; set; }

        public string CT_Taux02 { get; set; }

        public string N_CatCompta { get; set; }

        public string CT_EdiCode { get; set; }

        public string DE_NO { get; set; }

        #endregion


        public Client(string CT_Num, string CG_NumPrinc, string CT_NumPayeur, string N_Condition, string N_Devise, string N_Expedition, string CT_Langue, string CT_Facture, string N_Period, string N_CatTarif, string CT_Taux02, string N_CatCompta)
        {
            this.CT_Num=CT_Num;
            this.CG_NumPrinc = CG_NumPrinc;
            this.CT_NumPayeur = CT_NumPayeur;
            this.N_Condition = N_Condition;
            this.N_Devise = N_Devise;
            this.N_Expedition = N_Expedition;
            this.CT_Langue = CT_Langue;
            this.CT_Facture = CT_Facture;
            this.N_Period = N_Period;
            this.N_CatTarif = N_CatTarif;
            this.CT_Taux02 = CT_Taux02;
            this.N_CatCompta = N_CatCompta;
        }


        public Client(string CT_Num, string CT_EdiCode, string CG_NumPrinc, string CT_NumPayeur, string N_Condition, string N_Devise, string N_Expedition, string CT_Langue, string CT_Facture, string N_Period, string N_CatTarif, string CT_Taux02, string N_CatCompta)
        {
            this.CT_Num = CT_Num;
            this.CG_NumPrinc = CG_NumPrinc;
            this.CT_NumPayeur = CT_NumPayeur;
            this.N_Condition = N_Condition;
            this.N_Devise = N_Devise;
            this.N_Expedition = N_Expedition;
            this.CT_Langue = CT_Langue;
            this.CT_Facture = CT_Facture;
            this.N_Period = N_Period;
            this.N_CatTarif = N_CatTarif;
            this.CT_Taux02 = CT_Taux02;
            this.N_CatCompta = N_CatCompta;
            this.CT_EdiCode = CT_EdiCode;
        }

        public Client(int A,string CT_Num, string CG_NumPrinc, string CT_NumPayeur, string N_Condition, string N_Devise, string N_Expedition, string CT_Langue, string CT_Facture, string N_Period, string N_CatTarif, string CT_Taux02, string N_CatCompta, string DE_NO)
        {
            this.CT_Num = CT_Num;
            this.CG_NumPrinc = CG_NumPrinc;
            this.CT_NumPayeur = CT_NumPayeur;
            this.N_Condition = N_Condition;
            this.N_Devise = N_Devise;
            this.N_Expedition = N_Expedition;
            this.CT_Langue = CT_Langue;
            this.CT_Facture = CT_Facture;
            this.N_Period = N_Period;
            this.N_CatTarif = N_CatTarif;
            this.CT_Taux02 = CT_Taux02;
            this.N_CatCompta = N_CatCompta;
            this.DE_NO = DE_NO;
        }

    }
}
