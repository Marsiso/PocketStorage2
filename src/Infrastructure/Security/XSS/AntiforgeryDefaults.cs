namespace Infrastructure.Security.XSS;

public static class AntiforgeryDefaults
{
    #region Public Fields

    public const string CookieName = "__Host-X-XSRF-TOKEN";
    public const string HeaderName = "X-XSRF-TOKEN";

    #endregion Public Fields
}