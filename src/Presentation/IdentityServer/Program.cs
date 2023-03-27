using Domain.Data;
using Domain.Identity.Entities;
using IdentityServer.Data.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDatabaseContext>(options =>
{
    options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(nameof(IdentityServer)));
    options.UseOpenIddict();
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableServiceProviderCaching();
    options.UseLoggerFactory(ApplicationDatabaseContext.PropertyAppLoggerFactory);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDatabaseContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

#region AspNetIdentity

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(1));
builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = Claims.Role;
    options.ClaimsIdentity.EmailClaimType = Claims.Email;
    options.SignIn.RequireConfirmedAccount = false;

    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

#endregion AspNetIdentity

#region OpenId

builder.Services.AddOpenIddict()
    .AddCore(options => options.UseEntityFrameworkCore().UseDbContext<ApplicationDatabaseContext>())
    .AddServer(options =>
    {
        // Enable the authorization, logout, token and user info endpoints
        options.SetAuthorizationEndpointUris("/connect/authorize");
        options.SetLogoutEndpointUris("/connect/logout");
        options.SetTokenEndpointUris("/connect/token");
        options.SetUserinfoEndpointUris("/connect/userinfo");
        options.SetIntrospectionEndpointUris("/connect/introspect");
        options.SetVerificationEndpointUris("/connect/verify");

        // Mark the "email", "profile" and "roles" scopes as supported scopes
        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

        // Enable the client credentials flow
        options.AllowClientCredentialsFlow();
        options.AllowAuthorizationCodeFlow();
        options.AllowRefreshTokenFlow();
        options.RequireProofKeyForCodeExchange();

        // Register the signing and encryption credentials
        options.AddDevelopmentEncryptionCertificate();
        options.AddDevelopmentSigningCertificate();

        // Register the ASP.NET Core host and configure the ASP.NET Core options
        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableLogoutEndpointPassthrough()
               .EnableTokenEndpointPassthrough()
               .EnableUserinfoEndpointPassthrough()
               .EnableStatusCodePagesIntegration();
    })
    // Register the OpenIddict validation components
    .AddValidation(options =>
    {
        // Import the configuration from the local OpenIddict server instance
        options.UseLocalServer();

        // Register the ASP.NET Core host
        options.UseAspNetCore();
    });

// Register the worker responsible of seeding the database with the sample clients
// Note in a real world application, this step should be part of a setup script
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<UserSeed>();
#endregion OpenId

builder.Services.AddAuthentication();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDatabaseContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();