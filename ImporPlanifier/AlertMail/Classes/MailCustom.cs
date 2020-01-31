using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertMail.Classes
{
    class MailCustom
    {
        public string subject { set; get; }
        public string body { set; get; }
        public List<string> attachements { set; get; }

        public MailCustom(string subject, string body, List<string> attachements)
        {
            this.subject = subject;
            this.body = body;
            this.attachements = attachements;
        }
    }
}
