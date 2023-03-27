using Domain.Data;
using Domain.Identity.Defaults;
using Domain.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityServer.Data.Seed;

public sealed class UserSeed : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserSeed> _logger;

    public UserSeed(IServiceProvider serviceProvider, ILogger<UserSeed> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using (var scope = _serviceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDatabaseContext>();
            await context.Database.EnsureCreatedAsync();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await SeedRoleSystemAdmin(roleManager);
            await SeedRoleTenantAdmin(roleManager);
            await SeedRoleDefaultAccess(roleManager);
            await AddUsers(userManager);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task AddUsers(UserManager<ApplicationUser> userManager)
    {
        #region System Administrator
        var sysuser = new ApplicationUser
        {
            Email = DefaultIdentityConstants.DefaultSystemAdminEmail,
            UserName = DefaultIdentityConstants.DefaultSystemAdminEmail,
            FirstName = "System",
            LastName = "Administrator",
            Alias = "SysAdmin",
            EmailConfirmed = true,
            IsActive = true
        };

        var superUserInDb = await userManager.FindByEmailAsync(sysuser.Email);
        if (superUserInDb == null)
        {
            var result = await userManager.CreateAsync(sysuser, DefaultIdentityConstants.DefaultSystemAdminPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(sysuser, DefaultIdentityRoles.SystemAdmin);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultIdentityConstants.SystemAdministratorDesc + " User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(sysuser, DefaultIdentityRoles.TenantAdmin);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultIdentityConstants.TenantAdministratorDesc + " User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(sysuser, DefaultIdentityRoles.DefaultAccess);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultIdentityConstants.DefaultAccessDesc + " User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }
        }
        #endregion

        #region Tenant Administrator
        var tenantuser = new ApplicationUser
        {
            Email = DefaultIdentityConstants.DefaultTenantAdminEmail,
            UserName = DefaultIdentityConstants.DefaultTenantAdminEmail,
            FirstName = "Tenant",
            LastName = "Administrator",
            Alias = "TenantAdmin",
            EmailConfirmed = true,
            IsActive = true
        };

        var tenantUserInDb = await userManager.FindByEmailAsync(tenantuser.Email);
        if (tenantUserInDb == null)
        {
            var result = await userManager.CreateAsync(tenantuser, DefaultIdentityConstants.DefaultTenantAdminPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(tenantuser, DefaultIdentityRoles.TenantAdmin);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultIdentityConstants.TenantAdministratorDesc + " User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(tenantuser, DefaultIdentityRoles.DefaultAccess);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultIdentityConstants.DefaultAccessDesc + " User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }
        }
        #endregion

        #region Default
        var defaultuser = new ApplicationUser
        {
            Email = DefaultIdentityConstants.DefaultAccessEmail,
            UserName = DefaultIdentityConstants.DefaultAccessEmail,
            FirstName = "Default",
            LastName = "Access",
            Alias = "DefaultAccess",
            EmailConfirmed = true,
            IsActive = true
        };

        var defaultUserInDb = await userManager.FindByEmailAsync(defaultuser.Email);
        if (defaultUserInDb == null)
        {
            var result = await userManager.CreateAsync(defaultuser, DefaultIdentityConstants.DefaultAccessPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(defaultuser, DefaultIdentityRoles.DefaultAccess);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultIdentityConstants.DefaultAccessDesc + " User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }
        }
        #endregion
    }

    private async Task<ApplicationRole> AddUser(RoleManager<ApplicationRole> roleManager, string roleName, CancellationToken cancellationToken = default)
    {
        var role = new ApplicationRole { Name = roleName };
        var roleInDb = await roleManager.FindByNameAsync(roleName);
        if (roleInDb == null)
        {
            await roleManager.CreateAsync(role);
        }

        _logger.LogInformation($"Success Seed Role {roleName}");

        return role;
    }

    private static async Task SeedRoleSystemAdmin(RoleManager<ApplicationRole> roleManager)
    {
        var adminRole = await roleManager.FindByNameAsync(DefaultIdentityRoles.SystemAdmin);
        if (adminRole == null)
        {
            var role = new ApplicationRole
            {
                Name = DefaultIdentityRoles.SystemAdmin,
                Description = DefaultIdentityConstants.SystemAdministratorDesc,
                IsActive = true
            };

            await roleManager.CreateAsync(role);
            adminRole = role;
        }

        var allClaims = await roleManager.GetClaimsAsync(adminRole);

        var defaultPermissions = new List<string>()
        {
            DefaultIdentityPermissions.Create,
            DefaultIdentityPermissions.View,
            DefaultIdentityPermissions.Edit,
            DefaultIdentityPermissions.Delete
        };

        foreach (var permission in defaultPermissions)
        {
            if (!allClaims.Any(claim => claim.Type == "Permission" && claim.Value == permission))
            {
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", permission));
            }
        }
    }

    private static async Task SeedRoleTenantAdmin(RoleManager<ApplicationRole> roleManager)
    {
        var adminRole = await roleManager.FindByNameAsync(DefaultIdentityRoles.TenantAdmin);
        if (adminRole == null)
        {
            var role = new ApplicationRole
            {
                Name = DefaultIdentityRoles.TenantAdmin,
                Description = DefaultIdentityConstants.TenantAdministratorDesc,
                IsActive = true
            };
            await roleManager.CreateAsync(role);
            adminRole = role;
        }

        var allClaims = await roleManager.GetClaimsAsync(adminRole);

        var defaultPermissions = new List<string>()
        {
            DefaultIdentityPermissions.Create,
            DefaultIdentityPermissions.View,
            DefaultIdentityPermissions.Edit
        };

        foreach (var permission in defaultPermissions)
        {
            if (!allClaims.Any(claim => claim.Type == "Permission" && claim.Value == permission))
            {
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", permission));
            }
        }
    }

    private static async Task SeedRoleDefaultAccess(RoleManager<ApplicationRole> roleManager)
    {
        var defaultAccessRole = await roleManager.FindByNameAsync(DefaultIdentityRoles.DefaultAccess);
        if (defaultAccessRole == null)
        {
            var role = new ApplicationRole
            {
                Name = DefaultIdentityRoles.DefaultAccess,
                Description = DefaultIdentityConstants.DefaultAccessDesc,
                IsActive = true
            };
            await roleManager.CreateAsync(role);
            defaultAccessRole = role;
        }

        var allClaims = await roleManager.GetClaimsAsync(defaultAccessRole);

        var defaultPermissions = new List<string>()
        {
            DefaultIdentityPermissions.View
        };

        foreach (var permission in defaultPermissions)
        {
            if (!allClaims.Any(claim => claim.Type == "Permission" && claim.Value == permission))
            {
                await roleManager.AddClaimAsync(defaultAccessRole, new Claim("Permission", permission));
            }
        }
    }
}
