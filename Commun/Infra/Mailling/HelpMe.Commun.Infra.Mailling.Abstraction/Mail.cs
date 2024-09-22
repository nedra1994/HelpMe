using System.Collections.Generic;
using System.Net.Mail;

namespace HelpMe.Commun.Infra.Mailling.Abstraction
{
    public class Mail
    {
        public string From { get; internal set; }
        public List<string> To { get; }
        public List<string> CC { get; }
        public List<string> CCI { get; }
        public List<string> BCC { get; }
        public List<string> Attachments { get; }
        public string Subject { get; internal set; }
        public string Body { get; internal set; }
        public bool IsBodyHtml { get; internal set; }
        public AlternateView alternateView { get; internal set; }
        public Mail()
        {
            To = new List<string>();
            CC = new List<string>();
            CCI = new List<string>();
            BCC = new List<string>();
            Attachments = new List<string>();
        }
    }
}
