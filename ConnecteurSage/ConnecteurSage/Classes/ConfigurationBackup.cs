using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ConnecteurSage.Classes
{
    public class ConfigurationBackup
    {
        [XmlElement]
        public bool activate { get; set; }
        [XmlElement]
        public int general_Log { get; set; }
        [XmlElement]
        public int import_Log { get; set; }
        [XmlElement]
        public int export_Log { get; set; }
        [XmlElement]
        public int import_files_success { get; set; }
        [XmlElement]
        public int import_files_error { get; set; }
        [XmlElement]
        public int backup_files { get; set; }
        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConfigurationBackup()
        {
        }

        public ConfigurationBackup(bool activate, int general_Log, int import_Log, int export_Log, int import_files_success, int import_files_error, int backup_files)
        {
            this.activate = activate;
            this.general_Log = general_Log;
            this.import_Log = import_Log;
            this.export_Log = export_Log;
            this.import_files_success = import_files_success;
            this.import_files_error = import_files_error;
            this.backup_files = backup_files;
        }

        public void saveInfo(ConfigurationBackup backupSettings)
        {
            try
            {
                var myfile = File.Create(pathModule + @"\SettingBackup.xml");
                XmlSerializer xml = new XmlSerializer(typeof(ConfigurationBackup));
                xml.Serialize(myfile, backupSettings);
                myfile.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
            }
        }
        public void Load()
        {
            if (File.Exists("SettingBackup.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationBackup));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingBackup.xml");
                ConfigurationBackup config = new ConfigurationBackup();
                config = (ConfigurationBackup)reader.Deserialize(file);

                this.activate = config.activate;
                this.general_Log = config.general_Log;
                this.import_Log = config.import_Log;
                this.export_Log = config.export_Log;
                this.export_Log = config.export_Log;
                this.import_files_success = config.import_files_success;
                this.import_files_error = config.import_files_error;
                this.backup_files = config.backup_files;
                file.Close();
            }
        }
    }
}
