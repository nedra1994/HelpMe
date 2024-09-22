using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMe.Commun.Security.Identity.Abstraction
{
    public class HelpMeIdentityToken
    {
        public string SubjectId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public  string token { get; set; }
        public string refreshToken { get; set; }
    }
}
