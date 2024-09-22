
using System;

namespace HelpMe.Commun.Security.Identity.Abstraction
{
    public class Erreur
    {
        public readonly String Code;
        public readonly String Description;

        public Erreur(String code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
//#­307
