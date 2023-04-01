using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Domain.Constants.ApplicationConstants;

namespace IdentityServer.Data.Dtos.Post;

public sealed class SetPasswordInput
{
    #region Public Properties

    [DataType(DataType.Password, ErrorMessage = Account.ConfirmPassword.DataTypeErrorMessage)]
    [DisplayName(Account.ConfirmPassword.DisplayName)]
    [Compare(nameof(NewPassword), ErrorMessage = Account.ConfirmPassword.ComparisonErrorMessage)]
    public string ConfirmPassword { get; set; } = default!;

    [Required(ErrorMessage = Account.Password.RequiredErrorMessage)]
    [StringLength(Account.Password.MaximalLength, ErrorMessage = Account.Password.LengthErrorMessage, MinimumLength = Account.Password.MinimalLength)]
    [DataType(DataType.Password, ErrorMessage = Account.Password.DataTypeErrorMessage)]
    [DisplayName(Account.Password.DisplayName)]
    public string NewPassword { get; set; } = default!;

    #endregion Public Properties
}