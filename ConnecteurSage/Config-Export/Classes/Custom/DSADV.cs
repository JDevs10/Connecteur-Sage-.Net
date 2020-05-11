using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_Export.Classes.Custom
{
    public class DSADV
    {
        public Boolean Activate;
        public string Format;
        public string Status;

        public DSADV() { }
        public DSADV(Boolean Activate_, string Format_, string Status_)
        {
            this.Activate = Activate_;
            this.Format = Format_;
            this.Status = Status_;
        }
    }
}
