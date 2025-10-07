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
                new Claim(ClaimTypes.Name, usuario.Nome ?? "usuario"),
                new Claim("name", usuario.Nome ?? "usuario")
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

        // Tarefas

        [Authorize("usuario")]
        [HttpPost("criar")]
        public IActionResult CriarTarefa([FromBody] CriarTarefaDTO dto)
        {
            var usuarioOrigemId = User.FindFirst("id")?.Value;

            var tarefa = new Tarefa
            {
                descricao = dto.descricao,
                usuarioDestino = dto.usuarioDestino,
                usuarioOrigem = usuarioOrigemId,
                dataCriacao = DateTime.UtcNow,
                status = string.IsNullOrWhiteSpace(dto.status) ? "Nova" : dto.status,
                observacao = dto.observacao
            };

            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();

            return Ok("Tarefa criada com sucesso");
        }

        [Authorize]
        [HttpGet("minhas-tarefas")]
        public IActionResult MinhasTarefas()
        {
            var meuId = User.FindFirst("id")?.Value;

            var tarefasRecebidasRaw = _context.Tarefas
                .Where(t => t.usuarioDestino == meuId)
                .ToList();


            var recebidas = tarefasRecebidasRaw
                .Select(t => new TarefaDTO
                {
                    Id = t.Id,
                    descricao = t.descricao ?? "",
                    status = t.status ?? "",
                    dataExecucao = t.dataExecucao,
                    observacao = t.observacao ?? "",
                    usuarioOrigem = t.usuarioOrigem ?? "",
                    usuarioOrigemNome = _context.Usuarios
                        .FirstOrDefault(u => u.Id.ToString() == t.usuarioOrigem)?.Nome ?? ""
                })
                .ToList();


            var tarefasEnviadasRaw = _context.Tarefas
                 .Where(t => t.usuarioOrigem == meuId)
                 .ToList(); // Executa no banco

            var enviadas = tarefasEnviadasRaw
                .Select(t => new TarefaDTO
                {
                    Id = t.Id,
                    usuarioDestino = t.usuarioDestino ?? "",
                    usuarioDestinoNome = _context.Usuarios
                        .FirstOrDefault(u => u.Id.ToString() == t.usuarioDestino)?.Nome ?? "",
                    descricao = t.descricao ?? "",
                    observacao = t.observacao ?? "",
                    status = t.status ?? "",
                    dataExecucao = t.dataExecucao
                })
                .ToList();


            return Ok(new { recebidas, enviadas });
        }

        [Authorize]
        [HttpPut("concluir/{id}")]
        public IActionResult ConcluirTarefa(int id)
        {
            var meuId = User.FindFirst("id")?.Value;
            var tarefa = _context.Tarefas.FirstOrDefault(t => t.Id == id && t.usuarioDestino == meuId);

            if (tarefa == null)
                return NotFound("Tarefa não encontrada ou não pertence a você");

            tarefa.status = "Concluido";
            tarefa.dataExecucao = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok("Tarefa concluída");
        }


        [Authorize]
        [HttpPut("priorizar/{id}")]
        public IActionResult PriorizarTarefa(int id)
        {
            var meuId = User.FindFirst("id")?.Value;
            var tarefa = _context.Tarefas.FirstOrDefault(t => t.Id == id && t.usuarioDestino == meuId);

            if (tarefa == null)
                return NotFound("Tarefa não encontrada ou não pertence a você");

            tarefa.status = "Prioritario";
            _context.SaveChanges();

            return Ok("Tarefa marcada como prioritária");
        }

        [Authorize(Roles = "usuario")]
        [HttpGet("usuarios-ativos")]
        public IActionResult UsuariosComRoleUsuario()
        {
            var usuarios = _context.Usuarios
                .Where(u => u.Role == "usuario")
                .Select(u => new
                {
                    Id = u.Id.ToString(),
                    Nome = u.Nome,
                    Email = u.Email
                })
                .ToList();

            return Ok(usuarios);
        }

        [Authorize("usuario")]
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarTarefa(int id, [FromBody] EditarTarefaDTO dto)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa == null)
                return NotFound();

            var dataAtual = DateTime.UtcNow;

            var statusAnterior = tarefa.status?.Trim().ToLowerInvariant() ?? "";
            var statusNovo = dto.status?.Trim().ToLowerInvariant() ?? "";

            var statusMudou = statusAnterior != statusNovo;


            var temNovaObservacao = !string.IsNullOrWhiteSpace(dto.observacao);

            if (!statusMudou && !temNovaObservacao)
                return NoContent(); // Nada a fazer

            var novasEntradas = new List<string>();

            // Processa mudança de status
            if (statusMudou)
            {
                tarefa.status = dto.status;
                tarefa.dataExecucao = dataAtual;

                var anotacaoStatus = $"Data: {dataAtual:dd/MM/yyyy} Status mudado para {dto.status} ";

                // Evita duplicata exata de anotação de status
                if (string.IsNullOrWhiteSpace(tarefa.observacao) || !tarefa.observacao.Contains(anotacaoStatus))
                {
                    novasEntradas.Add(anotacaoStatus);
                }
            }

            // Processa nova observação
            if (temNovaObservacao)
            {
                var anotacaoObservacao = $"Data: {dataAtual:dd/MM/yyyy}: \n {dto.observacao.Trim()}";

                // Evita duplicação exata da observação
                if (string.IsNullOrWhiteSpace(tarefa.observacao) || !tarefa.observacao.Contains(anotacaoObservacao))
                {
                    novasEntradas.Add(anotacaoObservacao);
                }
            }

            // Adiciona novas entradas no topo
            if (novasEntradas.Any())
            {
                var novaAnotacao = string.Join("\n", novasEntradas);

                tarefa.observacao = string.IsNullOrWhiteSpace(tarefa.observacao)
                    ? novaAnotacao
                    : $"{novaAnotacao}\n{tarefa.observacao}";

                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        [HttpPost("tarefas/{id}/anexos")]
        // Alteração aqui: de Guid para int
        public async Task<IActionResult> UploadAnexo(int id, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Arquivo inválido");

            var root = Directory.GetCurrentDirectory();
            // Usa o ID como string para o nome da pasta
            var caminho = Path.Combine(root, "Uploads", id.ToString());

            try
            {
                Directory.CreateDirectory(caminho);
                var caminhoCompleto = Path.Combine(caminho, arquivo.FileName);

                using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                await arquivo.CopyToAsync(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao salvar arquivo: " + ex.Message);
                return StatusCode(500, "Erro interno ao salvar o arquivo");
            }

            return Ok("Arquivo salvo com sucesso");
        }

        [HttpGet("tarefas/{id}/anexos")]
        public IActionResult ListarAnexos(int id)
        {
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", id.ToString());
            if (!Directory.Exists(caminho))
                return Ok(new List<string>());

            var arquivos = Directory.GetFiles(caminho).Select(Path.GetFileName).ToList();
            return Ok(arquivos);
        }

        [HttpGet("tarefas/{id}/anexos/{nome}")]
        public IActionResult BaixarAnexo(int id, string nome)
        {
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", id.ToString(), nome);
            if (!System.IO.File.Exists(caminho))
                return NotFound("Arquivo não encontrado");

            var mime = "application/octet-stream";
            return PhysicalFile(caminho, mime, nome);
        }


        [HttpDelete("tarefas/{id}/anexos/{nome}")]
        public IActionResult ExcluirAnexo(int id, string nome)
        {
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", id.ToString(), nome);
            if (!System.IO.File.Exists(caminho))
                return NotFound("Arquivo não encontrado");

            System.IO.File.Delete(caminho);
            return Ok("Arquivo excluído com sucesso");
        }
        // convenios
        [Authorize]
        [HttpGet("convenios")]
        public IActionResult Convenios()
        {

            var conveniosListadosRaw = _context.Convenios
                .ToList();


            var conveiosCadastrados = conveniosListadosRaw
                .Select(t => new ConvenioDTO
                {
                    Id = t.Id,
                    ConvenioNome = t.ConvenioNome ?? "",
                    Cpf = t.Cpf ?? "",
                    Matricula = t.Matricula ?? "",
                    Nome = t.Nome ?? "",
                    Situacao = t.Situacao ?? "",
                    Categoria = t.Categoria ?? "",
                    DataAdmissao = t.DataAdmissao,
                    DataDemissao = t.DataDemissao,
                    Sexo = t.Sexo,
                    Funcao = t.Funcao ?? "",
                    DataAtualizacao = t.DataAtualizacao,
                    PostoTrabalho = t.PostoTrabalho ?? "",
                    MunicipioLotacao = t.MunicipioLotacao ?? "",
                    Usuario = t.Usuario
                })
                .ToList();

            return Ok(new { conveiosCadastrados });
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
