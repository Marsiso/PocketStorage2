using Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    #region Public Methods

    public static void Configure(this DbContextOptionsBuilder builder, WebApplicationBuilder applicationBuilder, string? connectionString, string? migrationAssembly = nameof(IdentityServer))
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            string errorMessage = $"Database connection string cannot be a null reference object or an empty string. Parameter name: {nameof(connectionString)} Value: {connectionString}";
            throw new ArgumentNullException(nameof(connectionString), errorMessage);
        }

        if (string.IsNullOrEmpty(migrationAssembly))
        {
            string errorMessage = $"Migrations assembly cannot be a null reference object or an empty string. Parameter name: {nameof(migrationAssembly)} Value: {migrationAssembly}";
            throw new ArgumentNullException(nameof(migrationAssembly), errorMessage);
        }

        builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationAssembly));
        builder.UseOpenIddict();
        builder.EnableSensitiveDataLogging(applicationBuilder.Environment.IsDevelopment());
        builder.EnableServiceProviderCaching();
        builder.UseLoggerFactory(ApplicationDatabaseContext.PropertyAppLoggerFactory);
    }

    #endregion Public Methods
}