using CsvHelper.Configuration;
using PpeBackendAPI.DTOs;


namespace PpeBackendAPI.Mapeamentos  // ou o namespace que você estiver usando
{

    public sealed class ConvenioImportMap : ClassMap<ConvenioImportDto>
    {
        public ConvenioImportMap()
        {
            Map(m => m.Matricula).Name("matricula");
            Map(m => m.Nome).Name("nome");
            Map(m => m.Situacao).Name("situacao");
            Map(m => m.DataAdmissao).Name("data_admissao")
                .TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.DataDemissao).Name("data_demissao")
                .TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.Sexo).Name("sexo");
            Map(m => m.Cpf).Name("cpf");
            Map(m => m.Funcao).Name("funcao");
            Map(m => m.PostoTrabalho).Name("posto_trabalho");
            Map(m => m.MunicipioLotacao).Name("Municipio_lotacao");
        }
    }

    public sealed class ConvenioImportComCategoriaMap : ClassMap<ConvenioImportComCategoriaDto>
    {
        public ConvenioImportComCategoriaMap()
        {
            Map(m => m.Matricula).Name("matricula");
            Map(m => m.Nome).Name("nome");
            Map(m => m.Situacao).Name("situacao");
            Map(m => m.Categoria).Name("categoria");
            Map(m => m.DataAdmissao).Name("data_admissao")
                .TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.DataDemissao).Name("data_demissao")
                .TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.Sexo).Name("sexo");
            Map(m => m.Cpf).Name("cpf");
            Map(m => m.Funcao).Name("funcao");
            Map(m => m.PostoTrabalho).Name("posto_trabalho");
            Map(m => m.MunicipioLotacao).Name("Municipio_lotacao");
        }
    }
}