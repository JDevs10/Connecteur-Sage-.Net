using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace importPlanifier.Classes
{
    class Veolog_DESADV_Colis
    {
        //Colis
        public string Numero_Ligne;
        public string Numero_Colis;
        public string ID_Tracking_Transporteur;
        public string URL_Tracking_Transporteur;

        public Veolog_DESADV_Colis()
        {

        }
        public Veolog_DESADV_Colis(string Numero_Ligne, string Numero_Colis, string ID_Tracking_Transporteur, string URL_Tracking_Transporteur)
        {
            this.Numero_Ligne = Numero_Ligne;
            this.Numero_Colis = Numero_Colis;
            this.ID_Tracking_Transporteur = ID_Tracking_Transporteur;
            this.URL_Tracking_Transporteur = URL_Tracking_Transporteur;
        }
    }
}
