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
        public Database.Database db { get; set; }
        public ConfigurationConnexion configurationConnexion { get; set; }
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConnexionSaveLoad() 
        {
            this.db = new Database.Database();
        }

        public Boolean isSettings()
        {
            List< Database.Model.Connexion> list = this.db.connexionManager.getList(this.db.connectionString);
            if (list != null && list.Count == 2)
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
                Database.Model.Connexion connexion = this.db.connexionManager.getByType(this.db.connectionString, this.db.connexionManager.ODBC);

                this.configurationConnexion = new ConfigurationConnexion();
                this.configurationConnexion.ODBC.ID = connexion.id;
                this.configurationConnexion.ODBC.DNS = connexion.dns;
                this.configurationConnexion.ODBC.USER = connexion.name;
                this.configurationConnexion.ODBC.PWD = connexion.password;

                connexion = this.db.connexionManager.getByType(this.db.connectionString, this.db.connexionManager.SQL);
                this.configurationConnexion.SQL.ID = connexion.id;
                this.configurationConnexion.SQL.DNS = connexion.dns;
                this.configurationConnexion.SQL.USER = connexion.name;
                this.configurationConnexion.SQL.PWD = connexion.password;
            }
        }

        public void saveInfo()
        {
            try
            {
                this.configurationConnexion.ODBC.PWD = Utilities.Utils.Encrypt(this.configurationConnexion.ODBC.PWD);
                this.configurationConnexion.SQL.PWD = Utilities.Utils.Encrypt(this.configurationConnexion.SQL.PWD);

                Database.Model.Connexion connexion_1 = new Database.Model.Connexion();
                connexion_1.id = this.configurationConnexion.ODBC.ID;
                connexion_1.type = this.db.connexionManager.ODBC;
                connexion_1.dns = this.configurationConnexion.ODBC.DNS;
                connexion_1.name = this.configurationConnexion.ODBC.USER;
                connexion_1.password = this.configurationConnexion.ODBC.PWD;

                Database.Model.Connexion connexion_db = this.db.connexionManager.getById(this.db.connectionString, connexion_1.id);
                if(connexion_db != null)
                {
                    //update
                    this.db.connexionManager.update(this.db.connectionString, connexion_1);
                }
                else
                {
                    //insert
                    this.db.connexionManager.insert(this.db.connectionString, connexion_1);
                }

                Database.Model.Connexion connexion_2 = new Database.Model.Connexion();
                connexion_2.id = this.configurationConnexion.SQL.ID;
                connexion_2.type = this.db.connexionManager.SQL;
                connexion_2.dns = this.configurationConnexion.SQL.DNS;
                connexion_2.name = this.configurationConnexion.SQL.USER;
                connexion_2.password = this.configurationConnexion.SQL.PWD;

                connexion_db = this.db.connexionManager.getById(this.db.connectionString, connexion_2.id);
                if (connexion_db != null)
                {
                    //update
                    this.db.connexionManager.update(this.db.connectionString, connexion_2);
                }
                else
                {
                    //insert
                    this.db.connexionManager.insert(this.db.connectionString, connexion_2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
            }
        }

        /*
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
                return "No file \"" + fileName + "\" found!";
            }
        }
        */
    }
}
