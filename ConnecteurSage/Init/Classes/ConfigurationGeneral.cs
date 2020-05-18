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
        public Reprocess reprocess;

        public ConfigurationGeneral() { }
        public ConfigurationGeneral(General General_, Paths paths_, Reprocess reprocess_)
        {
            this.general = General_;
            this.paths = paths_;
            this.reprocess = reprocess_;
        }
    }
}
