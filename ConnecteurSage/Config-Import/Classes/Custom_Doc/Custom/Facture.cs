﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_Import.Classes.Custom_Doc.Custom
{
    public class Facture
    {
        public string Activate;
        public string Format;
        public string Status;

        public Facture() { }
        public Facture(string Activate_, string Format_, string Status_)
        {
            this.Activate = Activate_;
            this.Format = Format_;
            this.Status = Status_;
        }
    }
}
