using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_Export.Classes.Custom
{
    public class Stock
    {
        public Boolean Activate;
        public string Format;
        public string Status;

        public Stock() { }
        public Stock(Boolean Activate_, string Format_, string Status_)
        {
            this.Activate = Activate_;
            this.Format = Format_;
            this.Status = Status_;
        }
    }
}
