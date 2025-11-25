
namespace PpeBackendAPI.DTOs
{
    public class ConferenciaDTO
    {
        public int Id { get; set; }
        public int ConvenioId { get; set; }
        public int DocumentoId { get; set; }
        public int OcorrenciaId { get; set; }
        public int RegistroOcorrenciasId { get; set; }
        public string? ConvenioNome { get; set; }
        public string? Status { get; set; }
        public DateTime DataRetorno { get; set; }
        public string? LinkOcorrencia { get; set; }
        public string? Usuario { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class PesquisaConferenciasDto
    {
        public string Convenio { get; set; } = "";
    }

    public class ConferenciaRealizadaDTO
    {
        public int Id { get; set; }
        public int ConvenioId { get; set; }
        public int DocumentoId { get; set; }
        public int OcorrenciaId { get; set; }
        public int RegistroOcorrenciasId { get; set; }
        public string? ConvenioNome { get; set; }
        public string? Cpf { get; set; }
        public string? Matricula { get; set; }
        public string? Nome { get; set; }
        public string? Situacao { get; set; }
        public string? Categoria { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }
        public string? Convenio { get; set; }
        public string? Documento { get; set; }
        public string? Ocorrencia { get; set; }
        public string? Descricao { get; set; }
        public string? RegistroOcorrencias { get; set; }
        public string? Sexo { get; set; }
        public string? Funcao { get; set; }
        public string? Status { get; set; }
        public DateTime DataRetorno { get; set; }
        public string? LinkOcorrencia { get; set; }
        public string? Usuario { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class EditarConferenciaDTO
    {
        public string Status { get; set; } = "";
        public string? Usuario { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
