using ConnecteurAuto.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConnecteurAuto
{
    class Launch
    {
        public void go()
        {
            try
            {
                
                Classes.Action action = new Classes.Action();
                action.LancerPlanification();
                
                /*
                Hey hey = new Hey();
                hey.hello();
                */

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
