using Domain.Identity.Defaults;
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
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

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

    private async Task AddUsers(UserManager<IdentityUser> userManager)
    {
        #region System Administrator
        var sysuser = new IdentityUser
        {
            Email = DefaultConstants.DefaultSystemAdminEmail,
            UserName = DefaultConstants.DefaultSystemAdminEmail,
            // FirstName = "System.Administrator",
            EmailConfirmed = true,
            // IsActive = true
        };

        var superUserInDb = await userManager.FindByEmailAsync(sysuser.Email);
        if (superUserInDb == null)
        {
            var result = await userManager.CreateAsync(sysuser, DefaultConstants.DefaultSystemAdminPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(sysuser, DefaultRoles.SystemAdmin);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultConstants.SystemAdministratorDesc + " User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(sysuser, DefaultRoles.TenantAdmin);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultConstants.TenantAdministratorDesc + " User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(sysuser, DefaultRoles.DefaultAccess);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultConstants.DefaultAccessDesc + " User");
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
        var tenantuser = new IdentityUser
        {
            Email = DefaultConstants.DefaultTenantAdminEmail,
            UserName = DefaultConstants.DefaultTenantAdminEmail,
            // FirstName = "System.Administrator",
            EmailConfirmed = true,
            // IsActive = true
        };

        var tenantUserInDb = await userManager.FindByEmailAsync(tenantuser.Email);
        if (tenantUserInDb == null)
        {
            var result = await userManager.CreateAsync(tenantuser, DefaultConstants.DefaultTenantAdminPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(sysuser, DefaultRoles.TenantAdmin);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultConstants.TenantAdministratorDesc + " User");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(sysuser, DefaultRoles.DefaultAccess);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultConstants.DefaultAccessDesc + " User");
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
        var defaultuser = new IdentityUser
        {
            Email = DefaultConstants.DefaultAccessEmail,
            UserName = DefaultConstants.DefaultAccessEmail,
            // FirstName = "System.Administrator",
            EmailConfirmed = true,
            // IsActive = true
        };

        var defaultUserInDb = await userManager.FindByEmailAsync(defaultuser.Email);
        if (defaultUserInDb == null)
        {
            var result = await userManager.CreateAsync(defaultuser, DefaultConstants.DefaultAccessPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            result = await userManager.AddToRoleAsync(sysuser, DefaultRoles.DefaultAccess);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seed " + DefaultConstants.DefaultAccessDesc + " User");
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

    private async Task<IdentityRole> AddUser(RoleManager<IdentityRole> roleManager, string roleName, CancellationToken cancellationToken = default)
    {
        var role = new IdentityRole { Name = roleName };
        var roleInDb = await roleManager.FindByNameAsync(roleName);
        if (roleInDb == null)
        {
            await roleManager.CreateAsync(role);
        }

        _logger.LogInformation($"Success Seed Role {roleName}");

        return role;
    }

    private static async Task SeedRoleSystemAdmin(RoleManager<IdentityRole> roleManager)
    {
        var adminRole = await roleManager.FindByNameAsync(DefaultRoles.SystemAdmin);
        if (adminRole == null)
        {
            var role = new IdentityRole { Name = DefaultRoles.SystemAdmin };
            await roleManager.CreateAsync(role);
            adminRole = role;
        }

        var allClaims = await roleManager.GetClaimsAsync(adminRole);

        var defaultPermissions = new List<string>()
        {
            DefaultPermissions.Create,
            DefaultPermissions.View,
            DefaultPermissions.Edit,
            DefaultPermissions.Delete
        };

        foreach (var permission in defaultPermissions)
        {
            if (!allClaims.Any(claim => claim.Type == "Permission" && claim.Value == permission))
            {
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", permission));
            }
        }
    }

    private static async Task SeedRoleTenantAdmin(RoleManager<IdentityRole> roleManager)
    {
        var adminRole = await roleManager.FindByNameAsync(DefaultRoles.TenantAdmin);
        if (adminRole == null)
        {
            var role = new IdentityRole
            {
                Name = DefaultRoles.TenantAdmin,
                // Description = DefaultRole.TenantAdmin,
                // TenantId = tenant.Id,
                // IsActive = true
            };
            await roleManager.CreateAsync(role);
            adminRole = role;
        }

        var allClaims = await roleManager.GetClaimsAsync(adminRole);

        var defaultPermissions = new List<string>()
        {
            DefaultPermissions.Create,
            DefaultPermissions.View,
            DefaultPermissions.Edit
        };

        foreach (var permission in defaultPermissions)
        {
            if (!allClaims.Any(claim => claim.Type == "Permission" && claim.Value == permission))
            {
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", permission));
            }
        }
    }

    private static async Task SeedRoleDefaultAccess(RoleManager<IdentityRole> roleManager)
    {
        var defaultAccessRole = await roleManager.FindByNameAsync(DefaultRoles.DefaultAccess);
        if (defaultAccessRole == null)
        {
            var role = new IdentityRole
            {
                Name = DefaultRoles.DefaultAccess,
                // Description = DefaultRole.TenantAdmin,
                // TenantId = tenant.Id,
                // IsActive = true
            };
            await roleManager.CreateAsync(role);
            defaultAccessRole = role;
        }

        var allClaims = await roleManager.GetClaimsAsync(defaultAccessRole);

        var defaultPermissions = new List<string>()
        {
            DefaultPermissions.View
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
