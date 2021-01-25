using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurAuto.Classes
{
    public class Veolog_BCF
    {
        //Entete
        public string Ref_Commande_Donneur_Ordre;
        public string Ref_Commande_Fournisseur;
        public string Origine_Commande;
        public string Code_Fournisseur;
        public string Date_De_Reception;
        public string Heure_De_Reception;
        public string Etat;

        public Veolog_BCF()
        {

        }
        public Veolog_BCF(string Commande_Donneur_Ordre, string Ref_Commande_Fournisseur, string Origine_Commande, string Code_Fournisseur, string Date_De_Reception, string Heure_De_Reception, string Etat)
        {
            this.Ref_Commande_Donneur_Ordre = Commande_Donneur_Ordre;
            this.Ref_Commande_Fournisseur = Ref_Commande_Fournisseur;
            this.Origine_Commande = Origine_Commande;
            this.Code_Fournisseur = Code_Fournisseur;
            this.Date_De_Reception = Date_De_Reception;
            this.Heure_De_Reception = Heure_De_Reception;
            this.Etat = Etat;
        }

    }
}
