using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Init.Classes.Configuration
{
    public class PlannerTask
    {
        public string Name;
        public string UserId;
        public Boolean Enabled;

        public PlannerTask() { }
        public PlannerTask(string Name, string UserId, Boolean Enabled)
        {
            this.Name = Name;
            this.UserId = UserId;
            this.Enabled = Enabled;
        }
    }
}
