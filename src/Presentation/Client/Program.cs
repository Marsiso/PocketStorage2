using Syncfusion.Blazor;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(ApplicationConstants.Syncfussion.LicenseKey);

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
IServiceCollection services = builder.Services;

services
    .AddOptions()
    .AddAuthorizationCore();

services.TryAddSingleton<AuthenticationStateProvider, HostAuthenticationStateProvider>();
services.TryAddSingleton(sp => (HostAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
services.AddTransient<AuthorizedHandler>();

builder.RootComponents.Add<App>("#app");

services.AddHttpClient("default", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

services.AddHttpClient(AuthorizationDefaults.AuthorizedClientName, client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddHttpMessageHandler<AuthorizedHandler>();

services.AddTransient(serviceProvider => serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("default"));
services.AddTransient<IAntiforgeryHttpClientFactory, AntiforgeryHttpClientFactory>();

services.AddSyncfusionBlazor();

await builder.Build().RunAsync();