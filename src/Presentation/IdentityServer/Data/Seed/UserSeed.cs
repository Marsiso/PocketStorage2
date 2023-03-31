using Domain.Constants;
using Domain.Data;
using Domain.Identity.Entities;
using LanguageExt.Common;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace IdentityServer.Data.Seed;

public sealed class UserSeed : IHostedService
{
    #region Private Fields

    private readonly ILogger<UserSeed> _logger;
    private readonly IServiceProvider _serviceProvider;

    #endregion Private Fields

    #region Private Methods

    private async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        Func<Exception, ApplicationRole> LogUserRoleCreationExceptionFunc = (exception) =>
        {
            _logger.LogError(exception.Message);
            return default!;
        };

        #region System Administrator

        var systemAdminRole = new ApplicationRole
        {
            Name = ApplicationConstants.Roles.SystemAdministrator,
            Description = "System administrator role with permission to create, edit, delete and view resources.",
            IsActive = true
        };

        Result<ApplicationRole> result = await systemAdminRole.TryCreateAsync(roleManager);
        ApplicationRole systemAdminRoleEntity = result.Match(applicationRole =>
        {
            string message = $"System administrator role with ID {applicationRole.Id} has been successfully created.";
            _logger.LogInformation(message);
            return applicationRole;
        }, LogUserRoleCreationExceptionFunc);

        result = await systemAdminRoleEntity.TryAddPermissionsAsync(roleManager,
            new Collection<string>()
            {
                ApplicationConstants.Persmissions.Create,
                ApplicationConstants.Persmissions.Edit,
                ApplicationConstants.Persmissions.Delete,
                ApplicationConstants.Persmissions.View
            });

        _ = result.Match(applicationRole =>
        {
            string message = string.Format("System administrator role with ID {0} has been successfully assigned with permissions {1}, {2}, {3}, {4}.",
                applicationRole.Id,
                ApplicationConstants.Persmissions.Create,
                ApplicationConstants.Persmissions.Edit,
                ApplicationConstants.Persmissions.Delete,
                ApplicationConstants.Persmissions.View);
            _logger.LogInformation(message);
            return applicationRole;
        }, LogUserRoleCreationExceptionFunc);

        #endregion System Administrator

        #region Tenant Administrator

        var tenantAdminRole = new ApplicationRole
        {
            Name = ApplicationConstants.Roles.TenantAdministrator,
            Description = "Tenant administrator role with permission to create, edit and view resources.",
            IsActive = true
        };

        result = await tenantAdminRole.TryCreateAsync(roleManager);
        ApplicationRole tenantAdminRoleEntity = result.Match(applicationRole =>
        {
            string message = $"Tenant administrator role with ID {applicationRole.Id} has been successfully created.";
            _logger.LogInformation(message);
            return applicationRole;
        }, LogUserRoleCreationExceptionFunc);

        result = await tenantAdminRoleEntity.TryAddPermissionsAsync(roleManager,
            new Collection<string>()
            {
                ApplicationConstants.Persmissions.Create,
                ApplicationConstants.Persmissions.Edit,
                ApplicationConstants.Persmissions.View
            });

        _ = result.Match(applicationRole =>
        {
            string message = string.Format("Tenant administrator role with ID {0} has been successfully assigned with permissions {1}, {2}, {3}.",
                applicationRole.Id,
                ApplicationConstants.Persmissions.Create,
                ApplicationConstants.Persmissions.Edit,
                ApplicationConstants.Persmissions.View);
            _logger.LogInformation(message);
            return applicationRole;
        }, LogUserRoleCreationExceptionFunc);

        #endregion Tenant Administrator

        #region Default Access

        var defaultAccessRole = new ApplicationRole
        {
            Name = ApplicationConstants.Roles.DefaultAccess,
            Description = "Default access role with permission to view resources.",
            IsActive = true
        };

        result = await defaultAccessRole.TryCreateAsync(roleManager);
        ApplicationRole defaultAccessRoleEntity = result.Match(applicationRole =>
        {
            string message = $"Default access role with ID {applicationRole.Id} has been successfully created.";
            _logger.LogInformation(message);
            return applicationRole;
        }, LogUserRoleCreationExceptionFunc);

        result = await defaultAccessRoleEntity.TryAddPermissionAsync(roleManager, ApplicationConstants.Persmissions.View);
        _ = result.Match(applicationRole =>
        {
            string message = $"Default access role with ID {applicationRole.Id} has been successfully assigned with permission {ApplicationConstants.Persmissions.View}.";
            _logger.LogInformation(message);
            return applicationRole;
        }, LogUserRoleCreationExceptionFunc);

