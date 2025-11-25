
namespace PpeFrontend.DTOs
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

    public class EditarConferenciaDTO
    {
        public string Status { get; set; } = "";
    }

}
