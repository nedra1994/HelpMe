using HelpMe.Commun.Security.Identity.Data;
using HelpMe.Web.Identity.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Web.Identity.Api.Data
{
    public class IdentityDbContext :  ApplicationDbContext<HelpMeResellerUser, RoleUser>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
       : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