        #endregion Default Access
    }

    private async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        Func<Exception, ApplicationUser> LogUserCreationExceptionFunc = (exception) =>
        {
            _logger.LogError(exception.Message);
            return default!;
        };

        #region System Administrator

        ApplicationUser systemUser = new()
        {
            Email = "system.admin@provider.dev",
            UserName = "system.admin@provider.dev",
            FirstName = "System",
            MiddleName = "Administrator",
            LastName = "User",
            Alias = "SysAdmin",
            EmailConfirmed = true,
            IsActive = true
        };

        const string systemUserPassword = "Pass123$systemAdministrator";

        Result<ApplicationUser> result = await systemUser.TryCreateAsync(userManager, systemUserPassword);
        ApplicationUser systemUserEntity = result.Match(
            applicationUser => { string message = $"System Administrator user with ID {applicationUser.Id} has been successfully created."; _logger.LogInformation(message); return applicationUser; },
            LogUserCreationExceptionFunc);

        if (systemUserEntity != null)
        {
            result = await systemUserEntity.TryAssignRolesAsync(
                userManager,
                roleManager,
                new string[] { ApplicationConstants.Roles.SystemAdministrator, ApplicationConstants.Roles.TenantAdministrator, ApplicationConstants.Roles.DefaultAccess });
            _ = result.Match(
                applicationUser =>
                {
                    string message = string.Format(
                    "System Administrator user with ID {0} has been successfully assigned roles {1}, {2}, {3}.",
                    applicationUser.Id,
                    ApplicationConstants.Roles.SystemAdministrator,
                    ApplicationConstants.Roles.TenantAdministrator,
                    ApplicationConstants.Roles.DefaultAccess); _logger.LogInformation(message); return applicationUser;
                },
                LogUserCreationExceptionFunc);
        }

        #endregion System Administrator

        #region Tenant Administrator

        ApplicationUser tenantUser = new()
        {
            Email = "tenant.admin@provider.dev",
            UserName = "tenant.admin@provider.dev",
            FirstName = "Tenant",
            MiddleName = "Administrator",
            LastName = "User",
            Alias = "TenantAdmin",
            EmailConfirmed = true,
            IsActive = true
        };

        const string tenantUserPassword = "Pass123$tenantAdministrator";

        result = await tenantUser.TryCreateAsync(userManager, tenantUserPassword);
        ApplicationUser tenantUserEntity = result.Match(
            applicationUser => { string message = $"Tenant Administrator user with ID {applicationUser.Id} has been successfully created."; _logger.LogInformation(message); return applicationUser; },
            LogUserCreationExceptionFunc);

        if (tenantUserEntity != null)
        {
            result = await tenantUserEntity.TryAssignRolesAsync(userManager, roleManager,
                new string[] { ApplicationConstants.Roles.TenantAdministrator, ApplicationConstants.Roles.DefaultAccess });
            _ = result.Match(
                applicationUser =>
                {
                    var message = string.Format(
                    "Tenant Administrator user with ID {0} has been successfully assigned roles {1}, {2}.",
                    applicationUser.Id,
                    ApplicationConstants.Roles.TenantAdministrator,
                    ApplicationConstants.Roles.DefaultAccess); _logger.LogInformation(message); return applicationUser;
                },
                LogUserCreationExceptionFunc);
        }

        #endregion Tenant Administrator

        #region Default Access

        ApplicationUser defaultUser = new()
        {
            Email = "default.access@provider.dev",
            UserName = "default.access@provider.dev",
            FirstName = "Default",
            LastName = "Access",
            Alias = "DefaultAccess",
            EmailConfirmed = true,
            IsActive = true
        };

        const string defaultUserPassword = "Pass123$defaultAccess";

        result = await defaultUser.TryCreateAsync(userManager, defaultUserPassword);
        ApplicationUser defaultUserEntity = result.Match(
            applicationUser => { string message = $"Default Access user with ID {applicationUser.Id} has been successfully created."; _logger.LogInformation(message); return applicationUser; },
            LogUserCreationExceptionFunc);

        if (defaultUserEntity != null)
        {
            result = await defaultUserEntity.TryAssignRoleAsync(userManager, roleManager);
            _ = result.Match(
                applicationUser => { string message = string.Format("Default Access user with ID {0} has been successfully assigned role {1}.", applicationUser.Id, ApplicationConstants.Roles.DefaultAccess); _logger.LogInformation(message); return applicationUser; },
                LogUserCreationExceptionFunc);
        }

        #endregion Default Access
    }

    #endregion Private Methods

    #region Public Constructors

    public UserSeed(IServiceProvider serviceProvider, ILogger<UserSeed> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using (var scope = _serviceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDatabaseContext>();
            await context.Database.EnsureCreatedAsync();

            UserManager<ApplicationUser>? userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager), $"User Manager cannot be a null object. Parameter: {nameof(userManager)} Value: {userManager}");
            }

            RoleManager<ApplicationRole>? roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            if (roleManager == null)
            {
                throw new ArgumentNullException(nameof(roleManager), $"User Manager cannot be a null object. Parameter: {nameof(roleManager)} Value: {roleManager}");
            }

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager, roleManager);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion Public Methods
}