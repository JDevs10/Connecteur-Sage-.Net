using Init.Classes.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Init.Classes
{
    public class ConfigurationGeneral
    {
        public General general;
        public Paths paths;
        public PlannerTask plannerTask;
        public PriceType priceType;
        public Reprocess reprocess;

        public ConfigurationGeneral() { }
        public ConfigurationGeneral(General General_, Paths paths_, PlannerTask plannerTask_, PriceType priceType_, Reprocess reprocess_)
        {
            this.general = General_;
            this.paths = paths_;
            this.plannerTask = plannerTask_;
            this.priceType = priceType_;
            this.reprocess = reprocess_;
        }
    }
}
