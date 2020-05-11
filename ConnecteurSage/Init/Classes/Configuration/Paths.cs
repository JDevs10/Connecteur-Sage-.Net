using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Init.Classes.Configuration
{
    public class Paths
    {
        public string EDI_Folder;

        public Paths() { }
        public Paths(string EDI_Folder)
        {
            this.EDI_Folder = EDI_Folder;
        }
    }
}
