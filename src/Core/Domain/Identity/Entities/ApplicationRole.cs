using Domain.Constants;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace Domain.Identity.Entities;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    #region Public Properties

    [DisplayName("Description")]
    [DataType(DataType.Text)]
    [StringLength(500, ErrorMessage = "Application role description length cannot exceed {1} characters.")]
    public string? Description { get; set; }

    [DisplayName("Is Active")]
    [DataType(DataType.Custom)]
    public bool IsActive { get; set; } = true;

    #endregion Public Properties

    #region Public Methods

    public ApplicationRole MapSelf(ApplicationRole role)
    {
        Id = role.Id;
        Name = role.Name;
        NormalizedName = role.NormalizedName;
        Description = role.Description;
        IsActive = role.IsActive;
        ConcurrencyStamp = role.ConcurrencyStamp;

        return this;
    }

    public async Task<Result<ApplicationRole>> TryAddPermissionAsync(RoleManager<ApplicationRole> roleManager, string? permission = ApplicationConstants.Persmissions.View)
    {
        if (roleManager == null)
        {
            string errorMessage = $"Role manager object for application user cannot be null. Parameter name: {nameof(roleManager)} Value: {roleManager}";
            return new Result<ApplicationRole>(new ArgumentNullException(nameof(roleManager), errorMessage));
        }

        try
        {
            permission ??= ApplicationConstants.Persmissions.View;
            IList<Claim> claims = await roleManager.GetClaimsAsync(this);
            if (!claims.Any(claim => claim.Type == "Permission" && claim.Value == permission))
            {
                await roleManager.AddClaimAsync(this, new Claim("Permission", permission));
            }
        }
        catch (Exception exception)
        {
            return new Result<ApplicationRole>(exception);
        }

        return this;
    }

    public async Task<Result<ApplicationRole>> TryAddPermissionsAsync(RoleManager<ApplicationRole> roleManager, IReadOnlyCollection<string>? permissions = default)
    {
        if (roleManager == null)
        {
            string errorMessage = $"Role manager object for application user cannot be null. Parameter name: {nameof(roleManager)} Value: {roleManager}";
            return new Result<ApplicationRole>(new ArgumentNullException(nameof(roleManager), errorMessage));
        }

        try
        {
            permissions ??= new List<string>() { ApplicationConstants.Persmissions.View };
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
            return new Result<ApplicationRole>(exception);
        }

        return this;
    }

    public async Task<Result<ApplicationRole>> TryCreateAsync(RoleManager<ApplicationRole> roleManager)
    {
        if (roleManager == null)
        {
            string errorMessage = $"Role manager object for application user cannot be null. Parameter name: {nameof(roleManager)} Value: {roleManager}";
            return new Result<ApplicationRole>(new ArgumentNullException(nameof(roleManager), errorMessage));
        }

        try
        {
            ApplicationRole? roleEntity = await roleManager.FindByNameAsync(this.Name!);
            if (roleEntity == null)
            {
                IdentityResult identityResult = await roleManager.CreateAsync(this);
                if (!identityResult.Succeeded)
                {
                    return new Result<ApplicationRole>(new ArgumentNullException(nameof(identityResult.Errors),
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
            return new Result<ApplicationRole>(exception);
        }

        return this;
    }

    #endregion Public Methods
}