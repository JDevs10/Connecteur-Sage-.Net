using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connecteur_Info
{
    public class Batch_Interrup
    {
        public void interruption()
        {
            Console.WriteLine("##########################################################################");
            Console.WriteLine("########################## L'import planifier ############################");
            Console.WriteLine("##### Version 1.12.11 ######################################## Jdevs #####");
            Console.WriteLine("");
            Console.WriteLine("Fermeture du système en raison d'un CTRL-C externe, d'une interruption du processus ou d'un arrêt.");
            Console.WriteLine("");
        }

        public void interruption(StreamWriter writer)
        {
            writer.Flush();
            writer.WriteLine("##########################################################################");
            writer.WriteLine("########################## L'import planifier ############################");
            writer.WriteLine("##### Version 1.12.11 ######################################## Jdevs #####");
            writer.WriteLine("");
            writer.WriteLine("Fermeture du système en raison d'un CTRL-C externe, d'une interruption du processus ou d'un arrêt.");
            writer.WriteLine("");
            writer.Flush();
        }
    }
}
