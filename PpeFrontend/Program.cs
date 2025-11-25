using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http;
using PpeFrontend;
using PpeFrontend.Services;
using Microsoft.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 🔐 Autorização para Blazor WebAssembly
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

// 🛡️ Handler para adicionar o token JWT nas requisições
builder.Services.AddScoped<TokenAuthorizationHandler>();
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
                 ?? throw new InvalidOperationException("ApiBaseUrl não configurada");
// 🌐 HttpClient com autenticação (usado para chamadas protegidas)
builder.Services.AddHttpClient("Autenticado", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<TokenAuthorizationHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Autenticado"));


// 🚀 Inicia a aplicação
await builder.Build().RunAsync();
