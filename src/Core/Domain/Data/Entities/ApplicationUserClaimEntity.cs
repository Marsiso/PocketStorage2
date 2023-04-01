using Microsoft.AspNetCore.Identity;

namespace Domain.Data.Entities;

public sealed class ApplicationUserClaimEntity : IdentityUserClaim<Guid>
{
}