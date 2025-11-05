namespace PpeBackendAPI.Models
{
    public class Investimento
    {
        public int Id { get; set; }
        public string? DescricaoInvestimento { get; set; }
        public decimal? Valor { get; set; }
        public string? Periodo { get; set; }

    }
}
