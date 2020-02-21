using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extract
{
    class Program
    {
        static void Main(string[] args)
        {
            string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string zipFile = localPath + @"\Z-Cron.zip";
            string extractPath = localPath + @"\extract";

            //System.IO.Compression.ZipFile.CreateFromDirectory(localPath, zipFile);
            System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, extractPath);
        }
    }
}
