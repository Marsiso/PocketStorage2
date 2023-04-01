namespace Domain.Data.Models.Identity;

public sealed class ApplicationUserInfo
{
    #region Public Fields

    public static readonly ApplicationUserInfo Anonymous = new();

    #endregion Public Fields

    #region Public Properties

    public ICollection<ApplicationClaimValue> Claims { get; set; } = new List<ApplicationClaimValue>();
    public bool IsAuthenticated { get; set; }

    public string NameClaimType { get; set; } = string.Empty;

    public string RoleClaimType { get; set; } = string.Empty;

    #endregion Public Properties
}