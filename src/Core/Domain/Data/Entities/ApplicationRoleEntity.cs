using Domain.Helpers;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace Domain.Data.Entities;

public sealed class ApplicationRoleEntity : IdentityRole<Guid>
{
    #region Public Properties

    [DisplayName("Description")]
    [DataType(DataType.Text, ErrorMessage = "Invalid data type for application role property '{0}'.")]
    [StringLength(500, ErrorMessage = "Application role property '{0}' cannot exceed '{1}' characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Application role property '{0}' is required.")]
    [DisplayName("Account Activity Status")]
    [DataType(DataType.Custom, ErrorMessage = "Invalid data type for application user property '{0}'.")]
    public bool IsActive { get; set; } = true;

    #endregion Public Properties

    #region Public Methods

    public ApplicationRoleEntity MapSelf(ApplicationRoleEntity role)
    {
        Id = role.Id;
        Name = role.Name;
        NormalizedName = role.NormalizedName;
        Description = role.Description;
        IsActive = role.IsActive;
        ConcurrencyStamp = role.ConcurrencyStamp;

        return this;
    }

    public async Task<Result<ApplicationRoleEntity>> TryAddPermissionsAsync(RoleManager<ApplicationRoleEntity> roleManager, IReadOnlyCollection<string>? permissions = default)
    {
        if (roleManager == null)
        {
            string errorMessage = $"[{nameof(ApplicationRoleEntity)}] Null reference exception. Parameter name: '{nameof(roleManager)}' Value: '{roleManager}'";
            return new Result<ApplicationRoleEntity>(new ArgumentNullException(nameof(roleManager), errorMessage));
        }

        try
        {
            permissions ??= this.GetRolePermissions();
            IList<Claim> claims = await roleManager.GetClaimsAsync(this);
            foreach (var permission in permissions)
            {
                if (!claims.Any(claim => claim.Type == "Permission" && claim.Value == permission))
                {
                    await roleManager.AddClaimAsync(this, new Claim("Permission", permission));
                }
            }
        }
        catch (Exception exception)
        {
            return new Result<ApplicationRoleEntity>(exception);
        }

        return this;
    }

    public async Task<Result<ApplicationRoleEntity>> TryCreateAsync(RoleManager<ApplicationRoleEntity> roleManager)
    {
        if (roleManager == null)
        {
            string errorMessage = $"[{nameof(ApplicationRoleEntity)}] Null reference exception. Parameter name: '{nameof(roleManager)}' Value: '{roleManager}'";
            return new Result<ApplicationRoleEntity>(new ArgumentNullException(nameof(roleManager), errorMessage));
        }

        try
        {
            ApplicationRoleEntity? roleEntity = await roleManager.FindByNameAsync(Name ?? string.Empty);
            if (roleEntity == null)
            {
                IdentityResult identityResult = await roleManager.CreateAsync(this);
                if (!identityResult.Succeeded)
                {
                    return new Result<ApplicationRoleEntity>(new ArgumentNullException(nameof(identityResult.Errors),
                    string.Join(", ", identityResult.Errors.Select(error => error.Description))));
                }
            }
            else
            {
                MapSelf(roleEntity);
            }
        }
        catch (Exception exception)
        {
            return new Result<ApplicationRoleEntity>(exception);
        }

        return this;
    }

    #endregion Public Methods
}