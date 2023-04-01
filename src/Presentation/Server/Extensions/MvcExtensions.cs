using Microsoft.AspNetCore.Mvc.Authorization;

namespace Server.Extensions;

public static class MvcExtensions
{
    #region Public Methods

    public static void Configure(this MvcOptions options)
    {
        AuthorizationPolicyBuilder policyBuilder = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser();

        AuthorizationPolicy policy = policyBuilder.Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    }

    #endregion Public Methods
}