using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Entities;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    [ProtectedPersonalData]
    public string FirstName { get; set; } = default!;

    [ProtectedPersonalData]
    public string? MiddleName { get; set; }

    [ProtectedPersonalData]
    public string LastName { get; set; } = default!;

    [PersonalData]
    public string Alias { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public ApplicationUser()
    {
    }

    public ApplicationUser(string userName) : base(userName)
    {
    }
}
