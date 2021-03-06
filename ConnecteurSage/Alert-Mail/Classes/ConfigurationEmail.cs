﻿using System;
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
        public Alert_Mail.Classes.Configuration.Connexion connexion { get; set; }
        public EmailLists emailLists { get; set; }
        public EmailError emailError { get; set; }
        public EmailSummary emailSummary { get; set; }
        public EmailPendingFiles emailPendingFiles { get; set; }


        public ConfigurationEmail()
        {
        }

        public ConfigurationEmail(Boolean active_, Alert_Mail.Classes.Configuration.Connexion connexion_, EmailLists emailLists_, EmailError emailError_, EmailSummary emailSummary_, EmailPendingFiles emailPendingFiles_)
        {
            this.active = active_;
            this.connexion = connexion_;
            this.emailLists = emailLists_;
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
