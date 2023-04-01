using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Domain.Constants.ApplicationConstants;

namespace IdentityServer.Data.Dtos.Post;

public sealed class ResetPasswordInput
{
    #region Public Properties

    [Required(ErrorMessage = Account.ResetPasswordCode.RequiredErrorMessage)]
    [DisplayName(Account.ResetPasswordCode.DisplayName)]
    public string Code { get; set; } = default!;

    [DataType(DataType.Password, ErrorMessage = Account.ConfirmPassword.DataTypeErrorMessage)]
    [DisplayName(Account.ConfirmPassword.DisplayName)]
    [Compare(nameof(Password), ErrorMessage = Account.ConfirmPassword.ComparisonErrorMessage)]
    public string ConfirmPassword { get; set; } = default!;

    [Required(ErrorMessage = Account.Email.RequiredErrorMessage)]
    [EmailAddress(ErrorMessage = Account.Email.FormatErrorMessage)]
    [DisplayName(Account.Email.DisplayName)]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = Account.Password.RequiredErrorMessage)]
    [StringLength(Account.Password.MaximalLength, ErrorMessage = Account.Password.LengthErrorMessage, MinimumLength = Account.Password.MinimalLength)]
    [DataType(DataType.Password, ErrorMessage = Account.Password.DataTypeErrorMessage)]
    [DisplayName(Account.Password.DisplayName)]
    public string Password { get; set; } = default!;

    #endregion Public Properties
}