using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public virtual bool IsActive { get; set; } = true;

    public virtual string? Description { get; set; }
}
