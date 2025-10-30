namespace PpeBackendAPI.DTOs
{
    public class PerfilDTO
    {
        public string Nome { get; set; } = "";
        public string Login { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string RefreshExpiraEm { get; set; } = "";
    }

    public class TrocarSenhaDTO
    {
        public string SenhaAtual { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
    }
}