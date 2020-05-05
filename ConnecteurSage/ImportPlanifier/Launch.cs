using importPlanifier.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImportPlanifier
{
    class Launch
    {
        public void go()
        {
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
