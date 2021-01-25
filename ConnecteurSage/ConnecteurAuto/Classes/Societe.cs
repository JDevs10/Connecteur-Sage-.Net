using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurAuto.Classes
{
    public class Societe
    {
        public string D_RaisonSoc { set; get; }
        public string D_Adresse { set; get; }
        public string D_CodePostal { set; get; }
        public string D_Ville { set; get; }
        public string D_Pays { set; get; }
        public string D_Siret { set; get; }
        public string D_Identifiant { set; get; }
        public string D_Commentaire { set; get; }

        public Societe(string D_RaisonSoc, string D_Adresse, string D_CodePostal, string D_Ville, string D_Pays, string D_Siret, string D_Identifiant, string D_Commentaire)
        {
            this.D_RaisonSoc = D_RaisonSoc;
            this.D_Adresse = D_Adresse;
            this.D_CodePostal = D_CodePostal;
            this.D_Identifiant = D_Identifiant;
            this.D_Siret = D_Siret;
            this.D_Pays = D_Pays;
            this.D_Ville = D_Ville;
            this.D_Commentaire = D_Commentaire;
        }

        public Societe()
        {
            // TODO: Complete member initialization
        }
    }
}
