using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace PpeFrontend.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;

        public JwtAuthenticationStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            if (string.IsNullOrWhiteSpace(token) || TokenExpirado(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }


        public void NotifyUserAuthentication(string token)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    var key = kvp.Key switch
                    {
                        "role" => ClaimTypes.Role,
                        "name" => ClaimTypes.Name,
                        "unique_name" => ClaimTypes.Name,
                        _ => kvp.Key
                    };

                    if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in element.EnumerateArray())
                        {
                            claims.Add(new Claim(key, item.ToString()));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(key, kvp.Value?.ToString() ?? string.Empty));
                    }
                }
            }

            return claims;
        }

        private bool TokenExpirado(string token)
        {
            try
            {
                var payload = token.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                if (keyValuePairs != null && keyValuePairs.TryGetValue("exp", out var expValue))
                {
                    var exp = Convert.ToInt64(expValue);
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(exp);

                    return expDate < DateTimeOffset.UtcNow;
                }
            }
            catch
            {
                return true; // Se falhar, trata como expirado
            }

            return false;
        }



        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }

}
