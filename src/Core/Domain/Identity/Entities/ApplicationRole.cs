using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Entities;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }
}
