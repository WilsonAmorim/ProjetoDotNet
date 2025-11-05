namespace PpeBackendAPI.DTOs
{
    public class EstagioDTO
    {
        public int Id { get; set; }
        public string? DescricaoEstagio { get; set; }
        public decimal? Valor { get; set; }
        public string? Periodo { get; set; }

    }

    public class CriarEstagioDTO
    {
        public string? DescricaoEstagio { get; set; }
        public decimal? Valor { get; set; }
        public string? Periodo { get; set; }

    }
}