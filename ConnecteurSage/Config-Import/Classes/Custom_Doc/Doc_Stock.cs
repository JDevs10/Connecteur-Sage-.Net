using Config_Import.Classes.Custom_Doc.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_Import.Classes.Custom_Doc
{
    public class Doc_Stock
    {
        public Stock Stock;

        public Doc_Stock() { }
        public Doc_Stock(Stock Stock_)
        {
            this.Stock = Stock_;
        }
    }
}
