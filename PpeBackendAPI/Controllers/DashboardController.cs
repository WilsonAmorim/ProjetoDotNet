using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PpeBackendAPI.Models;
using PpeBackendAPI.Services;
using PpeBackendAPI.DTOs;

namespace PpeBackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly PpeDbContext _context;

    public DashboardController(PpeDbContext context)
    {
        _context = context;
    }

    // repasse

    [HttpGet("repasses")]
    public IActionResult Repasses()
    {
        var repassesListadosRaw = _context.Repasses
            .ToList();

        var repassesCadastrados = repassesListadosRaw
            .Select(t => new RepasseDTO
            {
                Id = t.Id,
                NomeLote = t.NomeLote ?? "",
                ValorRepasse = t.ValorRepasse ?? "",
                Periodo = t.Periodo ?? "",
                Valor = t.Valor,
                Status = t.Status ?? "",
                Delay = t.Delay ?? "",
                DataPagamento = t.DataPagamento,
                ValorInformado = t.ValorInformado
            })
            .ToList();

        return Ok(new { repassesCadastrados });
    }

    [Authorize("gestor, admin")]
    [HttpPost("criar-repasse")]
    public IActionResult CriarRepasse([FromBody] CriarRepasseDTO dto)
    {
        // var usuarioOrigemId = User.FindFirst("id")?.Value;

        var repasse = new Repasse
        {
            NomeLote = dto.NomeLote,
            ValorRepasse = dto.ValorRepasse,
            Periodo = dto.Periodo,
            Valor = dto.Valor,
            Status = dto.Status,
            Delay = dto.Delay,
            DataPagamento = dto.DataPagamento,
            ValorInformado = dto.ValorInformado
        };

        _context.Repasses.Add(repasse);
        _context.SaveChanges();

        return Ok("Repasse criado com sucesso");
    }

    // estagio
    [HttpGet("estagios-listar")]
    public IActionResult Estagios()
    {
        var estagiosListadosRaw = _context.Estagios
            .ToList();

        var estagiosCadastrados = estagiosListadosRaw
            .Select(t => new EstagioDTO
            {
                Id = t.Id,
                DescricaoEstagio = t.DescricaoEstagio ?? "",
                Valor = t.Valor ?? 0,
                Periodo = t.Periodo ?? ""

            })
            .ToList();

        return Ok(new { estagiosCadastrados });
    }

    [Authorize("gestor, admin")]
    [HttpPost("criar-estagio")]
    public IActionResult CriarTarefa([FromBody] CriarEstagioDTO dto)
    {
        // var usuarioOrigemId = User.FindFirst("id")?.Value;

        var estagio = new Estagio
        {
            DescricaoEstagio = dto.DescricaoEstagio,
            Valor = dto.Valor,
            Periodo = dto.Periodo
        };

        _context.Estagios.Add(estagio);
        _context.SaveChanges();

        return Ok("Estágio criado com sucesso");
    }

    // investimento
    [HttpGet("investimentos-listar")]
    public IActionResult Investimentos()
    {
        var investimentosListadosRaw = _context.Investimentos
            .ToList();

        var investimentosCadastrados = investimentosListadosRaw
            .Select(t => new InvestimentoDTO
            {
                Id = t.Id,
                DescricaoInvestimento = t.DescricaoInvestimento ?? "",
                Valor = t.Valor ?? 0,
                Periodo = t.Periodo ?? ""

            })
            .ToList();

        return Ok(new { investimentosCadastrados });
    }

    [Authorize("gestor, admin")]
    [HttpPost("criar-investimento")]
    public IActionResult CriarTarefa([FromBody] CriarInvestimentoDTO dto)
    {
        // var usuarioOrigemId = User.FindFirst("id")?.Value;

        var investimento = new Investimento
        {
            DescricaoInvestimento = dto.DescricaoInvestimento,
            Valor = dto.Valor,
            Periodo = dto.Periodo
        };

        _context.Investimentos.Add(investimento);
        _context.SaveChanges();

        return Ok("Investimento criado com sucesso");

    }
}
