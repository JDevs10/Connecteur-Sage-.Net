using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Model
{
    public class Reprocess
    {
        public int ediFileID;
        public string fileName;
        public string filePath;
        public int fileReprocessCount;

        public Reprocess() { }
        public Reprocess(int ediFileID, string fileName, string filePath, int fileReprocessCount)
        {
            this.ediFileID = ediFileID;
            this.fileName = fileName;
            this.filePath = filePath;
            this.fileReprocessCount = fileReprocessCount;
        }
    }
}
