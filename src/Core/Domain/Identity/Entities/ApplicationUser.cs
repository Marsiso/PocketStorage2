using Domain.Constants;
using LanguageExt.Common;
using LanguageExt.Pipes;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Domain.Identity.Entities;

/// <summary>
/// Application user entity that extends the <see cref="IdentityUser"/> entity in the indetity system.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>, ICloneable
{
    #region Public Constructors

    /// <summary>
    /// Default <see cref="ApplicationUser"/> parameterless constructor.
    /// </summary>
    public ApplicationUser()
    {
    }

    /// <summary>
    /// Constructor that takes one argument.
    /// </summary>
    /// <param name="userName">
    /// <see cref="ApplicationUser"/> unique identifier used to distinguish user from other users.
    /// </param>
    public ApplicationUser(string userName) : base(userName)
    {
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// <see cref="ApplicationUser"/> nickname.
    /// </summary>
    [PersonalData]
    [DisplayName("Alias")]
    [DataType(DataType.Text)]
    [StringLength(50, ErrorMessage = "Application user last name cannot exceed {1} characters.")]
    public string Alias { get; set; } = default!;

    /// <summary>
    /// <see cref="ApplicationUser"/> given name.
    /// </summary>
    [ProtectedPersonalData]
    [DisplayName("Given Name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Application user given name cannot be null object.")]
    [StringLength(50, ErrorMessage = "Application user given name cannot exceed {1} characters.")]
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// <see cref="ApplicationUser"/>'s account status that indicates whether account has been
    /// suspended or not.
    /// </summary>
    public bool IsActive { get; set; } = false;

    /// <summary>
    /// <see cref="ApplicationUser"/> family name.
    /// </summary>
    [ProtectedPersonalData]
    [DisplayName("Family Name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Application user family name cannot be null object.")]
    [StringLength(50, ErrorMessage = "Application user family name cannot exceed {1} characters.")]
    public string LastName { get; set; } = default!;

    /// <summary>
    /// <see cref="ApplicationUser"/> other name.
    /// </summary>
    [ProtectedPersonalData]
    [DisplayName("Other Name")]
    [DataType(DataType.Text)]
    [StringLength(50, ErrorMessage = "Application user other name cannot exceed {1} characters.")]
    public string? MiddleName { get; set; }

    #endregion Public Properties

    #region Private Methods

    /// <summary>
    /// Assigns <paramref name="role"/> to <paramref name="user"/>. If <paramref name="role"/> was
    /// not provided than the <see cref="DefaultIdentityConstants.DefaultAccessRole"/> is assigned instead.
    /// </summary>
    /// <param name="userManager">See <see cref="UserManager{TUser}"/> for further details.</param>
    /// <param name="roleManager">See <see cref="RoleManager{TRole}"/> for further details.</param>
    /// <param name="role"><see cref="ApplicationRole"/> to be assigned to the <paramref name="user"/>.</param>
    /// <returns>
    /// An user <paramref name="role"/> assignment result containing <paramref name="user"/> object
    /// with potential errors.
    /// </returns>
    private async Task<Result<ApplicationUser>> TryAddToRoleAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        string role = ApplicationConstants.Roles.DefaultAccess)
    {
        try
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                string errorMessage = $"User role does not exist in the database. Parameter name: {nameof(role)} Value: {role}";
                return new Result<ApplicationUser>(new NullReferenceException(errorMessage));
            }

            if (await userManager.IsInRoleAsync(this, role))
            {
                return this;
            }

            IdentityResult result = await userManager.AddToRoleAsync(this, role);
            if (!result.Succeeded)
            {
                return new Result<ApplicationUser>(new ArgumentNullException(nameof(result.Errors),
                        string.Join(", ", result.Errors.Select(error => error.Description))));
            }

            return this;
        }
        catch (Exception exception)
        {
            return new Result<ApplicationUser>(exception);
        }
    }

    #endregion Private Methods

    #region Public Methods

    public object Clone()
    {
        return MemberwiseClone();
    }

    /// <summary>
    /// Assigns <paramref name="role"/> to <paramref name="user"/>. If <paramref name="role"/> was
    /// not provided than the <see cref="DefaultIdentityConstants.DefaultAccessRole"/> is assigned instead.
    /// </summary>
    /// <param name="userManager">See <see cref="UserManager{TUser}"/> for further details.</param>
    /// <param name="roleManager">See <see cref="RoleManager{TRole}"/> for further details.</param>
    /// <param name="role"><see cref="ApplicationRole"/> to be assigned to the <paramref name="user"/>.</param>
    /// <returns>
    /// An user <paramref name="role"/> assignment result containing <paramref name="user"/> object
    /// with potential errors.
    /// </returns>
    public async Task<Result<ApplicationUser>> TryAssignRoleAsync(
        UserManager<ApplicationUser>? userManager,
        RoleManager<ApplicationRole>? roleManager,
        string? role = default)
    {
        if (userManager == null)
        {
            string errorMessage = $"User manager object for application user cannot be null. Parameter name: {nameof(userManager)} Value: {userManager}";
            return new Result<ApplicationUser>(new ArgumentNullException(nameof(userManager), errorMessage));
        }

        if (roleManager == null)
        {
            string errorMessage = $"Role manager object for application user cannot be null. Parameter name: {nameof(userManager)} Value: {userManager}";
            return new Result<ApplicationUser>(new ArgumentNullException(nameof(roleManager), errorMessage));
        }

        if (role == null)
        {
            return await TryAddToRoleAsync(userManager, roleManager);
        }

        return await TryAddToRoleAsync(userManager, roleManager, role);
    }

    /// <summary>
    /// Assigns <paramref name="roles"/> to <paramref name="user"/>. If <paramref name="roles"/>
    /// collection is either a null object or an empty collection than the default role is assigned instead.
    /// </summary>
    /// <param name="userManager">See <see cref="UserManager{TUser}"/> for further details.</param>
    /// <param name="roleManager">See <see cref="RoleManager{TRole}"/> for further details.</param>
    /// <param name="roles">
    /// <see cref="ApplicationRole"/> collection to be assigned to the <paramref name="user"/>.
    /// </param>
    /// <returns>
    /// An user <paramref name="roles"/> assignment result containing <paramref name="user"/> object
    /// with potential errors.
    /// </returns>
    public async Task<Result<ApplicationUser>> TryAssignRolesAsync(
        UserManager<ApplicationUser>? userManager,
        RoleManager<ApplicationRole>? roleManager,
        string[]? roles = default)
    {
        if (userManager == null)
        {
            string errorMessage = $"User manager object for application user cannot be null. Parameter name: {nameof(userManager)} Value: {userManager}";
            return new Result<ApplicationUser>(new ArgumentNullException(nameof(userManager), errorMessage));
        }

        if (roleManager == null)
        {
            string errorMessage = $"Role manager object for application user cannot be null. Parameter name: {nameof(userManager)} Value: {userManager}";
            return new Result<ApplicationUser>(new ArgumentNullException(nameof(roleManager), errorMessage));
        }

        if (roles == null)
        {
            return await TryAddToRoleAsync(userManager, roleManager);
        }

        foreach (var role in roles)
        {
            Result<ApplicationUser> result = await TryAddToRoleAsync(userManager, roleManager, role);
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
    public async Task<Result<ApplicationUser>> TryCreateAsync(
        UserManager<ApplicationUser>? userManager,
        string? userPassword)
    {
        if (userManager == null)
        {
            string errorMessage = $"User manager object for application user cannot be null. Parameter name: {nameof(userManager)} Value: {userManager}";
            return new Result<ApplicationUser>(new ArgumentNullException(nameof(userManager), errorMessage));
        }

        try
        {
            var userInDb = await userManager.FindByEmailAsync(Email ?? string.Empty);
            if (userInDb == null)
            {
                var result = await userManager.CreateAsync(this, userPassword ?? string.Empty);
                if (!result.Succeeded)
                {
                    return new Result<ApplicationUser>(new ArgumentNullException(nameof(result.Errors),
                        string.Join(", ", result.Errors.Select(error => error.Description))));
                }
            }
            else
            {
                return userInDb;
            }
        }
        catch (Exception exception)
        {
            return new Result<ApplicationUser>(exception);
        }

        return this;
    }

    #endregion Public Methods
}