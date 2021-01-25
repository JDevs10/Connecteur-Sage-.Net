using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurAuto.Classes
{
    public class Desadv
    {
        public string reference;
        public string datelivraison;
        public string datecreation;
        public string poids;
        public string numerosuivi;
        public string expeditiontype;
        public string referenceclient;

        public Desadv()
        {
        }

        public Desadv(string reference, string datelivraison, string datecreation, string poids, string numerosuivi, string expeditiontype, string referenceclient)
        {
            this.reference = reference;
            this.datelivraison = datelivraison;
            this.datecreation = datecreation;
            this.poids = poids;
            this.numerosuivi = numerosuivi;
            this.expeditiontype = expeditiontype;
            this.referenceclient = referenceclient;
        }

    }
}
