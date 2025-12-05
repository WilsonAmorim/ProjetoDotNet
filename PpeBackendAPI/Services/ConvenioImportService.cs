using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using PpeBackendAPI.Models;
using PpeBackendAPI.DTOs;
using PpeBackendAPI.Mapeamentos;
using ExcelDataReader;
using System.Data;
using System.Text;

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

    // 🔄 Auxiliar: lê Excel e normaliza colunas/dados
    private static DataTable LerExcelNormalizado(Stream excelStream)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using var reader = ExcelReaderFactory.CreateReader(excelStream);

        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration()
            {
                UseHeaderRow = true // 🔑 Usa a primeira linha como cabeçalho
            }
        });

        var table = result.Tables[0];

        // 1️⃣ Normalizar nomes das colunas
        foreach (DataColumn col in table.Columns)
        {
            col.ColumnName = col.ColumnName
                .Trim().ToUpper()
                .Replace("\\", "")
                .Replace("/", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("\n", "")
                .Replace("Á", "A")
                .Replace("á", "a")
                .Replace("Í", "I")
                .Replace("í", "i")
                .Replace("Ç", "C")
                .Replace("ç", "c")
                .Replace("Ã", "A")
                .Replace("ã", "a")
                .Replace("Ê", "E")
                .Replace("ê", "e")
                .Replace("É", "E")
                .Replace("é", "e")
                .Replace("Ó", "O")
                .Replace("ó", "o")
                .Replace("Ô", "O")
                .Replace("ô", "o")
                .Replace("Ú", "U")
                .Replace("ú", "u")
                .Replace(" ", "_");
        }

        // 2️⃣ Mapeamento de colunas
        var mapeamento = new Dictionary<string, string>
        {
                { "MATRICULA_FLEM", "matricula" },
                { "NOME", "nome" },
                { "CPF", "cpf" },
                { "UNIDADE_DE_LOTAÇÃO", "posto_trabalho" },
                { "MUNICIPIO_VAGA", "municipio_lotacao" },
                { "CURSO", "funcao" },
                { "CATEGORIA", "categoria" },
                { "SITUACAO", "situacao" },
                { "DATA_ADMISSAO", "data_admissao" },
                { "DATA_DESLIGAMENTO", "data_demissao" },
                { "SEXO", "sexo" }
        };

        // Criar DataTable final
        var tableFinal = new DataTable();
        foreach (var col in mapeamento.Values.Distinct())
            tableFinal.Columns.Add(col);

        // 3️⃣ Copiar dados aplicando mapeamento
        foreach (DataRow row in table.Rows)
        {
            var newRow = tableFinal.NewRow();
            foreach (var kv in mapeamento)
            {
                if (table.Columns.Contains(kv.Key))
                    newRow[kv.Value] = row[kv.Key]?.ToString() ?? "";
            }

            // 4️⃣ Tratar datas
            if (DateTime.TryParse(newRow["data_admissao"]?.ToString(), out var adm))
                newRow["data_admissao"] = adm.ToString("dd/MM/yyyy");

            if (DateTime.TryParse(newRow["data_demissao"]?.ToString(), out var dem))
                newRow["data_demissao"] = dem.ToString("dd/MM/yyyy");

            // 5️⃣ Ajustar matrícula e CPF
            if (double.TryParse(newRow["matricula"]?.ToString(), out var mat))
                newRow["matricula"] = mat.ToString("0");

            if (!string.IsNullOrEmpty(newRow["cpf"]?.ToString()))
                newRow["cpf"] = newRow["cpf"].ToString()?.PadLeft(11, '0');

            // 6️⃣ Filtrar categorias
            if (tableFinal.Columns.Contains("categoria"))
            {
                var categoria = newRow["categoria"].ToString();
                if (categoria != "CONTRATADO" && categoria != "DESLIGADO")
                    continue; // ignora linha
            }

            tableFinal.Rows.Add(newRow);
        }
        Console.WriteLine("📑 Colunas detectadas no Excel (normalizadas):");
        foreach (DataColumn col in table.Columns)
        {
            Console.WriteLine($" - {col.ColumnName}");
        }
        return tableFinal;
    }

    private static StreamReader ConverterExcelParaCsv(Stream excelStream)
    {
        var table = LerExcelNormalizado(excelStream);

        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

        foreach (DataRow row in table.Rows)
        {
            var linha = string.Join(",", row.ItemArray.Select(r => r?.ToString()));
            sb.AppendLine(linha);
        }

        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        return new StreamReader(csvStream);
    }

    public static async Task ImportarConvenioDoUpload(PpeDbContext context, Stream fileStream, string nomeArquivo, string usuarioLogado)
    {

        StreamReader reader;

        if (nomeArquivo.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            reader = new StreamReader(fileStream);
        else if (nomeArquivo.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) ||
                 nomeArquivo.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            reader = ConverterExcelParaCsv(fileStream);
        else
            throw new NotSupportedException("Formato de arquivo não suportado");


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

        bool isFesfsus = nomeArquivo.ToLower().Contains("fesf");
        bool isFlem02 = nomeArquivo.ToLower().Contains("flem02");
        bool isFlem03 = nomeArquivo.ToLower().Contains("flem03");

        string convenioNome = isFesfsus ? "Fesfsus Lote 01"
                   : isFlem02 ? "Flem Lote 02"
                   : isFlem03 ? "Flem Lote 03"
                   : "Desconhecido";

        Console.WriteLine($"📌 Convenio identificado: {convenioNome}");

        if (isFesfsus)
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
        else if (isFlem02 || isFlem03)
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
