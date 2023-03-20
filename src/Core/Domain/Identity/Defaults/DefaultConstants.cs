namespace Domain.Identity.Defaults;

public static class DefaultConstants
{
    #region Roles
    public const string SystemAdministratorRole = "SystemAdministrator";
    public const string TenantAdministratorRole = "TenantAdministrator";
    public const string DefaultAccessRole = "Default";
    #endregion

    #region Roles Desc
    public const string SystemAdministratorDesc = "System Administrator";
    public const string TenantAdministratorDesc = "Tenant Administrator";
    public const string DefaultAccessDesc = "Default Access";
    #endregion

    #region Default Accounts
    public const string DefaultSystemAdminEmail = "sysadmin@provider.dev";
    public const string DefaultSystemAdminPassword = "Pass123$SystemAdmin";

    public const string DefaultTenantAdminEmail = "tenantadmin@provider.dev";
    public const string DefaultTenantAdminPassword = "Pass123$TenantAdmin";

    public const string DefaultAccessEmail = "default@provider.dev";
    public const string DefaultAccessPassword = "Pass123$DefaultAccess";
    #endregion
}
