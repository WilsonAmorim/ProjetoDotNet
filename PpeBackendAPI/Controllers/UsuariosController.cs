using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PpeBackendAPI.Models;
using PpeBackendAPI.DTOs;
using PpeBackendAPI.Services;


namespace PpeBackendAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly PpeDbContext _context;

        public UsuariosController(PpeDbContext context)
        {
            _context = context;
        }

        [Authorize("admin")]
        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] UsuarioDTO dto)
        {
            if (_context.Usuarios.Any(u => u.Email == dto.Email))
                return BadRequest("E-mail já cadastrado");

            if (_context.Usuarios.Any(u => u.Login == dto.Login))
                return BadRequest("Login já cadastrado");

            if (_context.Usuarios.Any(u => u.Nome == dto.Nome))
                return BadRequest("Nome já cadastrado");

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Login = dto.Login,
                Email = dto.Email,
                SenhaHash = SenhaHelper.GerarHash(dto.Senha),
                Role = dto.Role
            };


            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return Ok("Usuário registrado com sucesso");
        }

        [Authorize(Roles = "usuario")]
        [HttpPost("trocar-senha")]
        public IActionResult TrocarSenha([FromBody] TrocarSenhaDTO dto)
        {
            var login = User.Identity?.Name;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Login == login);
            Console.WriteLine($"🔐 Usuário logado: {login}");

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            if (string.IsNullOrEmpty(usuario.SenhaHash) || !SenhaHelper.VerificarSenha(dto.SenhaAtual, usuario.SenhaHash))
                return BadRequest("Senha atual incorreta");


            usuario.SenhaHash = SenhaHelper.GerarHash(dto.NovaSenha);
            _context.SaveChanges();

            return Ok("Senha alterada com sucesso");
        }
    }
}
