using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Model
{
    public class Settings
    {
        public int id;
        public int showWindow;
        public int isACP_ComptaCPT_CompteG; 
        public int ACP_ComptaCPT_CompteG;
        public string EXE_Folder;
        public string EDI_Folder;
        // plannerTask settings
        public string plannerTask_name;
        public string plannerTask_UserId;
        public int plannerTask_active;
        //priceType settings
        public int priceType_active;
        public int priceType_cmdEDIPrice;
        public int priceType_productPrice;
        public int priceType_categoryPrice;
        public int priceType_clientPrice;
        // reprocess settings
        public int reprocess_active;
        public decimal reprocess_hour;
        public int reprocess_countDown;

        public Settings() { }
        public Settings(int id, int showWindow, int isACP_ComptaCPT_CompteG, int ACP_ComptaCPT_CompteG, string EXE_Folder, string EDI_Folder,
            string plannerTask_name, string plannerTask_UserId, int plannerTask_active, 
            int priceType_active, int priceType_cmdEDIPrice, int priceType_productPrice, int priceType_categoryPrice, int priceType_clientPrice,
            int reprocess_active, decimal reprocess_hour, int reprocess_countDown)
        {
            this.id = id;
            this.showWindow = showWindow;
            this.isACP_ComptaCPT_CompteG = isACP_ComptaCPT_CompteG;
            this.ACP_ComptaCPT_CompteG = ACP_ComptaCPT_CompteG;
            this.EXE_Folder = EXE_Folder;
            this.EDI_Folder = EDI_Folder;
            // plannerTask settings
            this.plannerTask_name = plannerTask_name;
            this.plannerTask_UserId = plannerTask_UserId;
            this.plannerTask_active = plannerTask_active;
            // priceType settings
            this.priceType_active = priceType_active;
            this.priceType_cmdEDIPrice = priceType_cmdEDIPrice;
            this.priceType_productPrice = priceType_productPrice;
            this.priceType_categoryPrice = priceType_categoryPrice;
            this.priceType_clientPrice = priceType_clientPrice;
            // reprocess settings
            this.reprocess_active = reprocess_active;
            this.reprocess_hour = reprocess_hour;
            this.reprocess_countDown = reprocess_countDown;
        }
    }
}
