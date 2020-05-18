using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Init.Classes.Configuration
{
    public class Reprocess
    {
        public Boolean activate;
        public int hour;
        public int countDown;

        public Reprocess() { }
        public Reprocess(Boolean activate, int hour, int countDown)
        {
            this.activate = activate;
            this.hour = hour;
            this.countDown = countDown;
        }
    }
}
