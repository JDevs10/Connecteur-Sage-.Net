using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail.Classes.Configuration
{
    public class EmailImport
    {
        public Boolean active { get; set; }
        public Boolean eachDocument { get; set; }
        public Boolean atTheEnd { get; set; }
        public Boolean informClient { get; set; }
        public Boolean informTeam { get; set; }

        public EmailImport()
        {

        }

        public EmailImport(Boolean active, Boolean eachDocument, Boolean atTheEnd, Boolean informClient, Boolean informTeam)
        {
            this.active = active;
            this.eachDocument = eachDocument;
            this.atTheEnd = atTheEnd;
            this.informClient = informClient;
            this.informTeam = informTeam;
        }
    }
}
