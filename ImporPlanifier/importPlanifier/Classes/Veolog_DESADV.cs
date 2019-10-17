using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace importPlanifier.Classes
{
    public class Veolog_DESADV
    {
        //Entete
        public string Commande_Donneur_Ordre;
        public string Commande_Client_Livre;
        public string Date_De_Expedition;
        public string Heure_De_Expedition;
        public string Etat;

        public Veolog_DESADV()
        {

        }
        public Veolog_DESADV(string Commande_Donneur_Ordre, string Commande_Client_Livre, string Date_De_Expedition, string Heure_De_Expedition, string Etat)
        {
            this.Commande_Donneur_Ordre = Commande_Donneur_Ordre;
            this.Commande_Client_Livre = Commande_Client_Livre;
            this.Date_De_Expedition = Date_De_Expedition;
            this.Heure_De_Expedition = Heure_De_Expedition;
            this.Etat = Etat;
        }


    }
}
