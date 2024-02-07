using Microsoft.EntityFrameworkCore;
using Prospa.AuthService.Core.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.DataAccess
{
    public class ProspaDBContext : DbContext
    {
        public ProspaDBContext(DbContextOptions<ProspaDBContext> options) : base(options)
        {
            //this.Database.SetCommandTimeout(300);
        }
        public virtual DbSet<CustomUser> CustomUsers { get; set; }

        public virtual DbSet<AuthClient> AuthClients { get; set; }

        public virtual DbSet<AuthRevokedToken>  AuthRevokedTokens { get; set; }

    }
}
