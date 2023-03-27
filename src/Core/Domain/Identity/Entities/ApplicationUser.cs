using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    [ProtectedPersonalData]
    public virtual string FirstName { get; set; } = default!;

    [ProtectedPersonalData]
    public virtual string? MiddleName { get; set; }

    [ProtectedPersonalData]
    public virtual string LastName { get; set; } = default!;

    [PersonalData]
    public virtual string Alias { get; set; } = default!;

    public virtual bool IsActive { get; set; } = true;

    public ApplicationUser()
    {
    }

    public ApplicationUser(string userName) : base(userName)
    {
    }
}
