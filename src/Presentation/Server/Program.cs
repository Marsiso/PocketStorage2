using Server.Helpers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;
IConfigurationSection openIDConnectSettings = builder.Configuration.GetSection("OpenIdConnectSettings");

services
    .AddApplicationAntiforgery()
    .AddHttpClient()
    .AddOptions()
    .AddApplicationAuthentication()
    .AddCookie()
    .AddApplicationOpenIdConnect(builder.Configuration);

services
    .AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

services
    .AddRazorPages()
    .AddMvcOptions(options => options.Configure());

WebApplication app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseSecurityHeaders(SecurityHeadersHelper.GetHeaderPolicyCollection(app));
app
    .UseHttpsRedirection()
    .UseBlazorFrameworkFiles()
    .UseStaticFiles()
    .UseRouting()
    .UseNoUnauthorizedRedirect("/api")
    .UseAuthentication()
    .UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app
    .MapNotFound("/api/{**segment}")
    .MapFallbackToPage("/_Host");

app.Run();