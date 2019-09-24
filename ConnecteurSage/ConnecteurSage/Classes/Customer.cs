using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurSage.Classes
{
    class Customer
    {
        public string CT_Num { set; get; }
        public string CT_Intitule { set; get; }
        public string CT_Adresse { set; get; }
        public string CT_APE { set; get; }
        public string CAPITAL_SOCIAL { set; get; }
        public string CT_CodePostal { set; get; }
        public string CT_CodeRegion { set; get; }
        public string CT_Complement { set; get; }
        public string CT_CONTACT { set; get; }
        public string CT_EdiCode { set; get; }
        public string CT_email { set; get; }
        public string CT_Identifiant { set; get; }
        public string CT_Ville { set; get; }
        public string CT_Pays { set; get; }
        public string CT_Siret { set; get; }
        public string CT_Telephone { set; get; }
        public string N_DEVISE { set; get; }
        public string CT_SvFormeJuri { set; get; }

        public Customer()
        {

        }


        public Customer(string CT_Num,
         string CT_Intitule,
         string CT_Adresse,
         string CT_APE,
         string CT_CodePostal,
         string CT_CodeRegion,
         string CT_Complement,
         string CT_CONTACT,
         string CT_EdiCode,
         string CT_email,
         string CT_Identifiant,
         string CT_Ville,
         string CT_Pays,
         string CT_Siret,
         string CT_Telephone,
         string N_DEVISE,
         string CT_SvFormeJuri)
        {
            this.CT_Num = CT_Num;
            this.CT_Intitule = CT_Intitule;
            this.CT_Adresse = CT_Adresse;
            this.CT_APE = CT_APE;
            this.CT_CodePostal = CT_CodePostal;
            this.CT_CodeRegion = CT_CodeRegion;
            this.CT_Complement = CT_Complement;
            this.CT_CONTACT = CT_CONTACT;
            this.CT_EdiCode = CT_EdiCode;
            this.CT_email = CT_email;
            this.CT_Identifiant = CT_Identifiant;
            this.CT_Ville = CT_Ville;
            this.CT_Pays = CT_Pays;
            this.CT_Siret = CT_Siret;
            this.CT_Telephone = CT_Telephone;
            this.N_DEVISE = N_DEVISE;
            this.CT_SvFormeJuri = CT_SvFormeJuri;


        }
    }
}
