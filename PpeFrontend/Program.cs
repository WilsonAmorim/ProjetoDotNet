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

// 🌐 HttpClient com autenticação (usado para chamadas protegidas)
builder.Services.AddHttpClient("Autenticado", client =>
{
    client.BaseAddress = new Uri("http://localhost:5271/");
})
.AddHttpMessageHandler<TokenAuthorizationHandler>();

// // 🌐 HttpClient padrão (sem autenticação, usado por componentes públicos)
// builder.Services.AddScoped(sp => new HttpClient
// {
//     BaseAddress = new Uri("http://localhost:5271/")
// });

// 🚀 Inicia a aplicação
await builder.Build().RunAsync();
