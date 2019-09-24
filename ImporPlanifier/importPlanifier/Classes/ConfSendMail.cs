using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace importPlanifier.Classes
{
    public class ConfSendMail
    {
        public string smtp;
        public int port;
        public string login;
        public string password;
        public string dest1;
        public string dest2;
        public string dest3;
        public Boolean active;

        public ConfSendMail(string smtp, int port, string login, string password, string dest1, string dest2, string dest3)
        {
            this.smtp = smtp;
            this.port = port;
            this.login = login;
            this.password = password;
            this.dest1 = dest1;
            this.dest2 = dest2;
            this.dest3 = dest3;
        }

        public ConfSendMail()
        {

        }

    }
}
