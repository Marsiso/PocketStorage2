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

    #endregion Public Classes
}