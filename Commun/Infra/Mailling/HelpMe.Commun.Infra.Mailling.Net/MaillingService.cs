using HelpMe.Commun.Infra.Mailling.Abstraction;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HelpMe.Commun.Infra.Mailling.Net
{
    public class MaillingService : IMaillingService
    {
        MaillingConfig _config;
        public MaillingService(IOptions<MaillingConfig> config)
        {
            _config = config.Value;
        }
        private void Send(string from, List<string> to, List<string> cc, List<string> cci, List<string> pj, string subject, string body, bool IsBodyHtml, AlternateView alternateView)
        {
            try
            {

                if (to == null) to = new List<string>();
                if (cc == null) cc = new List<string>();
                if (cci == null) cci = new List<string>();
                if (pj == null) pj = new List<string>();

                if (string.IsNullOrEmpty(from)) from = _config.EmailAdress;
                from = "noreply@extranet-it.fr";
                System.Net.Mail.MailMessage eMail = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient Client = new System.Net.Mail.SmtpClient();
                Client.Host = _config.SmtpHost;
                Client.Port = _config.SmtpServerPort;
                if (_config.IsAutentification == 1)
                {
                  //  Client.UseDefaultCredentials = true;
                    Client.Credentials = new System.Net.NetworkCredential(_config.EmailLogin, _config.EmailPassword);
                }

                if (_config.EnableSsl == 1)
                    Client.EnableSsl = true;

                System.Net.Mail.MailMessage objMail = new System.Net.Mail.MailMessage();

                objMail.From = new System.Net.Mail.MailAddress(from);
                objMail.Subject = subject;
                objMail.Body = body;
                foreach (var _to in to) objMail.To.Add(_to);
                foreach (var _cci in cci) objMail.Bcc.Add(_cci);
                foreach (var _cc in cc) objMail.CC.Add(_cc);
                foreach (var _pj in pj) objMail.Attachments.Add(new System.Net.Mail.Attachment(_pj));

                objMail.IsBodyHtml = IsBodyHtml;
                if (alternateView != null)
                    objMail.AlternateViews.Add(alternateView);
                Client.Send(objMail);
            }

            catch (System.Exception ex)
            {

                throw;
            }
        }
        public  void Send(Mail mail)
        {
            Send(mail.From, mail.To, mail.CC, mail.CCI, mail.Attachments, mail.Subject, mail.Body, mail.IsBodyHtml, mail.alternateView);
        }
        private void SendFTM(string from, List<string> to, List<string> cc, List<string> pj, string subject, string body, bool IsBodyHtml, AlternateView alternateView)
        {
            try
            {

                if (to == null) to = new List<string>();
                if (cc == null) to = new List<string>();
                if (pj == null) pj = new List<string>();

                if (string.IsNullOrEmpty(from)) from = _config.EmailAdress;
                System.Net.Mail.MailMessage eMail = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient Client = new System.Net.Mail.SmtpClient();
                Client.Host = "10.10.20.55";
                Client.Port = 25;
                //if (_config.IsAutentification == 1)
                //{
                //    //  Client.UseDefaultCredentials = true;
                //    Client.Credentials = new System.Net.NetworkCredential(_config.EmailLogin, _config.EmailPassword);
                //}

                if (_config.EnableSsl == 1)
                    Client.EnableSsl = true;

                System.Net.Mail.MailMessage objMail = new System.Net.Mail.MailMessage();

                objMail.From = new System.Net.Mail.MailAddress(from);
                objMail.Subject = subject;
                objMail.Body = body;
                foreach (var _to in to) objMail.To.Add(_to);
                foreach (var _cc in cc) objMail.CC.Add(_cc);
                foreach (var _pj in pj) objMail.Attachments.Add(new System.Net.Mail.Attachment(_pj));

                objMail.IsBodyHtml = IsBodyHtml;
                if (alternateView != null)
                    objMail.AlternateViews.Add(alternateView);
                Client.Send(objMail);
            }

            catch (System.Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        public void SendFTM(Mail mail)
        {
            SendFTM(mail.From, mail.To, mail.CC, mail.Attachments, mail.Subject, mail.Body, mail.IsBodyHtml, mail.alternateView);
        }

    }

   
    //public class MaillingService : IMaillingService
    //{
    //    public void Send(string from, List<string> to, string subject, string body)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
