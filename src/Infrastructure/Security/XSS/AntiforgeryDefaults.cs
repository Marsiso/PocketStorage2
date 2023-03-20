namespace Infrastructure.Security.XSS;

public static class AntiforgeryDefaults
{
    public const string HeaderName = "X-XSRF-TOKEN";
    public const string CookieName = "__Host-X-XSRF-TOKEN";
}
