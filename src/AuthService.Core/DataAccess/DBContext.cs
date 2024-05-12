using Microsoft.EntityFrameworkCore;
using AuthService.Core.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.DataAccess
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
            //this.Database.SetCommandTimeout(300);
        }
        public virtual DbSet<CustomUser> CustomUsers { get; set; }

        public virtual DbSet<AuthClient> AuthClients { get; set; }

        public virtual DbSet<AuthRevokedToken>  AuthRevokedTokens { get; set; }

    }
}
