
namespace PpeBackendAPI.DTOs
{
    public class LoginDTO
    {
        public string? Email { get; set; }
        public string? Senha { get; set; }
    }

    public class UsuarioDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Role { get; set; } = "usuario";

    }
}
