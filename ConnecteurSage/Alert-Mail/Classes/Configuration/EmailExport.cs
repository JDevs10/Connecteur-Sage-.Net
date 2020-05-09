using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail.Classes.Configuration
{
    public class EmailExport
    {
        public Boolean active { get; set; }
        public Boolean informClient { get; set; }
        public Boolean informTeam { get; set; }

        public EmailExport()
        {
        }

        public EmailExport(Boolean active, Boolean informClient, Boolean informTeam)
        {
            this.active = active;
            this.informClient = informClient;
            this.informTeam = informTeam;
        }
    }
}
