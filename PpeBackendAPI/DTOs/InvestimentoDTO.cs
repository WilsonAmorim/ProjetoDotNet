namespace PpeBackendAPI.DTOs
{
    public class InvestimentoDTO
    {
        public int Id { get; set; }
        public string? DescricaoInvestimento { get; set; }
        public decimal? Valor { get; set; }
        public string? Periodo { get; set; }

    }

    public class CriarInvestimentoDTO
    {
        public string? DescricaoInvestimento { get; set; }
        public decimal? Valor { get; set; }
        public string? Periodo { get; set; }

    }
}