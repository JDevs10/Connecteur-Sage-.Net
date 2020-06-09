using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Connecteur_Info.Custom
{
    public class Batch_Ending
    {
        private ConnecteurInfo mConnecteurInfo()
        {
            return new Connecteur_Info.ConnecteurInfo();
        }

        public void ending()
        {
            Console.WriteLine("##############################################################################################################################");
            Console.WriteLine("#################################################### L'import planifier ######################################################");
            Console.WriteLine("##############################################################################################################################");
            Console.WriteLine("");
            Thread.Sleep(2000);
        }

        public void ending(StreamWriter writer)
        {
            writer.Flush();
            writer.WriteLine("##############################################################################################################################");
            writer.WriteLine("#################################################### L'import planifier ######################################################");
            writer.WriteLine("##### Version " + mConnecteurInfo().Version + " ############################################################################################ " + mConnecteurInfo().Developper + " #####");
            writer.WriteLine("##############################################################################################################################");
            writer.WriteLine("");
            writer.Flush();
        }
    }
}
