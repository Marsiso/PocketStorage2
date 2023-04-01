using Domain.Data;
using Domain.Data.Entities;
using Microsoft.AspNetCore.Identity;
using static Domain.Constants.ApplicationConstants;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Extensions;

public static class IdentityExtensions
{
    #region Public Methods

    public static IdentityBuilder AddApplicationIdentity(this IServiceCollection services)
    {
        return services
            .AddIdentity<ApplicationUserEntity, ApplicationRoleEntity>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();
    }

    public static IdentityOptions ConfigureApplicationIdentity(this IdentityOptions options)
    {
        // Claims
        options.ClaimsIdentity.UserNameClaimType = Claims.Name;
        options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
        options.ClaimsIdentity.RoleClaimType = Claims.Role;
        options.ClaimsIdentity.EmailClaimType = Claims.Email;
        options.SignIn.RequireConfirmedAccount = false;

        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = Account.Password.MinimalLength;
        options.Password.RequiredUniqueChars = Account.Password.MinimalUniqueCharacters;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = Account.Lockout.MaxFailedAccessAttempts;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.AllowedUserNameCharacters = Account.Username.AllowedCharacters;
        options.User.RequireUniqueEmail = true;

        return options;
    }

    #endregion Public Methods
}