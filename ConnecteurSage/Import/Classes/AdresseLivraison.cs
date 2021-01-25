using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Import.Classes
{
    public class AdresseLivraison
    {
        public string Li_no { get; set; }
        public string Nom_contact { get; set; }
        public string adresse { get; set; }
        public string codePostale { get; set; }
        public string ville { get; set; }
        public string pays { get; set; }
        public string condition { get; set; }
        public string intitule { get; set; }
        public string CT_NUM { get; set; }

        public AdresseLivraison(int i, string CT_NUM,
     string Nom_contact,
  string adresse,
  string codePostale,
  string ville,
  string pays)
        {
            this.CT_NUM = CT_NUM;
            this.Nom_contact = Nom_contact;
            this.adresse = adresse;
            this.codePostale = codePostale;
            this.ville = ville;
            this.pays = pays;
        }

        //public AdresseLivraison(string Li_no,
        //    string Nom_contact ,
        // string adresse ,
        // string codePostale ,
        // string ville ,
        // string pays,
        //    string condition)
        //{
        //    this.Li_no = Li_no;
        // this.Nom_contact =  Nom_contact ;
        // this.adresse = adresse  ;
        // this.codePostale = codePostale  ;
        // this.ville =  ville ;
        // this.pays = pays;
        // this.condition = condition;
        //}

        public AdresseLivraison(string Li_no,
          string Nom_contact,
       string adresse,
       string codePostale,
       string ville,
       string pays,
          string condition,
            string intitule)
        {
            this.Li_no = Li_no;
            this.Nom_contact = Nom_contact;
            this.adresse = adresse;
            this.codePostale = codePostale;
            this.ville = ville;
            this.pays = pays;
            this.condition = condition;
            this.intitule = intitule;
        }

        public AdresseLivraison(int i,int j,
            string CT_NUM,
            string Li_no,
            string intitule,
        string Nom_contact,
     string adresse,
     string codePostale,
     string ville,
     string pays)

        {
            this.Li_no = Li_no;
            this.Nom_contact = Nom_contact;
            this.adresse = adresse;
            this.codePostale = codePostale;
            this.ville = ville;
            this.pays = pays;
            this.CT_NUM = CT_NUM;
            this.intitule = intitule;
        }
    }
}
