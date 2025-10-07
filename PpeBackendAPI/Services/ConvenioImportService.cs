using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using PpeBackendAPI.Models;
using PpeBackendAPI.DTOs;

namespace PpeBackendAPI.Services;

public static class ConvenioImportService
{

    public static void ImportarConvenios(MeuDbContext context, string usuarioLogado)
    {
        var arquivos = new[] { "conv1.csv", "conv2.csv", "conv3.csv" };
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            HeaderValidated = null,
            MissingFieldFound = null
        };

        foreach (var arquivo in arquivos)
        {
            var path = Path.Combine("Data", arquivo);
            if (!File.Exists(path))
            {
                Console.WriteLine($"Arquivo não encontrado: {path}");
                continue;
            }

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);

            var registros = csv.GetRecords<ConvenioImportDto>().ToList();

            string convenioNome = arquivo switch
            {
                "conv1.csv" => "Fesfusu Lote 01",
                "conv2.csv" => "Flem Lote 02",
                "conv3.csv" => "Flem Lote 03",
                _ => "Desconhecido"
            };

            foreach (var dto in registros)
            {
                var convenio = new Convenio
                {
                    Matricula = dto.Matricula,
                    Nome = dto.Nome,
                    Situacao = dto.Situacao,
                    DataAdmissao = dto.DataAdmissao,
                    DataDemissao = dto.DataDemissao,
                    Sexo = dto.Sexo,
                    Cpf = dto.Cpf,
                    Funcao = dto.Funcao,
                    PostoTrabalho = dto.PostoTrabalho,
                    MunicipioLotacao = dto.MunicipioLotacao,
                    ConvenioNome = convenioNome,
                    DataAtualizacao = DateTime.UtcNow,
                    Usuario = usuarioLogado
                };

                context.Convenios.Add(convenio);
            }
        }

        context.SaveChanges();
    }
}
