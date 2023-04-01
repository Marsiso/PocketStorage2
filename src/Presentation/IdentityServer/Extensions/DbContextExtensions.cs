using Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Extensions;

public static class DbContextExtensions
{
    #region Public Methods

    public static IServiceCollection AddAplicationDatabaseContext(this IServiceCollection services, WebApplicationBuilder applicationBuilder)
    {
        return services
            .AddDbContext<ApplicationDbContext>(builder =>
            {
                string connectionString = applicationBuilder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException($"[{nameof(DbContextExtensions)}] Null reference exception. Parameter: 'ConnectionString' Value: 'DefaultConnection'.");

                builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(nameof(IdentityServer)));
                builder.UseOpenIddict();
                builder.EnableSensitiveDataLogging(applicationBuilder.Environment.IsDevelopment());
                builder.EnableServiceProviderCaching();
                builder.UseLoggerFactory(ApplicationDbContext.PropertyAppLoggerFactory);
            });
    }

    #endregion Public Methods
}