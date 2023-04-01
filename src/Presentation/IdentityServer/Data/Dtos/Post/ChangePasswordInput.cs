using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Data.Dtos.Post;

public sealed class ChangePasswordInput
{
    #region Public Properties

    [DataType(DataType.Password, ErrorMessage = "Invalid data type for change password input  property '{0}'.")]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = default!;

    [Required(ErrorMessage = "Change password input property '{0}' is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Change password input property '{0}' must be at least '{2}' and cannot exceed '{1}' characters.")]
    [DataType(DataType.Text, ErrorMessage = "Invalid data type for change password input  property '{0}'.")]
    [Display(Name = "New password")]
    public string NewPassword { get; set; } = default!;

    [Required(ErrorMessage = "Change password input property '{0}' is required.")]
    [DataType(DataType.Text, ErrorMessage = "Invalid data type for change password input  property '{0}'.")]
    [Display(Name = "Current password")]
    public string OldPassword { get; set; } = default!;

    #endregion Public Properties
}