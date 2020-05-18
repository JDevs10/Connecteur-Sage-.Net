using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Connecteur_Info
{
    public class Batch_Ending
    {
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
            writer.WriteLine("##### Version 1.12.11 ############################################################################################ Jdevs #####");
            writer.WriteLine("##############################################################################################################################");
            writer.WriteLine("");
            writer.Flush();
        }
    }
}
