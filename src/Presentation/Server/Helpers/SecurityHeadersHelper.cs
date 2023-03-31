namespace Server.Helpers;

/// <summary>
/// Security header helper class.
/// </summary>
public static class SecurityHeadersHelper
{
    #region Public Methods

    public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDevelopmentEnvironment, string identityProviderHost)
    {
        if (string.IsNullOrEmpty(identityProviderHost))
        {
            throw new ArgumentNullException(nameof(identityProviderHost), $"OpenID Connect Identity provider host, parameter {nameof(identityProviderHost)}, cannot be a null or empty string.");
        }

        var policy = new HeaderPolicyCollection()
            .AddFrameOptionsDeny()
            .AddXssProtectionBlock()
            .AddContentTypeOptionsNoSniff()
            .AddReferrerPolicyStrictOriginWhenCrossOrigin()
            .AddCrossOriginOpenerPolicy(builder => builder.SameOrigin())
            .AddCrossOriginResourcePolicy(builder => builder.SameOrigin())
            // Remove for developers if using hot reload
            .AddCrossOriginEmbedderPolicy(builder => builder.RequireCorp())
            .AddContentSecurityPolicy(builder =>
            {
                builder.AddObjectSrc().None();
                builder.AddBlockAllMixedContent();
                builder.AddImgSrc().Self().From("data:");
                builder.AddFormAction().Self().From(identityProviderHost);
                builder.AddFontSrc().Self();
                builder.AddStyleSrc().Self();
                builder.AddBaseUri().Self();
                builder.AddFrameAncestors().None();

                // Due to Blazor
                builder
                    .AddScriptSrc()
                    .Self()
                    .WithHash256("v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA=")
                    .UnsafeEval();

                // Disable script and style CSP protection if using Blazor hot reload (if using hot
                // reload, DO NOT deploy with an insecure CSP)
            })
            .RemoveServerHeader()
            .AddPermissionsPolicy(builder =>
            {
                builder.AddAccelerometer().None();
                builder.AddAutoplay().None();
                builder.AddCamera().None();
                builder.AddEncryptedMedia().None();
                builder.AddFullscreen().All();
                builder.AddGeolocation().None();
                builder.AddGyroscope().None();
                builder.AddMagnetometer().None();
                builder.AddMicrophone().None();
                builder.AddMidi().None();
                builder.AddPayment().None();
                builder.AddPictureInPicture().None();
                builder.AddSyncXHR().None();
                builder.AddUsb().None();
            });

        if (!isDevelopmentEnvironment)
        {
            // MaxAge = one year in seconds
            policy.AddStrictTransportSecurityMaxAgeIncludeSubDomains();
        }

        policy.ApplyDocumentHeadersToAllResponses();

        return policy;
    }

    #endregion Public Methods
}