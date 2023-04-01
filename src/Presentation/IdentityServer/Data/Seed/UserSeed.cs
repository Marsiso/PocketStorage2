using Domain.Constants;
using Domain.Data;
using Domain.Helpers;
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

    private record UserWithRoles(ApplicationUser Value, string Password, IReadOnlyCollection<ApplicationRole> Roles);

    #region Private Methods

    private ICollection<UserWithRoles> GetUsersWithRoles()
    {
        return new Collection<UserWithRoles>()
        {
            new(
                Value: new ApplicationUser
                {
                    Email = "system.admin@provider.dev",
                    UserName = "system.admin@provider.dev",
                    FirstName = "System",
                    MiddleName = "Administrator",
                    LastName = "User",
                    Alias = "SysAdmin",
                    EmailConfirmed = true,
                    IsActive = true
                },
                Password: "Pass123$systemAdministrator",
                Roles: ApplicationConstants.Roles.SystemAdministrator.GetSubRoles()
            ),
            new (
                Value: new ApplicationUser
                {
                    Email = "tenant.admin@provider.dev",
                    UserName = "tenant.admin@provider.dev",
                    FirstName = "Tenant",
                    MiddleName = "Administrator",
                    LastName = "User",
                    Alias = "TenantAdmin",
                    EmailConfirmed = true,
                    IsActive = true
                },
                Password: "Pass123$tenantAdministrator",
                Roles: ApplicationConstants.Roles.TenantAdministrator.GetSubRoles()
            ),
            new(
                Value: new ApplicationUser
                {
                    Email = "default.access@provider.dev",
                    UserName = "default.access@provider.dev",
                    FirstName = "Default",
                    LastName = "Access",
                    Alias = "DefaultAccess",
                    EmailConfirmed = true,
                    IsActive = true
                },
                Password: "Pass123$defaultAccess",
                Roles: ApplicationConstants.Roles.DefaultAccess.GetSubRoles()
            )
        };
    }

    private async Task SeedUserAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        UserWithRoles user)
    {
        Result<ApplicationUser> result = await user.Value.TryCreateAsync(userManager, user.Password);
        ApplicationUser userEntity = result.Match(
            appUser =>
            {
                string message = $"User with ID {appUser.Id} and email adress {appUser.Email} has been successfully created.";
                _logger.LogInformation(message);

                return appUser;
            },
            exception =>
            {
                _logger.LogError(exception.Message);
                return default!;
            });

        if (userEntity != null)
        {
            result = await userEntity.TryAssignRolesAsync(
                userManager,
                roleManager,
                user.Roles);
            _ = result.Match(
                appUser =>
                {
                    string message = $"User with ID {appUser.Id} and email adress {appUser.Email} has been successfully assigned roles {string.Join(", ", user.Roles)}.";
                    _logger.LogInformation(message);

                    return appUser;
                }, exception =>
                {
                    _logger.LogError(exception.Message);
                    return default!;
                });
        }
    }

    private async Task SeedRolesAsync(
        RoleManager<ApplicationRole> roleManager,
        UserWithRoles user)
    {
        foreach (var role in user.Roles)
        {
            Result<ApplicationRole> result = await role.TryCreateAsync(roleManager);
            ApplicationRole roleEntity = result.Match(appRole =>
            {
                string message = $"Role {appRole.Name} with ID {appRole.Id} has been successfully created.";
                _logger.LogInformation(message);
                return appRole;
            }, exception =>
            {
                _logger.LogError(exception.Message);
                return default!;
            });

            IReadOnlyCollection<string> permissions = role.GetRolePermissions();
            result = await roleEntity.TryAddPermissionsAsync(roleManager, permissions);

            _ = result.Match(appRole =>
            {
                string message = $"Role {appRole.Name} with ID {appRole.Id} has been successfully assigned with permissions {string.Join(", ", permissions)}.";
                _logger.LogInformation(message);
                return appRole;
            }, exception =>
            {
                _logger.LogError(exception.Message);
                return default!;
            });
        }
    }

    private async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        foreach (var user in GetUsersWithRoles())
        {
            await SeedRolesAsync(roleManager, user);
            await SeedUserAsync(userManager, roleManager, user);
        }
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
                string errorMessage = $"User Manager cannot be a null reference object. Parameter: {nameof(userManager)} Value: {userManager}";
                throw new ArgumentNullException(nameof(userManager), errorMessage);
            }

            RoleManager<ApplicationRole>? roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            if (roleManager == null)
            {
                string errorMessage = $"User Manager cannot be a null object. Parameter: {nameof(roleManager)} Value: {roleManager}";
                throw new ArgumentNullException(nameof(roleManager), errorMessage);
            }

            await SeedAsync(userManager, roleManager);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion Public Methods
}