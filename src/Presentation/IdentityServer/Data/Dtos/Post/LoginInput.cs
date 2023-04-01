using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Domain.Constants.ApplicationConstants;

namespace IdentityServer.Data.Dtos.Post;

public sealed class LoginInput
{
    #region Public Properties

    [Required(ErrorMessage = Account.Email.RequiredErrorMessage)]
    [EmailAddress(ErrorMessage = Account.Email.FormatErrorMessage)]
    [DisplayName(Account.Email.DisplayName)]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = Account.Password.RequiredErrorMessage)]
    [DataType(DataType.Password, ErrorMessage = Account.Password.DataTypeErrorMessage)]
    [DisplayName(Account.Password.DisplayName)]
    public string Password { get; set; } = default!;

    [DisplayName(Account.RememberMe.DisplayName)]
    [Required(ErrorMessage = Account.RememberMe.RequiredErrorMessage)]
    public bool RememberMe { get; set; }

    #endregion Public Properties
}