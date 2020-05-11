using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail.Classes.Configuration
{
    public class EmailLists
    {
        public Boolean activeClient { get; set; }
        public List<string> emailClientList { get; set; }
        public Boolean activeTeam { get; set; }
        public List<string> emailTeamList { get; set; }

        public EmailLists(){ }
        public EmailLists(Boolean activeClient, List<string> emailClientList, Boolean activeTeam, List<string> emailTeamList)
        {
            this.activeClient = activeClient;
            this.emailClientList = emailClientList;
            this.activeTeam = activeTeam;
            this.emailTeamList = emailTeamList;
        }
    }
}
