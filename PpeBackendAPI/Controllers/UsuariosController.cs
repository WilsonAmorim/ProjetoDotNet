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
        private readonly MeuDbContext _context;

        public UsuariosController(MeuDbContext context)
        {
            _context = context;
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
    }
}
