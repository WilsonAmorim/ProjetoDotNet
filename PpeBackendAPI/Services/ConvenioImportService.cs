using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using PpeBackendAPI.Models;
using PpeBackendAPI.DTOs;
using PpeBackendAPI.Mapeamentos;




namespace PpeBackendAPI.Services;

public static class ConvenioImportService
{
    public static void ImportarConvenios(PpeDbContext context, string usuarioLogado)
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

            string convenioNome = arquivo switch
            {
                "conv1.csv" => "Fesfsus Lote 01",
                "conv2.csv" => "Flem Lote 02",
                "conv3.csv" => "Flem Lote 03",
                _ => "Desconhecido"
            };

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);

            // 🔄 Bloco para conv1.csv (sem categoria)
            if (arquivo == "conv1.csv")
            {
                csv.Context.RegisterClassMap<ConvenioImportMap>();
                var registros = csv.GetRecords<ConvenioImportDto>().ToList();
                ;

                foreach (var dto in registros)
                {
                    var existente = context.Convenios.FirstOrDefault(c =>
                        c.Cpf == dto.Cpf &&
                        c.ConvenioNome == convenioNome);

                    if (existente != null)
                    {
                        // Atualiza registro existente
                        existente.Nome = dto.Nome;
                        existente.Situacao = dto.Situacao;
                        existente.DataAdmissao = dto.DataAdmissao;
                        existente.DataDemissao = dto.DataDemissao;
                        existente.Sexo = dto.Sexo;
                        existente.Funcao = dto.Funcao;
                        existente.PostoTrabalho = dto.PostoTrabalho;
                        existente.MunicipioLotacao = dto.MunicipioLotacao;
                        existente.DataAtualizacao = DateTime.UtcNow;
                        existente.Usuario = usuarioLogado;

                        Console.WriteLine($"Atualizado: {dto.Cpf} - {dto.Nome}");
                    }
                    else
                    {
                        // Cria novo registro
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
                        Console.WriteLine($"Importado: {dto.Cpf} - {dto.Nome}");
                    }
                }
            }
            // 🔄 Bloco para conv2.csv e conv3.csv (com categoria)
            else
            {
                csv.Context.RegisterClassMap<ConvenioImportComCategoriaMap>();
                var registros = csv.GetRecords<ConvenioImportComCategoriaDto>().ToList();

                foreach (var dto in registros)
                {
                    var existente = context.Convenios.FirstOrDefault(c =>
                        c.Cpf == dto.Cpf &&
                        c.ConvenioNome == convenioNome);

                    if (existente != null)
                    {
                        existente.Nome = dto.Nome;
                        existente.Situacao = dto.Situacao;
                        existente.DataAdmissao = dto.DataAdmissao;
                        existente.DataDemissao = dto.DataDemissao;
                        existente.Sexo = dto.Sexo;
                        existente.Funcao = dto.Funcao;
                        existente.PostoTrabalho = dto.PostoTrabalho;
                        existente.MunicipioLotacao = dto.MunicipioLotacao;
                        existente.Categoria = dto.Categoria;
                        existente.DataAtualizacao = DateTime.UtcNow;
                        existente.Usuario = usuarioLogado;

                        Console.WriteLine($"Atualizado: {dto.Cpf} - {dto.Nome}");
                    }
                    else
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
                            Categoria = dto.Categoria,
                            ConvenioNome = convenioNome,
                            DataAtualizacao = DateTime.UtcNow,
                            Usuario = usuarioLogado
                        };

                        context.Convenios.Add(convenio);
                        Console.WriteLine($"Importado: {dto.Cpf} - {dto.Nome}");
                    }
                }
            }
        }

        context.SaveChanges();
    }

    public static async Task ImportarConvenioDoUpload(PpeDbContext context, StreamReader reader, string nomeArquivo, string usuarioLogado)
    {
        Console.WriteLine($"📂 Iniciando leitura do arquivo: {nomeArquivo}");

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            HeaderValidated = null,
            MissingFieldFound = null
        };

        using var csv = new CsvReader(reader, config);

        string convenioNome = nomeArquivo switch
        {
            "conv1.csv" => "Fesfsus Lote 01",
            "conv2.csv" => "Flem Lote 02",
            "conv3.csv" => "Flem Lote 03",
            _ => "Desconhecido"
        };

        Console.WriteLine($"📌 Convenio identificado: {convenioNome}");

        if (nomeArquivo == "conv1.csv")
        {
            csv.Context.RegisterClassMap<ConvenioImportMap>();
            var registros = csv.GetRecords<ConvenioImportDto>().ToList();
            Console.WriteLine($"📊 Registros lidos (sem categoria): {registros.Count}");

            foreach (var dto in registros)
            {
                Console.WriteLine($"🔄 Processando CPF: {dto.Cpf} - {dto.Nome}");

                var existente = context.Convenios.FirstOrDefault(c =>
                    c.Cpf == dto.Cpf && c.ConvenioNome == convenioNome);

                if (existente != null)
                {
                    Console.WriteLine($"✏️ Atualizando registro existente: {dto.Cpf}");

                    existente.Matricula = dto.Matricula;
                    existente.Nome = dto.Nome;
                    existente.Situacao = dto.Situacao;
                    existente.DataAdmissao = dto.DataAdmissao;
                    existente.DataDemissao = dto.DataDemissao;
                    existente.Sexo = dto.Sexo;
                    existente.Funcao = dto.Funcao;
                    existente.PostoTrabalho = dto.PostoTrabalho;
                    existente.MunicipioLotacao = dto.MunicipioLotacao;
                    existente.DataAtualizacao = DateTime.UtcNow;
                    existente.Usuario = usuarioLogado;
                }
                else
                {
                    Console.WriteLine($"➕ Criando novo registro: {dto.Cpf}");

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
                    Console.WriteLine($"✅ Registro salvo: {dto.Cpf} - {dto.Nome}");
                }
            }
        }
        else
        {
            csv.Context.RegisterClassMap<ConvenioImportComCategoriaMap>();
            var registros = csv.GetRecords<ConvenioImportComCategoriaDto>().ToList();
            Console.WriteLine($"📊 Registros lidos (com categoria): {registros.Count}");

            foreach (var dto in registros)
            {
                Console.WriteLine($"🔄 Processando CPF: {dto.Cpf} - {dto.Nome}");

                var existente = context.Convenios.FirstOrDefault(c =>
                    c.Cpf == dto.Cpf && c.ConvenioNome == convenioNome);

                if (existente != null)
                {
                    Console.WriteLine($"✏️ Atualizando registro existente: {dto.Cpf}");

                    existente.Matricula = dto.Matricula;
                    existente.Nome = dto.Nome;
                    existente.Situacao = dto.Situacao;
                    existente.DataAdmissao = dto.DataAdmissao;
                    existente.DataDemissao = dto.DataDemissao;
                    existente.Sexo = dto.Sexo;
                    existente.Funcao = dto.Funcao;
                    existente.PostoTrabalho = dto.PostoTrabalho;
                    existente.MunicipioLotacao = dto.MunicipioLotacao;
                    existente.Categoria = dto.Categoria;
                    existente.DataAtualizacao = DateTime.UtcNow;
                    existente.Usuario = usuarioLogado;
                }
                else
                {
                    Console.WriteLine($"➕ Criando novo registro: {dto.Cpf}");

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
                        Categoria = dto.Categoria,
                        ConvenioNome = convenioNome,
                        DataAtualizacao = DateTime.UtcNow,
                        Usuario = usuarioLogado
                    };

                    context.Convenios.Add(convenio);
                    Console.WriteLine($"✅ Registro salvo: {dto.Cpf} - {dto.Nome}");
                }
            }
        }

        try
        {
            Console.WriteLine("💾 Salvando alterações no banco...");
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dados salvos com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔥 Erro ao salvar no banco: {ex.Message}");
            throw;
        }
    }



}
