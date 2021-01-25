using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Model
{
    public class Email
    {
        public int id;
        public int active;
        // connexion
        public string connexion_smtp; 
        public int connexion_port;
        public string connexion_login;
        public string connexion_password;
        // email list
        public int email_client_list_active;
        public string email_client_list;
        public int email_team_list_active;
        public string email_team_list;
        //email error
        public int email_error_active;
        public int email_error_informClient;
        public int email_error_informTeam;
        // email summary
        public int email_summary_active;
        public int email_summary_hours;
        public int email_summary_lastActivated;

        public Email() { }
        public Email(int id, int active, string connexion_smtp, int connexion_port, string connexion_login, string connexion_password,
            int email_client_list_active, string email_client_list, int email_team_list_active, string email_team_list, 
            int email_error_active, int email_error_informClient, int email_error_informTeam,
            int email_summary_active, int email_summary_hours, int email_summary_lastActivated)
        {
            this.id = id;
            this.active = active;
            // connexion
            this.connexion_smtp = connexion_smtp;
            this.connexion_port = connexion_port;
            this.connexion_login = connexion_login;
            this.connexion_password = connexion_password;
            // email list
            this.email_client_list_active = email_client_list_active;
            this.email_client_list = email_client_list;
            this.email_team_list_active = email_team_list_active;
            this.email_team_list = email_team_list;
            // email error
            this.email_error_active = email_error_active;
            this.email_error_informClient = email_error_informClient;
            this.email_error_informTeam = email_error_informTeam;
            // email summary
            this.email_summary_active = email_summary_active;
            this.email_summary_hours = email_summary_hours;
            this.email_summary_lastActivated = email_summary_lastActivated;
        }
    }
}
