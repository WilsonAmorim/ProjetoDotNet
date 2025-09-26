namespace PpeBackendAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? SenhaHash { get; set; }
        public string Role { get; set; } = "usuario";
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiracao { get; set; }

    }



}


