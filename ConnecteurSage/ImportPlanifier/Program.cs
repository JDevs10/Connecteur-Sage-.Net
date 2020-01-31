using importPlanifier.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportPlanifier
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            ConfigurationDNS dns = new ConfigurationDNS();
            dns.Load();
            Console.WriteLine("DNS: "+dns.Prefix + "\nLogin: "+dns.Nom_1 + " PWP: "+dns.Password_1);
            Console.ReadLine();
            */
            try
            {
                Action2 action = new Action2();
                action.LancerPlanification();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
