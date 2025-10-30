
namespace PpeBackendAPI.DTOs
{
    public class LoginDTO
    {
        public string? Login { get; set; }
        public string? Senha { get; set; }
    }

    public class UsuarioDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Role { get; set; } = "usuario";

    }

    public class TrocarSenha
    {
        public string SenhaAtual { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;

    }
}
