using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Domain.Constants.ApplicationConstants;

namespace IdentityServer.Data.Dtos.Post;

public sealed class ResendEmailConfirmationInput
{
    #region Public Properties

    [Required(ErrorMessage = Account.Email.RequiredErrorMessage)]
    [EmailAddress(ErrorMessage = Account.Email.FormatErrorMessage)]
    [DisplayName(Account.Email.DisplayName)]
    public string Email { get; set; } = default!;

    #endregion Public Properties
}