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
        public PriceType priceType;
        public Reprocess reprocess;

        public ConfigurationGeneral() { }
        public ConfigurationGeneral(General General_, Paths paths_, PriceType priceType_, Reprocess reprocess_)
        {
            this.general = General_;
            this.paths = paths_;
            this.priceType = priceType_;
            this.reprocess = reprocess_;
        }
    }
}
