using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SupportHub.Client;
using SupportHub.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using SupportHub.Client.Auth;
using SupportHub.Application.DTOs.Auth; 


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5070";

builder.Services.AddScoped<BrowserStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddTransient<JwtAuthorizationHandler>();

builder.Services.AddHttpClient("SupportHub.API", client => 
client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<JwtAuthorizationHandler>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

builder.Services.AddScoped<TicketClientService>();
// Varsayılan HttpClient olarak kaydet
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("SupportHub.API"));

await builder.Build().RunAsync();
