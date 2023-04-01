namespace Domain.Constants;

public static class ApplicationConstants
{
    #region Public Classes

    public static class Persmissions
    {
        #region Public Fields

        public const string Create = "Create";
        public const string Delete = "Delete";
        public const string Edit = "Edit";
        public const string View = "View";

        #endregion Public Fields
    }

    public static class Roles
    {
        #region Public Fields

        public const string DefaultAccess = "DefaultAccess";
        public const string SystemAdministrator = "SystemAdministrator";
        public const string TenantAdministrator = "TenantAdministrator";

        #endregion Public Fields
    }

    public static class Descriptions
    {
        public const string DefaultAccess = "Default access role with permission to view resources.";
        public const string SystemAdministrator = "System administrator role with permission to create, edit, delete and view resources.";
        public const string TenantAdministrator = "Tenant administrator role with permission to create, edit and view resources.";
    }

    #endregion Public Classes
}