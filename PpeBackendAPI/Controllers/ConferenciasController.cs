using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PpeBackendAPI.Models;
using PpeBackendAPI.Services;
using PpeBackendAPI.DTOs;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.IO.Image;
using Microsoft.AspNetCore.Hosting;


namespace PpeBackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConferenciasController : ControllerBase
{
    private readonly PpeDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ConferenciasController(PpeDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }



    [Authorize(Roles = "usuario")]
    [HttpGet("inconsistencias")]
    public IActionResult Listar()
    {
        var Conferencias = _context.Conferencias
            .Select(c => new
            {
                c.Convenio,
                c.Documento,
                c.Ocorrencia,
                c.Descricao,
                c.Status,
                c.DataRetorno,
                c.LinkOcorrencia,
                c.Usuario,
                c.DataAtualizacao

            })
            .ToList();

        return Ok(Conferencias);
    }

    // convenios
    [Authorize(Roles = "usuario")]
    [HttpGet("conferencia")]
    public IActionResult Conferencias()
    {

        var conferenciasListadosRaw = _context.Conferencias
            .ToList();


        var conferenciasCadastrados = conferenciasListadosRaw
            .Select(t => new ConferenciaDTO
            {
                Id = t.Id,
                ConvenioId = t.ConvenioId,
                DocumentoId = t.DocumentoId,
                OcorrenciaId = t.OcorrenciaId,
                RegistroOcorrenciasId = t.RegistroOcorrenciasId,
                Status = t.Status ?? "",
                DataRetorno = t.DataRetorno,
                LinkOcorrencia = t.LinkOcorrencia ?? "",
                Usuario = t.Usuario ?? "",
                DataAtualizacao = t.DataAtualizacao
            })
            .ToList();

        return Ok(new { conferenciasCadastrados });
    }

    // [Authorize(Roles = "usuario")]
    [HttpGet("documento/{documentoId}")]
    public async Task<ActionResult<IEnumerable<OcorrenciasDTO>>> GetOcorrenciasPorDocumento(int documentoId)
    {
        var ocorrencias = await _context.Ocorrencias
           .Where(t => t.DocumentoId == documentoId)
           .Select(t => new OcorrenciasDTO
           {
               Id = t.Id,
               Ocorrencia = t.Ocorrencia,
           })
           .ToListAsync();


        return Ok(ocorrencias);
    }

    [Authorize(Roles = "usuario")]
    [HttpGet("documentos")]
    public IActionResult ListarDocumentos()
    {
        var Documentos = _context.Documentos
            .Select(c => new
            {
                c.Id,
                c.Documento,
            })
            .ToList();

        return Ok(Documentos);
    }

    [Authorize(Roles = "usuario")]
    [HttpGet("registro-ocorrencias")]
    public IActionResult ListarRegistroOcorrencias()
    {
        var RegistroOcorrencias = _context.RegistroOcorrencias
            .Select(c => new
            {
                c.Id,
                c.Descricao,
            })
            .ToList();

        return Ok(RegistroOcorrencias);
    }

    [Authorize("usuario")]
    [HttpPost("registrar")]
    public IActionResult CriarTarefa([FromBody] ConferenciaDTO dto)
    {
        var usuarioOrigemId = User.FindFirst("id")?.Value;

        var conferencias = new Conferencias
        {
            ConvenioId = dto.ConvenioId,
            DocumentoId = dto.DocumentoId,
            OcorrenciaId = dto.OcorrenciaId,
            RegistroOcorrenciasId = dto.RegistroOcorrenciasId,
            Status = dto.Status,
            DataRetorno = dto.DataRetorno,
            LinkOcorrencia = dto.LinkOcorrencia,
            Usuario = usuarioOrigemId,
            DataAtualizacao = DateTime.UtcNow,
        };
        _context.Conferencias.Add(conferencias);
        _context.SaveChanges();

        return Ok("Registro de conferencia criada com sucesso");
    }

