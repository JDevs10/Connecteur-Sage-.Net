using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export.Classes
{
    public static class QueryHelper
    {
        public static string getPrefix()
        {
            string result = "";
            string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            Connexion.ConnexionSaveLoad connexionSaveLoad = new Connexion.ConnexionSaveLoad();
            if (connexionSaveLoad.isSettings())
            {
                connexionSaveLoad.Load();
                result = connexionSaveLoad.configurationConnexion.SQL.PREFIX + ".dbo.";
            }

            return result;
        }

        #region SQL Queries

        public static string getAllClients(bool sqlConnexion)
        {
            if (sqlConnexion)
            {
                return "SELECT CT_Num, CG_NumPrinc, CT_NumPayeur, N_Condition, N_Devise, N_Expedition, CT_Langue, CT_Facture, N_Period, N_CatTarif, CT_Taux02, N_CatCompta, CT_NumCentrale, CT_Intitule, CO_No, CT_EdiCode FROM " + getPrefix() + "F_COMPTET";
            }
            else
            {
                return "SELECT CT_Num, CG_NumPrinc, CT_NumPayeur, N_Condition, N_Devise, N_Expedition, CT_Langue, CT_Facture, N_Period, N_CatTarif, CT_Taux02, N_CatCompta, CT_NumCentrale, CT_Intitule, CO_No, CT_EdiCode FROM F_COMPTET";
            }
        }
                        
        public static string getAllProducts(bool sqlConnexion)
        {
            if (sqlConnexion)
            {
                return "SELECT AR_EdiCode, AR_CodeBarre, AR_Ref, AR_Design, AR_PoidsBrut, AR_PoidsNet, AR_PrixAch, AR_PrixVen, AR_PrixTTC, AR_Pays FROM " + getPrefix() + "F_ARTICLE";
            }
            else
            {
                return "SELECT AR_EdiCode, AR_CodeBarre, AR_Ref, AR_Design, AR_PoidsBrut, AR_PoidsNet, AR_PrixAch, AR_PrixVen, AR_PrixTTC, AR_Pays FROM F_ARTICLE";
            }
        }

        #endregion
    }
}
