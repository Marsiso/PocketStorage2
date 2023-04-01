using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Domain.Constants.ApplicationConstants;

namespace IdentityServer.Data.Dtos.Post;

public sealed class SigninInput
{
    #region Public Properties

    [DataType(DataType.Password, ErrorMessage = Account.ConfirmPassword.DataTypeErrorMessage)]
    [DisplayName(Account.ConfirmPassword.DisplayName)]
    [Compare(nameof(Password), ErrorMessage = Account.ConfirmPassword.ComparisonErrorMessage)]
    public string ConfirmPassword { get; set; } = default!;

    [Required(ErrorMessage = Account.Email.RequiredErrorMessage)]
    [EmailAddress(ErrorMessage = Account.Email.FormatErrorMessage)]
    [DisplayName(Account.Email.DisplayName)]
    public string Email { get; set; } = default!;

    [DisplayName(Account.FamilyName.DisplayName)]
    [DataType(DataType.Text, ErrorMessage = Account.FamilyName.DataTypeErrorMessage)]
    [Required(ErrorMessage = Account.FamilyName.RequiredErrorMessage)]
    [StringLength(Account.FamilyName.MaximalLength, MinimumLength = Account.FamilyName.MinimalLength, ErrorMessage = Account.FamilyName.LengthErrorMessage)]
    public string FamilyName { get; set; } = default!;

    [Required(ErrorMessage = Account.GivenName.RequiredErrorMessage)]
    [DataType(DataType.Text, ErrorMessage = Account.GivenName.DataTypeErrorMessage)]
    [DisplayName(Account.GivenName.DisplayName)]
    [StringLength(Account.GivenName.MaximalLength, MinimumLength = Account.GivenName.MinimalLength, ErrorMessage = Account.GivenName.LengthErrorMessage)]
    public string GivenName { get; set; } = default!;

    [DisplayName(Account.OtherName.DisplayName)]
    [DataType(DataType.Text, ErrorMessage = Account.OtherName.DataTypeErrorMessage)]
    [StringLength(Account.OtherName.MaximalLength, MinimumLength = Account.OtherName.MinimalLength, ErrorMessage = Account.OtherName.LengthErrorMessage)]
    public string? OtherName { get; set; }

    [Required(ErrorMessage = Account.Password.RequiredErrorMessage)]
    [StringLength(Account.Password.MaximalLength, ErrorMessage = Account.Password.LengthErrorMessage, MinimumLength = Account.Password.MinimalLength)]
    [DataType(DataType.Password, ErrorMessage = Account.Password.DataTypeErrorMessage)]
    [DisplayName(Account.Password.DisplayName)]
    public string Password { get; set; } = default!;

    #endregion Public Properties
}