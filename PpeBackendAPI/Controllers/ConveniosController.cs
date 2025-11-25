using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PpeBackendAPI.Models;
using PpeBackendAPI.Services;
using PpeBackendAPI.DTOs;

namespace PpeBackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConveniosController : ControllerBase
{
    private readonly PpeDbContext _context;

    public ConveniosController(PpeDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "admin")]
    [HttpPost("importar-inicial")]
    public async Task<IActionResult> ImportarInicial(IFormFile arquivo)
    {


        if (arquivo == null || arquivo.Length == 0)
        {
            Console.WriteLine("❌ Arquivo nulo ou vazio");
            return BadRequest("Arquivo inválido");
        }

        Console.WriteLine($"📄 Arquivo recebido: {arquivo.FileName}, tamanho: {arquivo.Length}");

        var usuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "desconhecido";
        Console.WriteLine($"👤 Usuário logado: {usuario}");
        Console.WriteLine($"🔐 Autenticado: {User.Identity?.IsAuthenticated}");
        Console.WriteLine($"🎯 Tem role admin: {User.IsInRole("admin")}");

        try
        {
            using var stream = arquivo.OpenReadStream();
            using var reader = new StreamReader(stream);

            Console.WriteLine("📥 Chamando ImportarConvenioDoUpload...");
            await ConvenioImportService.ImportarConvenioDoUpload(_context, reader, arquivo.FileName, usuario);
            Console.WriteLine("✅ Importação finalizada com sucesso");

            return Ok("Importação concluída");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔥 Erro durante importação: {ex.Message}");
            return StatusCode(500, "Erro interno ao importar");
        }
    }


    // [Authorize("usuario")]
    [HttpGet]
    public IActionResult Listar()
    {
        var convenios = _context.Convenios
            .Select(c => new
            {
                c.ConvenioNome,
                c.Cpf,
                c.Matricula,
                c.Nome,
                c.DataAdmissao,
                c.DataDemissao,
                c.Situacao,
                c.Categoria,
                c.Funcao,
                c.PostoTrabalho,
                c.MunicipioLotacao
            })
            .ToList();

        return Ok(convenios);
    }

    // convenios
    [Authorize(Roles = "usuario, gestor")]
    [HttpGet("convenios")]
    public IActionResult Convenios()
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        Console.WriteLine("📡 Backend recebeu requisição de ping");
        return Ok("pong");
    }

    [Authorize]
    [HttpGet("verificar-token")]
    public IActionResult VerificarToken()
    {
        var usuario = User.Identity?.Name ?? "desconhecido";
        var roleAdmin = User.IsInRole("admin");
        Console.WriteLine($"🔐 Token recebido. Usuário: {usuario}, Admin: {roleAdmin}");
        return Ok($"Usuário: {usuario}, Admin: {roleAdmin}");
    }

    [Authorize(Roles = "usuario, gestor")]
    [HttpPost("pesquisar")]
    public IActionResult PesquisarConvenios([FromBody] PesquisaConvenioDto filtro)
    {
        Console.WriteLine($"🔐 Filtro recebido. Usuário: {filtro.Convenio}");
        try
        {
            if (filtro.Convenio == "Fesfsus Lote 01")
            {
                var registros = _context.Convenios
            .Where(c =>
                (filtro.Convenio == "" || c.ConvenioNome == filtro.Convenio) &&
                (filtro.Situacao == "" || c.Situacao == filtro.Situacao) &&
                c.DataAdmissao.HasValue &&
                c.DataAdmissao.Value.Month == filtro.Mes &&
                c.DataAdmissao.Value.Year == filtro.Ano)

            .Select(c => new ConvenioRegistroDto
            {
                Id = c.Id,
                Cpf = c.Cpf ?? "",
                Matricula = c.Matricula ?? "",
                Nome = c.Nome ?? "",
                DataAdmissao = c.DataAdmissao,
                DataDemissao = c.DataDemissao,
                Sexo = c.Sexo ?? "",
                Situacao = c.Situacao ?? "",
                Categoria = c.Categoria ?? "",
                Funcao = c.Funcao ?? "",
            })
            .ToList();
                return Ok(registros);
            }
            else
            {
                var registros = _context.Convenios
            .Where(c =>
                (filtro.Convenio == "" || c.ConvenioNome == filtro.Convenio) &&
                (filtro.Categoria == "" || c.Categoria == filtro.Categoria) &&
                c.DataAdmissao.HasValue &&
                c.DataAdmissao.Value.Month == filtro.Mes &&
                c.DataAdmissao.Value.Year == filtro.Ano)

            .Select(c => new ConvenioRegistroDto
            {
                Id = c.Id,
                Cpf = c.Cpf ?? "",
                Matricula = c.Matricula ?? "",
                Nome = c.Nome ?? "",
                DataAdmissao = c.DataAdmissao,
                DataDemissao = c.DataDemissao,
                Sexo = c.Sexo ?? "",
                Situacao = c.Situacao ?? "",
                Categoria = c.Categoria ?? "",
                Funcao = c.Funcao ?? "",
            })
            .ToList();
                return Ok(registros);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }



    }




}
