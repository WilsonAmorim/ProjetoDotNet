

namespace PpeBackendAPI.DTOs
{
    public class ConvenioDTO
    {
        public int Id { get; set; }
        public string? ConvenioNome { get; set; }
        public string? Cpf { get; set; }
        public string? Matricula { get; set; }
        public string? Nome { get; set; }
        public string? Situacao { get; set; }
        public string? Categoria { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }
        public string? Sexo { get; set; }
        public string? Funcao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public string? Usuario { get; set; }
        public string? PostoTrabalho { get; set; }
        public string? MunicipioLotacao { get; set; }
    }

    public class ConvenioImportDto
    {
        public string Matricula { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Situacao { get; set; } = string.Empty;
        public string? Categoria { get; set; } // opcional
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Funcao { get; set; } = string.Empty;
        public string PostoTrabalho { get; set; } = string.Empty;
        public string MunicipioLotacao { get; set; } = string.Empty;
    }

}