using Domain.Constants;
using Domain.Identity.Entities;
using System.Collections.ObjectModel;

namespace Domain.Helpers;

public static class ApplicationRoleHelper
{
    public static IReadOnlyCollection<string> GetRolePermissions(this ApplicationRole role)
    {
        return role.Name switch
        {
            ApplicationConstants.Roles.SystemAdministrator => new Collection<string>
                {
                    ApplicationConstants.Persmissions.Create,
                    ApplicationConstants.Persmissions.Edit,
                    ApplicationConstants.Persmissions.Delete,
                    ApplicationConstants.Persmissions.View
                },
            ApplicationConstants.Roles.TenantAdministrator => new Collection<string>
                {
                    ApplicationConstants.Persmissions.Create,
                    ApplicationConstants.Persmissions.Edit,
                    ApplicationConstants.Persmissions.View
                },
            ApplicationConstants.Roles.DefaultAccess => new Collection<string>
                {
                    ApplicationConstants.Persmissions.View
                },
            _ => new Collection<string>(),
        };
    }

    public static IReadOnlyCollection<ApplicationRole> GetSubRoles(this string roleName)
    {
        return roleName switch
        {
            ApplicationConstants.Roles.SystemAdministrator => new Collection<ApplicationRole>()
                {

                    new ApplicationRole
                    {
                        Name = ApplicationConstants.Roles.SystemAdministrator,
                        Description = ApplicationConstants.Descriptions.SystemAdministrator,
                        IsActive = true
                    },
                    new ApplicationRole
                    {
                        Name = ApplicationConstants.Roles.TenantAdministrator,
                        Description = ApplicationConstants.Descriptions.TenantAdministrator,
                        IsActive = true
                    },
                    new ApplicationRole
                    {
                        Name = ApplicationConstants.Roles.DefaultAccess,
                        Description = ApplicationConstants.Descriptions.DefaultAccess,
                        IsActive = true
                    }
                },
            ApplicationConstants.Roles.TenantAdministrator => new Collection<ApplicationRole>()
                {
                    new ApplicationRole
                    {
                        Name = ApplicationConstants.Roles.TenantAdministrator,
                        Description = ApplicationConstants.Descriptions.TenantAdministrator,
                        IsActive = true
                    },
                    new ApplicationRole
                    {
                        Name = ApplicationConstants.Roles.DefaultAccess,
                        Description = ApplicationConstants.Descriptions.DefaultAccess,
                        IsActive = true
                    }
                },
            ApplicationConstants.Roles.DefaultAccess => new Collection<ApplicationRole>()
                {
                    new ApplicationRole
                    {
                        Name = ApplicationConstants.Roles.DefaultAccess,
                        Description = ApplicationConstants.Descriptions.DefaultAccess,
                        IsActive = true
                    }
                },
            _ => new Collection<ApplicationRole>()
        };
    }
}