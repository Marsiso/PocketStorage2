using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Domain.Constants.ApplicationConstants;

namespace IdentityServer.Data.Dtos.Post;

public sealed class LoginWithRecoveryCodeInput
{
    #region Public Properties

    [BindProperty]
    [Required(ErrorMessage = Account.RecoveryCode.RequiredErrorMessage)]
    [DataType(DataType.Text, ErrorMessage = Account.RecoveryCode.DataTypeErrorMessage)]
    [DisplayName(Account.RecoveryCode.DisplayName)]
    public string RecoveryCode { get; set; } = default!;

    #endregion Public Properties
}