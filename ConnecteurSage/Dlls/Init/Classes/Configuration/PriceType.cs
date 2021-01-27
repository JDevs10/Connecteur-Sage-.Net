using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Init.Classes.Configuration
{
    public class PriceType
    {
        public Boolean activate;
        public Boolean cmdEDIPrice;
        public Boolean productPrice;
        public Boolean categoryPrice;
        public Boolean clientPrice;

        public PriceType() { }
        public PriceType(Boolean activate, Boolean cmdEDIPrice, Boolean productPrice, Boolean categoryPrice, Boolean clientPrice)
        {
            this.activate = activate;
            this.cmdEDIPrice = cmdEDIPrice;
            this.productPrice = productPrice;
            this.categoryPrice = categoryPrice;
            this.clientPrice = clientPrice;
        }
    }
}
