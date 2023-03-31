using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Authorization;

public sealed class AuthorizeViewModel
{
    #region Public Properties

    [Display(Name = "Application")]
    public string? ApplicationName { get; set; }

    [Display(Name = "Scope")]
    public string? Scope { get; set; }

    #endregion Public Properties
}