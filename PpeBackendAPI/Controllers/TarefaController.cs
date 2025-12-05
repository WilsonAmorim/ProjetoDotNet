using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PpeBackendAPI.Models;
using PpeBackendAPI.Services;
using PpeBackendAPI.DTOs;

namespace PpeBackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarefaController : ControllerBase
{
    private readonly PpeDbContext _context;

    public TarefaController(PpeDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var tarefas = _context.Tarefas.ToList();
        return Ok(tarefas);
    }

    [HttpPost]
    public IActionResult Criar([FromBody] Tarefa tarefa)
    {
        _context.Tarefas.Add(tarefa);
        _context.SaveChanges();
        return Ok(tarefa);
    }

    [Authorize(Roles = "usuario, gestor")]
    [HttpPost("criar")]
    public IActionResult CriarTarefa([FromBody] CriarTarefaDTO dto)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [Authorize(Roles = "usuario, gestor")]
    [HttpGet("minhas-tarefas")]
    public IActionResult MinhasTarefas()
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [Authorize(Roles = "usuario, gestor")]
    [HttpGet("conferencia-tarefas")]
    public IActionResult ConferenciaTarefas()
    {
        try
        {
            var meuId = User.FindFirst("id")?.Value;
            Console.WriteLine("Meu ID: " + meuId);

            var tarefasRecebidasRaw = _context.Tarefas
                .Where(t => t.usuarioDestino == meuId && t.status != "Concluido")
                .ToList();

            Console.WriteLine("Tarefas recebidas: " + tarefasRecebidasRaw.Count());

            var recebidas = tarefasRecebidasRaw
                .Select(t => new TarefaConferenciaDTO
                {
                    Id = t.Id,
                })
                .ToList();

            return Ok(recebidas);

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [Authorize]
    [HttpPut("concluir/{id}")]
    public IActionResult ConcluirTarefa(int id)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }


    [Authorize]
    [HttpPut("priorizar/{id}")]
    public IActionResult PriorizarTarefa(int id)
    {
        try
        {
            var meuId = User.FindFirst("id")?.Value;
            var tarefa = _context.Tarefas.FirstOrDefault(t => t.Id == id && t.usuarioDestino == meuId);

            if (tarefa == null)
                return NotFound("Tarefa não encontrada ou não pertence a você");

            tarefa.status = "Prioritario";
            _context.SaveChanges();

            return Ok("Tarefa marcada como prioritária");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [Authorize(Roles = "usuario, admin, gestor")]
    [HttpGet("usuarios-ativos")]
    public IActionResult UsuariosComRoleUsuario()
    {
        try
        {
            var usuarios = _context.Usuarios
                .Where(u => u.Role == "usuario" || u.Role == "gestor")
                .Select(u => new
                {
                    Id = u.Id.ToString(),
                    Nome = u.Nome,
                    Email = u.Email
                })
                .ToList();

            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [Authorize(Roles = "usuario, gestor")]
    [HttpPut("editar/{id}")]
    public async Task<IActionResult> EditarTarefa(int id, [FromBody] EditarTarefaDTO dto)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpPost("tarefas/{id}/anexos")]
    // Alteração aqui: de Guid para int
    public async Task<IActionResult> UploadAnexo(int id, IFormFile arquivo)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpGet("tarefas/{id}/anexos")]
    public IActionResult ListarAnexos(int id)
    {
        try
        {
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", id.ToString());
            if (!Directory.Exists(caminho))
                return Ok(new List<string>());

            var arquivos = Directory.GetFiles(caminho).Select(Path.GetFileName).ToList();
            return Ok(arquivos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpGet("tarefas/{id}/anexos/{nome}")]
    public IActionResult BaixarAnexo(int id, string nome)
    {
        try
        {
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", id.ToString(), nome);
            if (!System.IO.File.Exists(caminho))
                return NotFound("Arquivo não encontrado");

            var mime = "application/octet-stream";
            return PhysicalFile(caminho, mime, nome);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }


    [HttpDelete("tarefas/{id}/anexos/{nome}")]
    public IActionResult ExcluirAnexo(int id, string nome)
    {
        try
        {
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", id.ToString(), nome);
            if (!System.IO.File.Exists(caminho))
                return NotFound("Arquivo não encontrado");

            System.IO.File.Delete(caminho);
            return Ok("Arquivo excluído com sucesso");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [Authorize(Roles = "admin, gestor")]
    [HttpGet("tarefas-executadas")]
    public IActionResult TarefasExecutadas()
    {
        try
        {
            var meuId = User.FindFirst("id")?.Value;

            var tarefasEnviadasRaw = _context.Tarefas
                .Where(t => t.usuarioOrigem == meuId)
                .ToList();


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
                .OrderByDescending(t => t.dataExecucao)
                .ToList();


            return Ok(new { enviadas });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [Authorize(Roles = "admin, gestor")]
    [HttpGet("sem-tarefas")]
    public IActionResult SemTarefas()
    {
        try
        {
            var meuId = User.FindFirst("id")?.Value;

            // Tarefas enviadas pelo usuário
            var tarefasEnviadasRaw = _context.Tarefas
                .Where(t => t.status != "Concluido" && t.usuarioOrigem == meuId)
                .ToList();


            var usuariosComRoleUsuario = _context.Usuarios
                .Where(u => u.Role == "usuario")
                .ToList();


            // Mapeia tarefas existentes
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

            // Adiciona usuários sem tarefas
            var usuariosSemTarefas = usuariosComRoleUsuario
                .Where(u => !enviadas.Any(e => e.usuarioDestino == u.Id.ToString()))
                .Select(u => new SemTarefaDTO
                {
                    usuarioDestinoNome = u.Nome ?? "",
                    descricao = "Sem Atividade",
                });

            return Ok(usuariosSemTarefas);

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

}
