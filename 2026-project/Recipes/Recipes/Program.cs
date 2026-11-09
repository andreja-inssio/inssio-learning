using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Recipes;
using Recipes.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl is required in appsettings.json.");
if (builder.HostEnvironment.IsDevelopment()
    && new Uri(builder.HostEnvironment.BaseAddress).Scheme == Uri.UriSchemeHttps)
{
    apiBaseUrl = builder.Configuration["ApiHttpsBaseUrl"]
        ?? throw new InvalidOperationException("ApiHttpsBaseUrl is required for HTTPS development.");
}

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<RecipeApiClient>();

await builder.Build().RunAsync();
