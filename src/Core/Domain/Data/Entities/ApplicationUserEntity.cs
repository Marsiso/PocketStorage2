using Domain.Helpers;
using LanguageExt.Common;
using LanguageExt.Pipes;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using static Domain.Constants.ApplicationConstants;

namespace Domain.Data.Entities;

/// <summary>
/// Application user entity that extends the <see cref="IdentityUser"/> entity in the indetity system.
/// </summary>
public sealed class ApplicationUserEntity : IdentityUser<Guid>, ICloneable
{
    #region Public Constructors

    /// <summary>
    /// Default <see cref="ApplicationUserEntity"/> parameterless constructor.
    /// </summary>
    public ApplicationUserEntity()
    {
    }

    /// <summary>
    /// Constructor that takes one argument.
    /// </summary>
    /// <param name="userName">
    /// <see cref="ApplicationUserEntity"/> unique identifier used to distinguish user from other users.
    /// </param>
    public ApplicationUserEntity(string userName) : base(userName)
    {
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// <see cref="ApplicationUserEntity"/> family name.
    /// </summary>
    [ProtectedPersonalData]
    [DisplayName(Account.FamilyName.DisplayName)]
    [DataType(DataType.Text, ErrorMessage = Account.FamilyName.DataTypeErrorMessage)]
    [Required(ErrorMessage = Account.FamilyName.RequiredErrorMessage)]
    [StringLength(Account.FamilyName.MaximalLength, MinimumLength = Account.FamilyName.MinimalLength, ErrorMessage = Account.FamilyName.LengthErrorMessage)]
    public string FamilyName { get; set; } = default!;

    /// <summary>
    /// <see cref="ApplicationUserEntity"/> given name.
    /// </summary>
    [ProtectedPersonalData]
    [Required(ErrorMessage = Account.GivenName.RequiredErrorMessage)]
    [DataType(DataType.Text, ErrorMessage = Account.GivenName.DataTypeErrorMessage)]
    [DisplayName(Account.GivenName.DisplayName)]
    [StringLength(Account.GivenName.MaximalLength, MinimumLength = Account.GivenName.MinimalLength, ErrorMessage = Account.GivenName.LengthErrorMessage)]
    public string GivenName { get; set; } = default!;

    /// <summary>
    /// <see cref="ApplicationUserEntity"/>'s account status that indicates whether account has been
    /// suspended or not.
    /// </summary>
    [Required(ErrorMessage = Account.IsActive.RequiredErrorMessage)]
    [DisplayName(Account.IsActive.DisplayName)]
    public bool IsActive { get; set; } = false;

    /// <summary>
    /// <see cref="ApplicationUserEntity"/> other name.
    /// </summary>
    [ProtectedPersonalData]
    [DisplayName(Account.OtherName.DisplayName)]
    [DataType(DataType.Text, ErrorMessage = Account.OtherName.DataTypeErrorMessage)]
    [StringLength(Account.OtherName.MaximalLength, MinimumLength = Account.OtherName.MinimalLength, ErrorMessage = Account.OtherName.LengthErrorMessage)]
    public string? OtherName { get; set; } = default;

    #endregion Public Properties

    #region Private Methods

    /// <summary>
    /// Assigns <paramref name="role"/> to <paramref name="user"/>. If <paramref name="role"/> was
    /// not provided than the <see cref="DefaultIdentityConstants.DefaultAccessRole"/> is assigned instead.
    /// </summary>
    /// <param name="userManager">See <see cref="UserManager{TUser}"/> for further details.</param>
    /// <param name="roleManager">See <see cref="RoleManager{TRole}"/> for further details.</param>
    /// <param name="role">
    /// <see cref="ApplicationRoleEntity"/> to be assigned to the <paramref name="user"/>.
    /// </param>
    /// <returns>
    /// An user <paramref name="role"/> assignment result containing <paramref name="user"/> object
    /// with potential errors.
    /// </returns>
    private async Task<Result<ApplicationUserEntity>> TryAddToRoleAsync(
        UserManager<ApplicationUserEntity> userManager,
        RoleManager<ApplicationRoleEntity> roleManager,
        ApplicationRoleEntity role)
    {
        try
        {
            role.Name ??= string.Empty;
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                string errorMessage = $"User role does not exist in the database. Parameter name: {nameof(role)} Value: {role}";
                return new Result<ApplicationUserEntity>(new NullReferenceException(errorMessage));
            }

            if (await userManager.IsInRoleAsync(this, role.Name))
            {
                return this;
            }

            IdentityResult identityResult = await userManager.AddToRoleAsync(this, role.Name);
            if (!identityResult.Succeeded)
            {
                return new Result<ApplicationUserEntity>(new ArgumentNullException(nameof(identityResult.Errors),
                        string.Join(", ", identityResult.Errors.Select(error => error.Description))));
            }

            return this;
        }
        catch (Exception exception)
        {
            return new Result<ApplicationUserEntity>(exception);
        }
    }

