using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_Import.Classes.Custom_Doc.Custom
{
    public class Stock
    {
        public string Activate;
        public string Format;
        public string Status;

        public Stock() { }
        public Stock(string Activate_, string Format_, string Status_)
        {
            this.Activate = Activate_;
            this.Format = Format_;
            this.Status = Status_;
        }
    }
}
