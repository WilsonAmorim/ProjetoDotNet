
namespace PpeBackendAPI.DTOs
{
    public class OcorrenciasDTO
    {
        public int Id { get; set; }
        public int DocumentoId { get; set; }
        public string? Ocorrencia { get; set; }
    }

    public class NovaOcorrenciasDTO
    {
        public int DocumentoId { get; set; }
        public string? Ocorrencia { get; set; }
    }
}