    [Authorize(Roles = "usuario")]
    [HttpPost("pesquisar-conferencia")]
    public IActionResult PesquisaConferencias([FromBody] ConferenciaRealizadaDTO filtro)
    {
        var registros = (from conferencia in _context.Conferencias
                         join convenio in _context.Convenios
                             on conferencia.ConvenioId equals convenio.Id
                         join documentos in _context.Documentos
                             on conferencia.DocumentoId equals documentos.Id
                         join ocorrencias in _context.Ocorrencias
                             on conferencia.OcorrenciaId equals ocorrencias.Id
                         join registroOcorrencias in _context.RegistroOcorrencias
                             on conferencia.RegistroOcorrenciasId equals registroOcorrencias.Id
                         where conferencia.ConvenioNome == filtro.ConvenioNome
                         select new ConferenciaRealizadaDTO
                         {
                             Id = conferencia.Id,
                             ConvenioId = conferencia.ConvenioId,
                             ConvenioNome = convenio.ConvenioNome,
                             Documento = documentos.Documento,
                             Ocorrencia = ocorrencias.Ocorrencia,
                             Descricao = registroOcorrencias.Descricao,
                             Status = conferencia.Status,
                             DataRetorno = conferencia.DataRetorno,
                             LinkOcorrencia = conferencia.LinkOcorrencia,
                             Usuario = conferencia.Usuario,
                             DataAtualizacao = conferencia.DataAtualizacao,

                             // Dados do convênio (via join)
                             Cpf = convenio.Cpf ?? "",
                             Matricula = convenio.Matricula ?? "",
                             Nome = convenio.Nome ?? "",
                             DataAdmissao = convenio.DataAdmissao,
                             DataDemissao = convenio.DataDemissao,
                             Situacao = convenio.Situacao ?? "",
                             Categoria = convenio.Categoria ?? "",
                             Funcao = convenio.Funcao ?? "",
                             Sexo = convenio.Sexo ?? ""
                         }).ToList();


        return Ok(registros);
    }

    [Authorize(Roles = "usuario")]
    [HttpPost("relatorio-conferencia")]
    public IActionResult GerarRelatorioConferencia([FromBody] ConferenciaRealizadaDTO filtro)
    {
        var registros = (from conferencia in _context.Conferencias
                         join convenio in _context.Convenios
                             on conferencia.ConvenioId equals convenio.Id
                         join documentos in _context.Documentos
                             on conferencia.DocumentoId equals documentos.Id
                         join ocorrencias in _context.Ocorrencias
                             on conferencia.OcorrenciaId equals ocorrencias.Id
                         join registroOcorrencias in _context.RegistroOcorrencias
                             on conferencia.RegistroOcorrenciasId equals registroOcorrencias.Id
                         where conferencia.ConvenioNome == filtro.ConvenioNome
                         select new ConferenciaRealizadaDTO
                         {
                             Id = conferencia.Id,
                             ConvenioId = conferencia.ConvenioId,
                             ConvenioNome = convenio.ConvenioNome,
                             Documento = documentos.Documento,
                             Ocorrencia = ocorrencias.Ocorrencia,
                             Descricao = registroOcorrencias.Descricao,
                             Status = conferencia.Status,
                             DataRetorno = conferencia.DataRetorno,
                             LinkOcorrencia = conferencia.LinkOcorrencia,
                             Usuario = conferencia.Usuario,
                             DataAtualizacao = conferencia.DataAtualizacao,

                             // Dados do convênio (via join)
                             Cpf = convenio.Cpf ?? "",
                             Matricula = convenio.Matricula ?? "",
                             Nome = convenio.Nome ?? "",
                             DataAdmissao = convenio.DataAdmissao,
                             DataDemissao = convenio.DataDemissao,
                             Situacao = convenio.Situacao ?? "",
                             Categoria = convenio.Categoria ?? "",
                             Funcao = convenio.Funcao ?? "",
                             Sexo = convenio.Sexo ?? ""
                         }).ToList();

        var pdfBytes = GerarRelatorioConferencias(registros);
        return File(pdfBytes, "application/pdf", "relatorio_conferencia.pdf");
        // return Ok(registros);
    }

    // [Authorize(Roles = "usuario")]
    // [HttpPost("relatorio-conferencia")]
    // public IActionResult GerarRelatorioConferencia([FromBody] ConferenciaFiltroDTO filtro)
    // {
    //     Console.WriteLine($"🔍 Filtro recebido: {filtro?.ConvenioNome}");

    //     if (string.IsNullOrEmpty(filtro?.ConvenioNome))
    //         return BadRequest("ConvenioNome está vazio ou nulo");

    //     var registros = ConsultarConferencias(filtro);
    //     var pdfBytes = GerarRelatorioConferencias(registros);

    //     return File(pdfBytes, "application/pdf", "relatorio_conferencia.pdf");
    // }

