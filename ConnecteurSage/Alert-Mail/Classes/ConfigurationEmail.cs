using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alert_Mail.Classes.Configuration;

namespace Alert_Mail.Classes
{
    public class ConfigurationEmail
    {
        public Boolean active { get; set; }
        public Connexion connexion { get; set; }
        public EmailLists emailLists { get; set; }
        public EmailImport emailImport { get; set; }
        public EmailExport emailExport { get; set; }
        public EmailError emailError { get; set; }
        public EmailSummary emailSummary { get; set; }
        public EmailPendingFiles emailPendingFiles { get; set; }


        public ConfigurationEmail()
        {
        }

        public ConfigurationEmail(Boolean active_, Connexion connexion_, EmailLists emailLists_, EmailImport emailImport_, EmailExport emailExport_, EmailError emailError_, EmailSummary emailSummary_, EmailPendingFiles emailPendingFiles_)
        {
            this.active = active_;
            this.connexion = connexion_;
            this.emailLists = emailLists_;
            this.emailImport = emailImport_;
            this.emailExport = emailExport_;
            this.emailError = emailError_;
            this.emailSummary = emailSummary_;
            this.emailPendingFiles = emailPendingFiles_;
        }

        public Boolean isValid_Email(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
