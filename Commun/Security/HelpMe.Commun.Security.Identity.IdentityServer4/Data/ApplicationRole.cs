using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using HelpMe.Commun.Security.Identity.Abstraction;

namespace HelpMe.Commun.Security.Identity.Data
{
    public abstract class ApplicationRole : IdentityRole, IRole
    {
    }
}
