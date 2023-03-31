using Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    #region Public Methods

    public static void Configure(this DbContextOptionsBuilder builder, WebApplicationBuilder applicationBuilder, string connectionString, string migrationAssembly)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), $"Database connection string cannot be a null or empty string. Parameter name: {nameof(connectionString)} Value: {connectionString}");
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), $"Migrations assembly, cannot be a null or empty string. Parameter name: {nameof(migrationAssembly)} Value: {migrationAssembly}");
        }

        builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationAssembly));
        builder.UseOpenIddict();
        builder.EnableSensitiveDataLogging(applicationBuilder.Environment.IsDevelopment());
        builder.EnableServiceProviderCaching();
        builder.UseLoggerFactory(ApplicationDatabaseContext.PropertyAppLoggerFactory);
    }

    #endregion Public Methods
}