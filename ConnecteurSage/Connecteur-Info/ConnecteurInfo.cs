using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Reflection;
using System.Deployment.Application;

namespace Connecteur_Info
{
    public class ConnecteurInfo
    {
        public string Name = "EDI Connect - TableWear";
        public string Version = "1.20.3";
        public string Developper = "@Développeur JL";
        public string Author = "Big Data Consulting";
        public string Client = "TableWear";
        /*
         * Will get current folder everytime when Connecteur Sage 'Manuel' opens general config
         * This action is needed for the first time when the Connecteur is installed
         * Or when the init.ini file is missing
         */
        public string installation_dir = "";

        public ConnecteurInfo(){}
    }
}
