using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace HelpMe.Commun.Infra.Mailling.Abstraction
{
    public class MailBuilder
    {
        private Mail CurrentMail = new Mail();

        Dictionary<string, string> ReplaceValeur=new Dictionary<string, string>();
        public MailBuilder To(string to)
        {
            if (!IsValidEmail(to)) throw new Exception($"To email {to} format invalide");
            if (!CurrentMail.To.Exists(p => p.ToUpper() == to.ToUpper()))
                CurrentMail.To.Add(to);
            return this;
        }
        public MailBuilder From(string from)
        {
            if (!IsValidEmail(from)) throw new Exception($"From email {from} format invalide");
            CurrentMail.From = from;
            return this;
        }
       

        public MailBuilder Cc(string cc)
        {
            if (!IsValidEmail(cc)) throw new Exception($"cc email {cc} format invalide");
            if (!CurrentMail.CC.Exists(p=>p.ToUpper()== cc.ToUpper())) CurrentMail.CC.Add(cc);

            return this;
        }

        public MailBuilder CCI(string cci)
        {
            if (!IsValidEmail(cci)) throw new Exception($"cci email {cci} format invalide");
            if (!CurrentMail.CCI.Exists(p => p.ToUpper() == cci.ToUpper())) CurrentMail.CCI.Add(cci);
            return this;
        }
        public MailBuilder Bcc(string bcc)
        {
            if (!IsValidEmail(bcc)) throw new Exception($"bcc email {bcc} format invalide");
            if (!CurrentMail.BCC.Exists(p => p.ToUpper() == bcc.ToUpper())) CurrentMail.BCC.Add(bcc);
            return this;
        }



        public MailBuilder Subject(string subject)
        {
            CurrentMail.Subject = subject;
            return this;
        }
        public MailBuilder Pj(string fileName)
        {
            if (!File.Exists(fileName)) throw new Exception($"Pj {fileName} introuvable ");
            CurrentMail.Attachments.Add(fileName);


            return this;
        }
        public MailBuilder BodyFromTemplateFile(string fileName)
        {
            if (!File.Exists(fileName)) throw new Exception($"Fichier {fileName} introuvable ");
            var body = File.ReadAllText(fileName);
            CurrentMail.Body = body;
            return this;
        }
        public MailBuilder Body(string body)
        {
            CurrentMail.Body = body;
            return this;
        }

        public MailBuilder AlternateViewInner(string mailBody, string signaturePath)
        {

            if (!string.IsNullOrEmpty(signaturePath))
            {
                var signature = "<img alt=\"\" hspace=0 src=\"cid:uniqueId\" align=baseline border=0 >";
                mailBody = "<html><body> " + mailBody + " </br> " + signature + " </br> </body></html>";
                
                var htmlView = AlternateView.CreateAlternateViewFromString(mailBody, null, "text/html");

                var imageResource = new LinkedResource(signaturePath);
                imageResource.ContentId = "uniqueId";
                htmlView.LinkedResources.Add(imageResource);
                CurrentMail.alternateView = htmlView;
            }
            else
            {
                var htmlView = AlternateView.CreateAlternateViewFromString(mailBody, null, "text/html");
                CurrentMail.alternateView = htmlView;
            }
            
            return this;

        }
        public MailBuilder WithVariable(string varName, string val)
        {
            if (ReplaceValeur.ContainsKey(varName)) ReplaceValeur[varName] = val;
            else ReplaceValeur.Add(varName, val);
            return this;
        }
        private void ProcessReplaceVariable()
        {
            foreach (var key in ReplaceValeur.Keys)
            {
                CurrentMail.Body = CurrentMail.Body.Replace(key, ReplaceValeur[key]);
            }
        }

        public void AllowBodyHtml()
        {
            CurrentMail.IsBodyHtml = true;
        }
        public void DisableBodyHtml()
        {
            CurrentMail.IsBodyHtml = false;
        }
        public Mail Build()
        {
            ProcessReplaceVariable();
            return CurrentMail;
        }

        bool IsValidEmail(string email)
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

   
    //public class MaillingService : IMaillingService
    //{
    //    public void Send(string from, List<string> to, string subject, string body)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
