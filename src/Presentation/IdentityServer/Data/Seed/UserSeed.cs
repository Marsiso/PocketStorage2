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
            await SeedRoleBasicUser(roleManager);
            await AddUser(userManager);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task AddUser(UserManager<IdentityUser> userManager)
    {
        var sysuser = new IdentityUser
        {
            Email = "sysadmin@utb.dev",
            UserName = "sysadmin@utb.dev",
            // FirstName = "System.Administrator",
            EmailConfirmed = true,
            // IsActive = true
        };

        var superUserInDb = await userManager.FindByEmailAsync(sysuser.Email);
        if (superUserInDb == null)
        {
            var rs = await userManager.CreateAsync(sysuser, DefaultConstants.DefaultPassword);
            if (!rs.Succeeded)
            {
                foreach (var error in rs.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }

            var result = await userManager.AddToRoleAsync(sysuser, DefaultRoles.SystemAdmin);
            result = await userManager.AddToRoleAsync(sysuser, DefaultRoles.TenantAdmin);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seeded Default Sysadmin User");
            }
            else
            {
                foreach (var error in rs.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }
        }
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

    private static async Task SeedRoleBasicUser(RoleManager<IdentityRole> roleManager)
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

    public static class DefaultPermissions
    {
        internal static readonly string View = "View";
        internal static readonly string Edit = "Edit";
        internal static readonly string Delete = "Delete";
        internal static readonly string Create = "Create";
    }

    public static class DefaultRoles
    {
        internal static readonly string SystemAdmin = "SystemAdministrator";
        internal static readonly string TenantAdmin = "TenantAdministrator";
    }

    public static class DefaultConstants
    {
        public const string SysAdminRole = "SystemAdministrator";
        public const string SysAdminDesc = "System Administrator";
        public const string BasicRole = "Basic";
        public const string UserRole = "User";
        public const string DefaultPassword = "Pass123$SystemAdmin";
        public const string DefaultTenantAdminPassword = "Pass123$TenantAdmin";
    }
}
