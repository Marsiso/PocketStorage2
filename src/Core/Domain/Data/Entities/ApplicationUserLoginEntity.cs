using Microsoft.AspNetCore.Identity;

namespace Domain.Data.Entities;

public sealed class ApplicationUserLoginEntity : IdentityUserLogin<Guid>
{
}