using Server.Helpers;

var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;
IConfigurationSection openIDConnectSettings = builder.Configuration.GetSection("OpenIDConnectSettings");

services.AddAntiforgery(options => options.Configure());
services.AddHttpClient();
services.AddOptions();
services.AddAuthentication(options => options.Configure())
        .AddCookie()
        .AddOpenIdConnect(options => options.Configure(
            configuration["OpenIDConnectConfiguration:Authority"]!,
            configuration["OpenIDConnectConfiguration:ClientID"]!,
            configuration["OpenIDConnectConfiguration:ClientSecret"]!));

services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
services.AddRazorPages()
        .AddMvcOptions(options => options.Configure());

var app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseSecurityHeaders(SecurityHeadersHelper.GetHeaderPolicyCollection(
    environment.IsDevelopment(),
    configuration["OpenIDConnectConfiguration:Authority"]!));

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseNoUnauthorizedRedirect("/api");
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
app.MapNotFound("/api/{**segment}");
app.MapUnauthorized("/api/{**segment}");
app.MapFallbackToPage("/_Host");
app.Run();