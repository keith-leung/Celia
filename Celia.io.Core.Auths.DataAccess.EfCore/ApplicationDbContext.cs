using System;
using Microsoft.EntityFrameworkCore;
using Celia.io.Core.Auths.Abstractions;

namespace Celia.io.Core.Auths.DataAccess.EfCore
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<MigrationVersion> MigrationVersions { get; set; }

        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<ApplicationRole> Roles { get; set; }

        public DbSet<ApplicationRoleClaim> RoleClaims { get; set; }

        public DbSet<ApplicationUserClaim> UserClaims { get; set; }

        public DbSet<ApplicationUserLogin> UserLogins { get; set; }

        public DbSet<ApplicationUserRole> UserRoles { get; set; }

        public DbSet<ApplicationUserToken> UserTokens { get; set; }

        public DbSet<ServiceApp> ServiceApps { get; set; }
    }
}