using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail.Classes.Custom
{
    public class CustomMailSuccessLines
    {
        public string ProductRef { set; get; }
        public string ProductName { set; get; }
        public string ProductPriceTTC { set; get; }
        public string ProductQte { set; get; }
        

        public CustomMailSuccessLines() { }
        public CustomMailSuccessLines(string ProductRef, string ProductName, string ProductPriceTTC, string ProductQte)
        {
            this.ProductRef = ProductRef;
            this.ProductName = ProductName;
            this.ProductPriceTTC = ProductPriceTTC;
            this.ProductQte = ProductQte;
            
        }
    }
}
