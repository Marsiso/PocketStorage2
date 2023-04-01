using Domain.Data;
using Domain.Identity.Entities;
using IdentityServer.Data.Seed;
using IdentityServer.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddDbContext<ApplicationDatabaseContext>(options => options.Configure(builder, connectionString, nameof(IdentityServer)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

#region AspNetIdentity

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDatabaseContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(1));
builder.Services.Configure<IdentityOptions>(options => options.Configure());
builder.Services.ConfigureApplicationCookie(options => options.Configure());

#endregion AspNetIdentity

#region OpenId

builder.Services.AddOpenIddict()
    .AddCore(options => options.UseEntityFrameworkCore().UseDbContext<ApplicationDatabaseContext>())
    .AddServer(options => options.Configure())
    // Register the OpenIddict validation components
    .AddValidation(options =>
    {
        // Import the configuration from the local OpenIddict server instance
        options.UseLocalServer();

        // Register the ASP.NET Core host
        options.UseAspNetCore();
    });

// Register the worker responsible of seeding the database with the sample clients Note in a real
// world application, this step should be part of a setup script
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<UserSeed>();

#endregion OpenId

builder.Services.AddAuthentication()
    // For further details about Google external login setup in ASP.NET Core see https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-7.0
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Clients:Google:Id"]!;
        options.ClientSecret = builder.Configuration["Clients:Google:Secret"]!;
    })
    .AddFacebook(options =>
    {
        options.ClientId = builder.Configuration["Clients:Facebook:Id"]!;
        options.ClientSecret = builder.Configuration["Clients:Facebook:Secret"]!;
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = builder.Configuration["Clients:MicrosoftAccount:Id"]!;
        options.ClientSecret = builder.Configuration["Clients:MicrosoftAccount:Secret"]!;
    });

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