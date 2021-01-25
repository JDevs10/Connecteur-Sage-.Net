using Connexion.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connexion
{
    public class ConnexionSaveLoad
    {
        public ConfigurationConnexion configurationConnexion { get; set; }
        private string fileName = "SettingConnexions.json";
        public string path = null;
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConnexionSaveLoad() 
        {
            Database.Database db = new Database.Database();
            this.path = @"" + db.settingsManager.get(db.connectionString, 1).EXE_Folder + @"\" + fileName;
        }
        public ConnexionSaveLoad(ConfigurationConnexion mConfigurationConnexion)
        {
            Database.Database db = new Database.Database();
            this.path = @"" + db.settingsManager.get(db.connectionString, 1).EXE_Folder + @"\" + fileName;

            this.configurationConnexion = mConfigurationConnexion;
        }

        public Boolean isSettings()
        {
            if (File.Exists(this.path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Load()
        {
            if (isSettings())
            {
                StreamReader file = new System.IO.StreamReader(this.path);
                ConfigurationConnexion deserializedProduct = JsonConvert.DeserializeObject<ConfigurationConnexion>(file.ReadToEnd());
                deserializedProduct.ODBC.PWD = Utilities.Utils.Decrypt(deserializedProduct.ODBC.PWD);
                deserializedProduct.SQL.PWD = Utilities.Utils.Decrypt(deserializedProduct.SQL.PWD);

                this.configurationConnexion = deserializedProduct;
                file.Close();
            }
        }

        public void saveInfo()
        {
            try
            {
                var myfile = File.Create(this.path);

                this.configurationConnexion.ODBC.PWD = Utilities.Utils.Encrypt(this.configurationConnexion.ODBC.PWD);
                this.configurationConnexion.SQL.PWD = Utilities.Utils.Encrypt(this.configurationConnexion.SQL.PWD);
                string json = JsonConvert.SerializeObject(this.configurationConnexion, Newtonsoft.Json.Formatting.Indented);

                using (StreamWriter writer = new StreamWriter(myfile))
                {
                    writer.Write(json);
                    writer.Flush();
                    writer.Close();
                }
                myfile.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
            }
        }


        public string FormatJson()
        {
            if (isSettings())
            {
                Load();
                var f = Newtonsoft.Json.JsonConvert.SerializeObject(this.configurationConnexion, Newtonsoft.Json.Formatting.Indented);
                return f;
            }
            else
            {
                return "No file \"" + this.path + "\" found!";
            }
        }
    }
}