    private byte[] GerarRelatorioConferencias(List<ConferenciaRealizadaDTO> registros)
    {
        // 1. Agrupar os registros por CPF para iterar por colaborador
        var gruposPorCpf = registros
            .GroupBy(r => r.Cpf)
            .ToList();

        using var ms = new MemoryStream();
        using var writer = new PdfWriter(ms);
        using var pdf = new PdfDocument(writer);
        var doc = new Document(pdf);

        PdfFont fonteNegrito = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

        // --- BLOCO 1: ADICIONANDO O LOGO ---

        // 1. Defina o caminho completo para o seu arquivo de imagem no servidor
        // ATENÇÃO: Substitua este caminho pelo caminho real do seu logo no backend!
        string caminhoLogo = Path.Combine(_env.ContentRootPath, "Assets", "brasao.png");

        try
        {
            // 2. Carrega a imagem a partir do caminho
            ImageData data = ImageDataFactory.Create(caminhoLogo);
            Image logo = new Image(data);

            // 3. Define o tamanho da imagem (AJUSTE ESTES VALORES)
            float larguraLogo = 60f;
            float alturaLogo = logo.GetImageHeight() * larguraLogo / logo.GetImageWidth();
            logo.SetWidth(larguraLogo).SetHeight(alturaLogo);

            // 4. Define a posição da imagem (superior esquerda)
            // Você pode definir a posição absoluta, mas é mais fácil deixá-la no fluxo do documento.
            // Para colocar o texto ao lado, vamos adicionar o logo e o título principal em uma tabela de layout (1 linha, 2 colunas).

            // Crio uma tabela para alinhar o logo e o título do relatório lado a lado
            var tabelaCabecalho = new Table(UnitValue.CreatePercentArray(new float[] { 1, 4 })).UseAllAvailableWidth();

            // Célula 1: Logo
            tabelaCabecalho.AddCell(new Cell()
                .Add(logo)
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.TOP)); // Alinhar o logo ao topo

            // Célula 2: Título do Relatório
            tabelaCabecalho.AddCell(new Cell()
                .Add(new Paragraph("Relatório de Inconsistência")
                    .SetFont(fonteNegrito)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            doc.Add(tabelaCabecalho);
        }
        catch (FileNotFoundException ex)
        {
            // Se a imagem não for encontrada ou houver erro, loga e continua sem o logo
            Console.WriteLine($"ERRO: Logo não encontrado no caminho: {caminhoLogo}. Detalhe: {ex.Message}");
        }
        // --- TÍTULO DO RELATÓRIO ---
        doc.Add(new Paragraph("Relatório de Inconsistência")
            .SetFont(fonteNegrito)
            .SetFontSize(16)
            .SetTextAlignment(TextAlignment.CENTER));

        // Pega o convênio do primeiro registro (assumindo que o filtro garante um único convênio)
        string nomeConvenio = registros.FirstOrDefault()?.ConvenioNome ?? "N/A";

        // --- CABEÇALHO DO CONVÊNIO ---
        doc.Add(new Paragraph($"CONVÊNIO: ")
            .SetFont(fonteNegrito).SetFontSize(10)
            .Add(new Paragraph(nomeConvenio).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10)))
            .SetBottomMargin(15f);


        // 2. Iterar sobre os grupos de CPF
        foreach (var grupo in gruposPorCpf)
        {
            // Pega os dados fixos do colaborador (Primeiro item do grupo)
            var colaborador = grupo.First();

            // --- TABELA 1: DADOS PRINCIPAIS DO COLABORADOR ---
            var tabelaColaborador = new Table(UnitValue.CreatePercentArray(new float[] { 2.5f, 2f, 4f, 2.5f })).UseAllAvailableWidth();

            // Cabeçalho da Tabela
            tabelaColaborador.AddHeaderCell(new Cell().Add(new Paragraph("CPF").SetFont(fonteNegrito).SetFontSize(10)));
            tabelaColaborador.AddHeaderCell(new Cell().Add(new Paragraph("Matrícula").SetFont(fonteNegrito).SetFontSize(10)));
            tabelaColaborador.AddHeaderCell(new Cell().Add(new Paragraph("Nome").SetFont(fonteNegrito).SetFontSize(10)));
            tabelaColaborador.AddHeaderCell(new Cell().Add(new Paragraph("Data Admissão").SetFont(fonteNegrito).SetFontSize(10)));

            // Linha de Dados
            tabelaColaborador.AddCell(new Cell().Add(new Paragraph(colaborador.Cpf ?? "").SetFontSize(10)));
            tabelaColaborador.AddCell(new Cell().Add(new Paragraph(colaborador.Matricula ?? "").SetFontSize(10)));
            tabelaColaborador.AddCell(new Cell().Add(new Paragraph(colaborador.Nome ?? "").SetFontSize(10)));
            tabelaColaborador.AddCell(new Cell().Add(new Paragraph(colaborador.DataAdmissao?.ToString("dd/MM/yyyy") ?? "-").SetFontSize(10)));

            doc.Add(tabelaColaborador).SetBottomMargin(20f);


            // --- TÍTULO DA SEÇÃO DE DOCUMENTOS ---
            doc.Add(new Paragraph("Documentos Analisados")
                .SetFont(fonteNegrito)
                .SetFontSize(12)
                .SetMarginBottom(10f));


            // --- TABELA 2: DETALHES DAS INCONSISTÊNCIAS ---

            // Iterar sobre as inconsistências (registros) do colaborador
            foreach (var inconsistencia in grupo)
            {
                // Tabela de detalhes com uma única coluna
                var tabelaDetalhe = new Table(UnitValue.CreatePercentArray(new float[] { 1 })).UseAllAvailableWidth();

                // 1. Documentos de Admissão (Ocorrência)
                tabelaDetalhe.AddCell(new Cell()
                    .Add(new Paragraph(inconsistencia.Ocorrencia ?? "Documentos de Admissão") // Usando Ocorrência como título
                    .SetFont(fonteNegrito).SetFontSize(10))
                    .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)); // Fundo cinza

                // 2. Tipo Documento
                tabelaDetalhe.AddCell(new Cell()
                    .Add(new Paragraph("Tipo Documento: ")
                    .SetFont(fonteNegrito).SetFontSize(10)
                    .Add(new Paragraph(inconsistencia.Documento ?? "N/A").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10))));

                // 3. Inconsistência
                tabelaDetalhe.AddCell(new Cell()
                    .Add(new Paragraph("Inconsistência: ")
                    .SetFont(fonteNegrito).SetFontSize(10)
                    .Add(new Paragraph(inconsistencia.Descricao ?? "Nenhuma descrição disponível").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10))));

