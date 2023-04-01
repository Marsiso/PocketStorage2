using Domain.Data;
using IdentityServer.Data.Seed;
using IdentityServer.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IConfiguration>(builder.Configuration)
    .AddAplicationDatabaseContext(builder)
    .AddDatabaseDeveloperPageExceptionFilter()
    .AddApplicationIdentity();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(1));
builder.Services.Configure<IdentityOptions>(options => options.ConfigureApplicationIdentity());
builder.Services.ConfigureApplicationCookie(options => options.ConfigureApplicationCookie());

builder.Services
    .AddApplicationOpenIddict()
    .AddHostedService<Worker>()
    .AddHostedService<UserSeed>()
    .AddAuthentication()
    .AddExternalProviders(builder.Configuration);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();