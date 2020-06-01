using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reprocess_Error_Files.Classes.Configuration
{
    public class ReprocessFiles
    {
        public int fileReprocessCount;
        public int ediFileId;
        public string fileName;
        public string filePath;

        public ReprocessFiles() { }
        public ReprocessFiles(int fileReprocessCount, int ediFileId, string fileName, string filePath)
        {
            this.fileReprocessCount = fileReprocessCount;
            this.ediFileId = ediFileId;
            this.fileName = fileName;
            this.filePath = filePath;
        }
    }
}
