using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connecteur_Info
{
    public class Batch_Intro
    {
        public void intro()
        {
            Console.WriteLine("Execution en cours...");
            Console.WriteLine("##############################################################################################################################");
            Console.WriteLine("#################################################### L'import planifier ######################################################");
            Console.WriteLine("##### Version 1.12.11 ############################################################################################ Jdevs #####");
            Console.WriteLine("");
        }

        public void intro(StreamWriter writer)
        {
            writer.Flush();
            writer.WriteLine("##############################################################################################################################");
            writer.WriteLine("#################################################### L'import planifier ######################################################");
            writer.WriteLine("##### Version 1.12.11 ############################################################################################ Jdevs #####");
            writer.WriteLine("");
            writer.Flush();
        }

    }
}
