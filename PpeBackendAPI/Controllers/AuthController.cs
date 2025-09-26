using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PpeBackendAPI.Models;
using PpeBackendAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using PpeBackendAPI.Services;
using System.Security.Cryptography;




namespace PpeBackendAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MeuDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(MeuDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == dto.Email);

            // ✅ Verificação extra para evitar warnings CS8604
            if (string.IsNullOrEmpty(dto.Senha) || string.IsNullOrEmpty(usuario?.SenhaHash))
                return Unauthorized("Credenciais inválidas");

            if (!SenhaHelper.VerificarSenha(dto.Senha, usuario.SenhaHash))
                return Unauthorized("Credenciais inválidas");

            var token = GerarToken(usuario);
            var refreshToken = GerarRefreshToken();

            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiracao = DateTime.UtcNow.AddDays(7);

            _context.Usuarios.Update(usuario);
            _context.SaveChanges();

            return Ok(new { token, refreshToken });
        }


        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] UsuarioDTO dto)
        {
            if (_context.Usuarios.Any(u => u.Email == dto.Email))
                return BadRequest("E-mail já cadastrado");

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = SenhaHelper.GerarHash(dto.Senha),
                Role = dto.Role
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return Ok("Usuário registrado com sucesso");
        }

        [Authorize]
        [HttpGet("produtos")]
        public IActionResult GetProdutos()
        {
            return Ok(new[] {
                new { Id = 1, Nome = "Notebook", Preco = 3500 },
                new { Id = 2, Nome = "Mouse", Preco = 150 }
            });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin-area")]
        public IActionResult GetAdminArea()
        {
            return Ok("Bem-vindo à área administrativa!");
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshDTO dto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.RefreshToken == dto.RefreshToken);

            if (usuario == null || usuario.RefreshTokenExpiracao < DateTime.UtcNow)
                return Unauthorized("Refresh token inválido ou expirado");

            var novoToken = GerarToken(usuario);
            var novoRefresh = GerarRefreshToken();

            usuario.RefreshToken = novoRefresh;
            usuario.RefreshTokenExpiracao = DateTime.UtcNow.AddDays(7);
            _context.SaveChanges();

            return Ok(new { token = novoToken, refreshToken = novoRefresh });
        }

        [HttpPost("corrigir-role")]
        public IActionResult CorrigirRole()
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == "admin@gmail.com");
            if (usuario == null) return NotFound();

            usuario.Role = "admin";
            _context.SaveChanges();

            return Ok("Role atualizada para admin");
        }

        [Authorize]
        [HttpGet("perfil")]
        public IActionResult Perfil()
        {
            var email = User.Identity?.Name;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            return Ok(new
            {
                usuario.Nome,
                usuario.Email,
                usuario.Role,
                RefreshExpiraEm = usuario.RefreshTokenExpiracao.ToString("dd/MM/yyyy HH:mm")
            });
        }



        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var email = User.Identity?.Name;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            usuario.RefreshToken = null;
            usuario.RefreshTokenExpiracao = DateTime.MinValue;

            _context.SaveChanges();

            return Ok("Logout realizado com sucesso");
        }
        
        private string GerarToken(Usuario usuario)
        {
            var agora = DateTime.UtcNow;

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, usuario.Email ?? ""),
                new Claim("id", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Role ?? "usuario"),
                new Claim(ClaimTypes.Name, usuario.Nome ?? "usuario")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role),
                Expires = agora.AddMinutes(15),
                NotBefore = agora,
                IssuedAt = agora,
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        private string GerarRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

    }
}
