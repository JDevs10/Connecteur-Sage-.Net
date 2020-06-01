using Config_Import.Classes.Custom_Doc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_Import.Classes
{
    public class ConfigurationImport
    {
        public Doc_Achat Doc_Achat;
        public Doc_Stock Doc_Stock;
        public Doc_Ventes Doc_Ventes;

        public ConfigurationImport() { }
        public ConfigurationImport(Doc_Achat Doc_Achat_, Doc_Stock Doc_Stock_, Doc_Ventes Doc_Ventes_)
        {
            this.Doc_Achat = Doc_Achat_;
            this.Doc_Stock = Doc_Stock_;
            this.Doc_Ventes = Doc_Ventes_;
        }
    }
}
