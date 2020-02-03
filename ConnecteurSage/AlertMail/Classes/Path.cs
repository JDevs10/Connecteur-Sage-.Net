using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AlertMail.Classes
{
    public class Path
    {
        [XmlElement]
        public string path;

        public Path()
        {

        }

        public Path(string path)
        {
            this.path = path;
        }

        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public void Load()
        {
            if (File.Exists(pathModule + @"\Path.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Path));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\Path.xml");
                Path setting = new Path();
                setting = (Path)reader.Deserialize(file);

                this.path = setting.path;

                file.Close();
            }
        }
    }
}
