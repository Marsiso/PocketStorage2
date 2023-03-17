using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Data;

public sealed class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Move Identity to "dbo" Schema:
        modelBuilder
            .Entity<IdentityUser>()
            .ToTable("Users", "dbo");

        modelBuilder
            .Entity<IdentityRole>()
            .ToTable("Roles", "dbo");

        modelBuilder
            .Entity<IdentityUserToken<string>>()
            .ToTable("UserTokens", "dbo");

        modelBuilder
            .Entity<IdentityUserRole<string>>()
            .ToTable("UserRoles", "dbo");

        modelBuilder
            .Entity<IdentityRoleClaim<string>>()
            .ToTable("RoleClaims", "dbo");

        modelBuilder
            .Entity<IdentityUserClaim<string>>()
            .ToTable("UserClaims", "dbo");

        modelBuilder
            .Entity<IdentityUserLogin<string>>()
            .ToTable("UserLogins", "dbo");
    }
}