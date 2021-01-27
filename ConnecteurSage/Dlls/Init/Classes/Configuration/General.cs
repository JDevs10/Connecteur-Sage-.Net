using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Init.Classes.Configuration
{
    public class General
    {
        public int showWindow;
        public Boolean isAppOpen;
        public Boolean isACP_ComptaCPT_CompteG;
        public int ACP_ComptaCPT_CompteG;

        public General() { }
        public General(int showWindow, Boolean isAppOpen, Boolean isACP_ComptaCPT_CompteG, int ACP_ComptaCPT_CompteG)
        {
            this.showWindow = showWindow;
            this.isAppOpen = isAppOpen;
            this.isACP_ComptaCPT_CompteG = isACP_ComptaCPT_CompteG;
            this.ACP_ComptaCPT_CompteG = ACP_ComptaCPT_CompteG;
        }
    }
}
