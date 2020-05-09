using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail.Classes.Configuration
{
    public class EmailPendingFiles
    {
        public Boolean active { get; set; }
        public int hours { get; set; }

        public EmailPendingFiles()
        {
        }

        public EmailPendingFiles(Boolean active, int hours)
        {
            this.active = active;
            this.hours = hours;
        }
    }
}
