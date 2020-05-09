using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail.Classes.Configuration
{
    public class EmailSummary
    {
        public Boolean active { get; set; }
        public int hours { get; set; }

        public EmailSummary()
        {
        }

        public EmailSummary(Boolean active, int hours)
        {
            this.active = active;
            this.hours = hours;
        }
    }
}
