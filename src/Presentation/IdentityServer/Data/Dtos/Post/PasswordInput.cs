using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Domain.Constants.ApplicationConstants;

namespace IdentityServer.Data.Dtos.Post;

public sealed class PasswordInput
{
    #region Public Properties

    [Required(ErrorMessage = Account.Password.RequiredErrorMessage)]
    [DataType(DataType.Password, ErrorMessage = Account.Password.DataTypeErrorMessage)]
    [DisplayName(Account.Password.DisplayName)]
    public string Password { get; set; } = default!;

    #endregion Public Properties
}