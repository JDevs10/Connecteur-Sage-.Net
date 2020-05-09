﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AlertMail.Classes
{
    public class CustomMailRecap
    {
        [XmlElement]
        public string MailType { set; get; }
        [XmlElement]
        public string Client { set; get; }
        [XmlElement]
        public string Subject { set; get; }
        [XmlElement]
        public string DateTimeCreated { set; get; }
        [XmlElement]
        public string DateTimeModified { set; get; }
        [XmlElement]
        public List<CustomMailRecapLines> Lines { get; set; }
        [XmlElement]
        public List<string> Attachments { get; set; }

        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public CustomMailRecap()
        {
        }

        public CustomMailRecap(string MailType, string Client, string Subject, string DateTimeCreated, string DateTimeModified, List<CustomMailRecapLines> Lines, List<string> Attachments)
        {
            this.MailType = MailType;
            this.Client = Client;
            this.Subject = Subject;
            this.DateTimeCreated = DateTimeCreated;
            this.DateTimeModified = DateTimeModified;
            this.Lines = Lines;
            this.Attachments = Attachments;
        }

        public void saveInfo(CustomMailRecap mCustomMailRecap, string fileName)
        {
            try
            {
                var myfile = File.Create(pathModule + @"\" + fileName);
                XmlSerializer xml = new XmlSerializer(typeof(CustomMailRecap));
                xml.Serialize(myfile, mCustomMailRecap);
                myfile.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
            }
        }
        public void Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(CustomMailRecap));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\" + fileName);
                CustomMailRecap mCustomMailRecap = new CustomMailRecap();
                mCustomMailRecap = (CustomMailRecap)reader.Deserialize(file);

                this.MailType = mCustomMailRecap.MailType;
                this.Client = mCustomMailRecap.Client;
                this.Subject = mCustomMailRecap.Subject;
                this.DateTimeCreated = mCustomMailRecap.DateTimeCreated;
                this.DateTimeModified = mCustomMailRecap.DateTimeModified;
                this.Lines = mCustomMailRecap.Lines;
                this.Attachments = mCustomMailRecap.Attachments;
                file.Close();
            }
        }
    }
}
