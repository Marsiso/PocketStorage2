using Domain.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Domain.Data;

/// <summary>
/// Application database context that implements ASP.NET Core Identity with custom application
/// logic. For further details about customization of ASP.NET Core Identity see <see href="https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-7.0"/>.
/// </summary>
public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUserEntity, ApplicationRoleEntity, Guid, ApplicationUserClaimEntity, ApplicationUserRoleEntity, ApplicationUserLoginEntity, ApplicationRoleClaimEntity, ApplicationUserTokenEntity>
{
    #region Protected Methods

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseIdentityColumns();
    }

    #endregion Protected Methods

    #region Public Fields

    public static readonly ILoggerFactory PropertyAppLoggerFactory =
            LoggerFactory.Create(builder =>
                builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && (level == LogLevel.Warning))
            .AddConsole());

    #endregion Public Fields

    #region Public Constructors

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
    {
    }

    #endregion Public Constructors
}