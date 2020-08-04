using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Model
{
    public class AlertMailLog
    {
        public int id;
        public string log;

        public AlertMailLog() { }
        public AlertMailLog(int id, string log)
        {
            this.id = id;
            this.log = log;
        }
    }
}
