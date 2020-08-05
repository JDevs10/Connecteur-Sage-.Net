﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connexion.Classes.Custom
{
    public class SQL
    {
        public int ID;
        public string PREFIX;
        public string DNS;
        public string USER;
        public string PWD;

        public SQL() { }
        public SQL(int ID, string PREFIX_, string DNS_, string USER_, string PWD_)
        {
            this.ID = ID;
            this.PREFIX = PREFIX_;
            this.DNS = DNS_;
            this.USER = USER_;
            this.PWD = PWD_;
        }
        public SQL(string PREFIX_, string DNS_, string USER_, string PWD_)
        {
            this.PREFIX = PREFIX_;
            this.DNS = DNS_;
            this.USER = USER_;
            this.PWD = PWD_;
        }
    }
}
