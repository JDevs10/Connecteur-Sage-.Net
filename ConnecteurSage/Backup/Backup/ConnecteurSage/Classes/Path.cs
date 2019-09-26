using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace ConnecteurSage.Classes
{
    public class Path
    {
        [XmlElement]
        public string path;
        public Boolean exportFactures;
        public Boolean exportBonsLivraisons;
        public Boolean exportBonsCommandes;

        public Path()
        {

        }

        public Path(string path, Boolean exportFactures, Boolean exportBonsLivraisons, Boolean exportBonsCommandes)
        {
            this.path = path;
            this.exportFactures = exportFactures;
            this.exportBonsLivraisons = exportBonsLivraisons;
            this.exportBonsCommandes = exportBonsCommandes;
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
                this.exportFactures = setting.exportFactures;
                this.exportBonsLivraisons = setting.exportBonsLivraisons;
                this.exportBonsCommandes = setting.exportBonsCommandes;

                file.Close();
            }
        }
    }
}
