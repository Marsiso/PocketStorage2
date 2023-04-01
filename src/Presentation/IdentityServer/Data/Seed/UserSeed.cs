using Domain.Constants;
using Domain.Data;
using Domain.Data.Entities;
using Domain.Helpers;
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

    private record UserWithRoles(ApplicationUserEntity Value, string Password, IReadOnlyCollection<ApplicationRoleEntity> Roles);

    #region Private Methods

    private ICollection<UserWithRoles> GetUsersWithRoles()
    {
        return new Collection<UserWithRoles>()
        {
            new(
                Value: new ApplicationUserEntity
                {
                    Email = "system.admin@provider.dev",
                    UserName = "system.admin@provider.dev",
                    GivenName = "System",
                    OtherName = "Administrator",
                    FamilyName = "User",
                    EmailConfirmed = true,
                    IsActive = true
                },
                Password: "Pass123$systemAdministrator",
                Roles: ApplicationConstants.Roles.SystemAdministrator.GetSubRoles()
            ),
            new (
                Value: new ApplicationUserEntity
                {
                    Email = "tenant.admin@provider.dev",
                    UserName = "tenant.admin@provider.dev",
                    GivenName = "Tenant",
                    OtherName = "Administrator",
                    FamilyName = "User",
                    EmailConfirmed = true,
                    IsActive = true
                },
                Password: "Pass123$tenantAdministrator",
                Roles: ApplicationConstants.Roles.TenantAdministrator.GetSubRoles()
            ),
            new(
                Value: new ApplicationUserEntity
                {
                    Email = "default.access@provider.dev",
                    UserName = "default.access@provider.dev",
                    GivenName = "Default",
                    OtherName = "Access",
                    FamilyName = "User",
                    EmailConfirmed = true,
                    IsActive = true
                },
                Password: "Pass123$defaultAccess",
                Roles: ApplicationConstants.Roles.DefaultAccess.GetSubRoles()
            )
        };
    }

    private async Task SeedAsync(
        UserManager<ApplicationUserEntity> userManager,
        RoleManager<ApplicationRoleEntity> roleManager)
    {
        foreach (var user in GetUsersWithRoles())
        {
            await SeedRolesAsync(roleManager, user);
            await SeedUserAsync(userManager, roleManager, user);
        }
    }

    private async Task SeedRolesAsync(
        RoleManager<ApplicationRoleEntity> roleManager,
        UserWithRoles user)
    {
        foreach (var role in user.Roles)
        {
            Result<ApplicationRoleEntity> result = await role.TryCreateAsync(roleManager);
            ApplicationRoleEntity roleEntity = result.Match(appRole =>
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

    private async Task SeedUserAsync(
                UserManager<ApplicationUserEntity> userManager,
        RoleManager<ApplicationRoleEntity> roleManager,
        UserWithRoles user)
    {
        Result<ApplicationUserEntity> result = await user.Value.TryCreateAsync(userManager, user.Password);
        ApplicationUserEntity userEntity = result.Match(
            appUser =>
            {
                string message = $"User with ID {appUser.Id} and email address {appUser.Email} has been successfully created.";
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
                    string message = $"User with ID {appUser.Id} and email address {appUser.Email} has been successfully assigned roles {string.Join(", ", user.Roles)}.";
                    _logger.LogInformation(message);

                    return appUser;
                }, exception =>
                {
                    _logger.LogError(exception.Message);
                    return default!;
                });
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
        await using var scope = _serviceProvider.CreateAsyncScope();
        ApplicationDbContext? context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (context == null)
        {
            string errorMessage = $"[{nameof(UserSeed)}] Null reference exception. Parameter: '{nameof(context)}' Value: '{context}'";
            throw new NullReferenceException(errorMessage);
        }

        await context.Database.EnsureCreatedAsync();

        UserManager<ApplicationUserEntity>? userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUserEntity>>();
        if (userManager == null)
        {
            string errorMessage = $"[{nameof(UserSeed)}] Null reference exception. Parameter: '{nameof(userManager)}' Value: '{userManager}'";
            throw new NullReferenceException(errorMessage);
        }

        RoleManager<ApplicationRoleEntity>? roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRoleEntity>>();
        if (roleManager == null)
        {
            string errorMessage = $"[{nameof(UserSeed)}] Null reference exception. Parameter: '{nameof(roleManager)}' Value: '{roleManager}'";
            throw new NullReferenceException(errorMessage);
        }

        await SeedAsync(userManager, roleManager);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion Public Methods
}