    #endregion Private Methods

    #region Public Methods

    public object Clone()
    {
        return MemberwiseClone();
    }

    /// <summary>
    /// Assigns <paramref name="roles"/> to <paramref name="user"/>. If <paramref name="roles"/>
    /// collection is either a null object or an empty collection than the default role is assigned instead.
    /// </summary>
    /// <param name="userManager">See <see cref="UserManager{TUser}"/> for further details.</param>
    /// <param name="roleManager">See <see cref="RoleManager{TRole}"/> for further details.</param>
    /// <param name="roles">
    /// <see cref="ApplicationRoleEntity"/> collection to be assigned to the <paramref name="user"/>.
    /// </param>
    /// <returns>
    /// An user <paramref name="roles"/> assignment result containing <paramref name="user"/> object
    /// with potential errors.
    /// </returns>
    public async Task<Result<ApplicationUserEntity>> TryAssignRolesAsync(
        UserManager<ApplicationUserEntity>? userManager,
        RoleManager<ApplicationRoleEntity>? roleManager,
        IReadOnlyCollection<ApplicationRoleEntity>? roles = default)
    {
        if (userManager == null)
        {
            string errorMessage = $"[{nameof(ApplicationUserEntity)}] Null reference exception. Parameter name: '{nameof(userManager)}' Value: '{userManager}'";
            return new Result<ApplicationUserEntity>(new ArgumentNullException(nameof(userManager), errorMessage));
        }

        if (roleManager == null)
        {
            string errorMessage = $"[{nameof(ApplicationUserEntity)}] Null reference exception. Parameter name: '{nameof(userManager)}' Value: '{userManager}'";
            return new Result<ApplicationUserEntity>(new ArgumentNullException(nameof(roleManager), errorMessage));
        }

        roles ??= Roles.DefaultAccess.GetSubRoles();
        foreach (var role in roles)
        {
            Result<ApplicationUserEntity> result;
            if (role == null)
            {
                continue;
            }

            result = await TryAddToRoleAsync(userManager, roleManager, role);
            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return this;
    }

    /// <summary>
    /// Creates and stores <paramref name="user"/> in the persistence store.
    /// </summary>
    /// <param name="userManager">See <see cref="UserManager{TUser}"/> for further details.</param>
    /// <param name="userPassword">Password to be assigned to the <paramref name="user"/>.</param>
    /// <returns>
    /// An user creation result containing <paramref name="user"/> object with potential errors.
    /// </returns>
    public async Task<Result<ApplicationUserEntity>> TryCreateAsync(
        UserManager<ApplicationUserEntity>? userManager,
        string userPassword)
    {
        if (userManager == null)
        {
            string errorMessage = $"[{nameof(ApplicationUserEntity)}] Null reference exception. Parameter name: '{nameof(userManager)}' Value: '{userManager}'";
            return new Result<ApplicationUserEntity>(new ArgumentNullException(nameof(userManager), errorMessage));
        }

        try
        {
            ApplicationUserEntity? userInDb = await userManager.FindByEmailAsync(Email ?? string.Empty);
            if (userInDb == null)
            {
                IdentityResult identityResult = await userManager.CreateAsync(this, userPassword);
                if (!identityResult.Succeeded)
                {
                    return new Result<ApplicationUserEntity>(new ArgumentNullException(nameof(identityResult.Errors),
                        string.Join(", ", identityResult.Errors.Select(error => error.Description))));
                }
            }
            else
            {
                return userInDb;
            }
        }
        catch (Exception exception)
        {
            return new Result<ApplicationUserEntity>(exception);
        }

        return this;
    }

    #endregion Public Methods
}