                // 4. Link
                tabelaDetalhe.AddCell(new Cell()
                    .Add(new Paragraph("Link: ")
                    .SetFont(fonteNegrito).SetFontSize(10)
                    .Add(new Paragraph(inconsistencia.LinkOcorrencia ?? "N/A").SetFontColor(iText.Kernel.Colors.ColorConstants.BLUE).SetFontSize(10))));

                doc.Add(tabelaDetalhe).SetBottomMargin(20f); // Espaçamento entre os blocos de inconsistência
            }

            // Adiciona um separador visual entre diferentes colaboradores
            doc.Add(new Paragraph("").SetMarginBottom(30f));
        }

        doc.Close();
        return ms.ToArray();
    }

    private List<ConferenciaRealizadaDTO> ConsultarConferencias(ConferenciaFiltroDTO filtro)
    {
        Console.WriteLine($"🔍 Consultando conferências... {filtro.ConvenioNome}");
        return (from conferencia in _context.Conferencias
                join convenio in _context.Convenios on conferencia.ConvenioId equals convenio.Id
                join documentos in _context.Documentos on conferencia.DocumentoId equals documentos.Id
                join ocorrencias in _context.Ocorrencias on conferencia.OcorrenciaId equals ocorrencias.Id
                join registroOcorrencias in _context.RegistroOcorrencias on conferencia.RegistroOcorrenciasId equals registroOcorrencias.Id
                where conferencia.ConvenioNome == filtro.ConvenioNome
                select new ConferenciaRealizadaDTO
                {
                    Id = conferencia.Id,
                    ConvenioId = conferencia.ConvenioId,
                    ConvenioNome = convenio.ConvenioNome,
                    Documento = documentos.Documento,
                    Ocorrencia = ocorrencias.Ocorrencia,
                    Descricao = registroOcorrencias.Descricao,
                    Status = conferencia.Status,
                    DataRetorno = conferencia.DataRetorno,
                    LinkOcorrencia = conferencia.LinkOcorrencia,
                    Usuario = conferencia.Usuario,
                    DataAtualizacao = conferencia.DataAtualizacao,
                    Cpf = convenio.Cpf ?? "",
                    Matricula = convenio.Matricula ?? "",
                    Nome = convenio.Nome ?? "",
                    DataAdmissao = convenio.DataAdmissao,
                    DataDemissao = convenio.DataDemissao,
                    Situacao = convenio.Situacao ?? "",
                    Categoria = convenio.Categoria ?? "",
                    Funcao = convenio.Funcao ?? "",
                    Sexo = convenio.Sexo ?? ""
                }).ToList();
    }



}
