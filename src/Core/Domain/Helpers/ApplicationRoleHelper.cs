using Domain.Constants;
using Domain.Data.Entities;
using System.Collections.ObjectModel;

namespace Domain.Helpers;

public static class ApplicationRoleHelper
{
    #region Public Methods

    public static IReadOnlyCollection<string> GetRolePermissions(this ApplicationRoleEntity role)
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

    public static IReadOnlyCollection<ApplicationRoleEntity> GetSubRoles(this string roleName)
    {
        return roleName switch
        {
            ApplicationConstants.Roles.SystemAdministrator => new Collection<ApplicationRoleEntity>()
                {
                    new ApplicationRoleEntity
                    {
                        Name = ApplicationConstants.Roles.SystemAdministrator,
                        Description = ApplicationConstants.Descriptions.SystemAdministrator,
                        IsActive = true
                    },
                    new ApplicationRoleEntity
                    {
                        Name = ApplicationConstants.Roles.TenantAdministrator,
                        Description = ApplicationConstants.Descriptions.TenantAdministrator,
                        IsActive = true
                    },
                    new ApplicationRoleEntity
                    {
                        Name = ApplicationConstants.Roles.DefaultAccess,
                        Description = ApplicationConstants.Descriptions.DefaultAccess,
                        IsActive = true
                    }
                },
            ApplicationConstants.Roles.TenantAdministrator => new Collection<ApplicationRoleEntity>()
                {
                    new ApplicationRoleEntity
                    {
                        Name = ApplicationConstants.Roles.TenantAdministrator,
                        Description = ApplicationConstants.Descriptions.TenantAdministrator,
                        IsActive = true
                    },
                    new ApplicationRoleEntity
                    {
                        Name = ApplicationConstants.Roles.DefaultAccess,
                        Description = ApplicationConstants.Descriptions.DefaultAccess,
                        IsActive = true
                    }
                },
            ApplicationConstants.Roles.DefaultAccess => new Collection<ApplicationRoleEntity>()
                {
                    new ApplicationRoleEntity
                    {
                        Name = ApplicationConstants.Roles.DefaultAccess,
                        Description = ApplicationConstants.Descriptions.DefaultAccess,
                        IsActive = true
                    }
                },
            _ => new Collection<ApplicationRoleEntity>()
        };
    }

    #endregion Public Methods
}