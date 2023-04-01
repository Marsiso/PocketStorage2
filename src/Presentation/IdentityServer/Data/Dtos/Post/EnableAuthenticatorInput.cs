using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Domain.Constants.ApplicationConstants;

namespace IdentityServer.Data.Dtos.Post;

public sealed class EnableAuthenticatorInput
{
    #region Public Properties

    [Required(ErrorMessage = Account.TwoFactorAuthenticationCode.RequiredErrorMessage)]
    [StringLength(Account.TwoFactorAuthenticationCode.MaximalLength, ErrorMessage = Account.TwoFactorAuthenticationCode.LengthErrorMessage, MinimumLength = Account.TwoFactorAuthenticationCode.MinimalLength)]
    [DataType(DataType.Text, ErrorMessage = Account.TwoFactorAuthenticationCode.DataTypeErrorMessage)]
    [DisplayName(Account.TwoFactorAuthenticationCode.DisplayName)]
    public string Code { get; set; } = default!;

    #endregion Public Properties
}