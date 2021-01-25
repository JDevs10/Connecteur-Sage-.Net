using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace importPlanifier.Classes
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
        public int export_files_BC { get; set; }
        [XmlElement]
        public string export_files_BC_type { get; set; }
        [XmlElement]
        public int export_files_BL { get; set; }
        [XmlElement]
        public string export_files_BL_type { get; set; }
        [XmlElement]
        public int export_files_FA { get; set; }
        [XmlElement]
        public string export_files_FA_type { get; set; }
        [XmlElement]
        public int export_files_ME_MS { get; set; }
        [XmlElement]
        public string export_files_ME_MS_type { get; set; }



        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConfigurationBackup()
        {
        }

        public ConfigurationBackup(bool activate, int general_Log, int import_Log, int export_Log, int import_files_success, int import_files_error, int export_files_BC, string export_files_BC_type, int export_files_BL, string export_files_BL_type, int export_files_FA, string export_files_FA_type, int export_files_ME_MS, string export_files_ME_MS_type)
        {
            this.activate = activate;
            this.general_Log = general_Log;
            this.import_Log = import_Log;
            this.export_Log = export_Log;
            this.export_Log = export_Log;
            this.import_files_success = import_files_success;
            this.import_files_error = import_files_error;
            this.export_files_BC = export_files_BC;
            this.export_files_BC_type = export_files_BC_type;
            this.export_files_BL = export_files_BL;
            this.export_files_BL_type = export_files_BL_type;
            this.export_files_FA = export_files_FA;
            this.export_files_FA_type = export_files_FA_type;
            this.export_files_ME_MS = export_files_ME_MS;
            this.export_files_ME_MS_type = export_files_ME_MS_type;
        }

        public void saveInfo(ConfigurationBackup backupSettings)
        {
            try
            {
                //var myfile = File.Create(pathModule + @"\SettingBackup.xml");
                FileStream fs = new FileStream(pathModule + @"\SettingBackup.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                XmlSerializer xml = new XmlSerializer(typeof(ConfigurationBackup));
                xml.Serialize(fs, backupSettings);
                fs.Close();
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
                FileStream fs = new FileStream(pathModule + @"\SettingBackup.xml", FileMode.Open, FileAccess.ReadWrite);
                StreamReader file = new System.IO.StreamReader(fs);
                ConfigurationBackup mail = new ConfigurationBackup();
                mail = (ConfigurationBackup)reader.Deserialize(file);

                this.activate = mail.activate;
                this.general_Log = mail.general_Log;
                this.import_Log = mail.import_Log;
                this.export_Log = mail.export_Log;
                this.export_Log = mail.export_Log;
                this.import_files_success = mail.import_files_success;
                this.import_files_error = mail.import_files_error;
                this.export_files_BC = mail.export_files_BC;
                this.export_files_BC_type = mail.export_files_BC_type;
                this.export_files_BL = mail.export_files_BL;
                this.export_files_BL_type = mail.export_files_BL_type;
                this.export_files_FA = mail.export_files_FA;
                this.export_files_FA_type = mail.export_files_FA_type;
                this.export_files_ME_MS = mail.export_files_ME_MS;
                this.export_files_ME_MS_type = mail.export_files_ME_MS_type;
                file.Close();
            }
        }
    }
}
