namespace Domain.Constants;

public static class ApplicationConstants
{
    #region Public Classes

    public static class Account
    {
        #region Public Classes

        public static class ConfirmPassword
        {
            #region Public Fields

            public const string ComparisonErrorMessage = "Password and confirmation password do not match.";
            public const string DataTypeErrorMessage = "Invalid data type for property '{0}'.";
            public const string DisplayName = "Confirm Password";
            public const int MaximalLength = 100;
            public const int MinimalLength = 6;
            public const int MinimalUniqueCharacters = 1;

            #endregion Public Fields
        }

        public static class Email
        {
            #region Public Fields

            public const string DisplayName = "Email Address";
            public const string FormatErrorMessage = "Property '{0}' has invalid format.";
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class FamilyName
        {
            #region Public Fields

            public const string DataTypeErrorMessage = "Invalid data type for property '{0}'.";
            public const string DisplayName = "Family Name";
            public const string LengthErrorMessage = "Property '{0}' must be at least '{2}' and cannot exceed '{1}' characters.";
            public const int MaximalLength = 50;
            public const int MinimalLength = 2;
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class GivenName
        {
            #region Public Fields

            public const string DataTypeErrorMessage = "Invalid data type for property '{0}'.";
            public const string DisplayName = "Given Name";
            public const string LengthErrorMessage = "Property '{0}' must be at least '{2}' and cannot exceed '{1}' characters.";
            public const int MaximalLength = 50;
            public const int MinimalLength = 2;
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class IsActive
        {
            #region Public Fields

            public const string DisplayName = "Account Status";
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class Lockout
        {
            #region Public Fields

            public const int MaxFailedAccessAttempts = 5;

            #endregion Public Fields
        }

        public static class OtherName
        {
            #region Public Fields

            public const string DataTypeErrorMessage = "Invalid data type for property '{0}'.";
            public const string DisplayName = "Other Name";
            public const string LengthErrorMessage = "Property '{0}' must be at least '{2}' and cannot exceed '{1}' characters.";
            public const int MaximalLength = 50;
            public const int MinimalLength = 2;
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class Password
        {
            #region Public Fields

            public const string DataTypeErrorMessage = "Invalid data type for property '{0}'.";
            public const string DisplayName = "Password";
            public const string LengthErrorMessage = "Property '{0}' must be at least '{2}' and cannot exceed '{1}' characters.";
            public const int MaximalLength = 100;
            public const int MinimalLength = 6;
            public const int MinimalUniqueCharacters = 1;
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class RecoveryCode
        {
            #region Public Fields

            public const string DataTypeErrorMessage = "Invalid data type for property '{0}'.";
            public const string DisplayName = "Recovery Code";
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class RememberMachine
        {
            #region Public Fields

            public const string DisplayName = "Remember Machine";
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class RememberMe
        {
            #region Public Fields

            public const string DisplayName = "Remember Me";
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class ResetPasswordCode
        {
            #region Public Fields

            public const string DisplayName = "Reset Password Code";
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class TwoFactorAuthenticationCode
        {
            #region Public Fields

            public const string DataTypeErrorMessage = "Invalid data type for property '{0}'.";
            public const string DisplayName = "Two Factor Authentication Code";
            public const string LengthErrorMessage = "Property '{0}' must be at least '{2}' and cannot exceed '{1}' characters.";
            public const int MaximalLength = 7;
            public const int MinimalLength = 6;
            public const string RequiredErrorMessage = "Property '{0}' is required.";

            #endregion Public Fields
        }

        public static class Username
        {
            #region Public Fields

            public const string AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            #endregion Public Fields
        }

        #endregion Public Classes
    }

    public static class Descriptions
    {
        #region Public Fields

        public const string DefaultAccess = "Default access role with permission to view resources.";
        public const string SystemAdministrator = "System administrator role with permission to create, edit, delete and view resources.";
        public const string TenantAdministrator = "Tenant administrator role with permission to create, edit and view resources.";

        #endregion Public Fields
    }

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

    public static class Syncfussion
    {
        #region Public Fields

        public const string LicenseKey = "Mgo+DSMBaFt+QHFqVkFrXVNbdV5dVGpAd0N3RGlcdlR1fUUmHVdTRHRcQlljSH9XdkJjUHlXdHI=;Mgo+DSMBPh8sVXJ1S0d+X1ZPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXpTd0RnWXheeXNURmY=;NRAiBiAaIQQuGjN/V0d+XU9HcVRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TckViWHted3RcQWJYVw==;Mgo+DSMBMAY9C3t2VFhhQlJDfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5XdkRiXX9ZcH1SRWda;MTU4MjIxNkAzMjMxMmUzMTJlMzMzN2pPdFgrN0d4VkNMSEtqaEsyb21HbG5mZmJOUEluRGMxVitrWHVkSnFQZnM9;MTU4MjIxN0AzMjMxMmUzMTJlMzMzN09qMTVWL3NpSkNiSDUxcFBvcTQvSmdQeDdOLzNHUlQwenBJblEwN0NZbVE9";

        #endregion Public Fields
    }

    #endregion Public Classes
}