namespace Domain.Identity.Entities;

public sealed class ApplicationUserInfo
{
    public static readonly ApplicationUserInfo Anonymous = new();

    public bool IsAuthenticated { get; set; }

    public string NameClaimType { get; set; } = string.Empty;

    public string RoleClaimType { get; set; } = string.Empty;

    public ICollection<ApplicationClaimValue> Claims { get; set; } = new List<ApplicationClaimValue>();
}
