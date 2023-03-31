using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Entities;

public sealed class ApplicationUserToken : IdentityUserToken<Guid>
{
}