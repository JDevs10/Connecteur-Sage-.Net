using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connecteur_Info.Custom
{
    public class Batch_Intro
    {
        private ConnecteurInfo mConnecteurInfo()
        {
            return new Connecteur_Info.ConnecteurInfo();
        }

        public void intro()
        {
            Console.WriteLine("Execution en cours...");
            Console.WriteLine("##############################################################################################################################");
            Console.WriteLine("#################################################### L'import planifier ######################################################");
            Console.WriteLine("##### Version "+ mConnecteurInfo().Version+ " ############################################################################################ " + mConnecteurInfo().Developper + " #####");
            Console.WriteLine("");
        }

        public void intro(StreamWriter writer)
        {
            writer.Flush();
            writer.WriteLine("##############################################################################################################################");
            writer.WriteLine("#################################################### L'import planifier ######################################################");
            writer.WriteLine("##### Version " + mConnecteurInfo().Version + " ############################################################################################ " + mConnecteurInfo().Developper + " #####");
            writer.WriteLine("");
            writer.Flush();
        }

    }
}
