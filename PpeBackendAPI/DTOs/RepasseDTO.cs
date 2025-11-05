namespace PpeBackendAPI.DTOs
{
    public class RepasseDTO
    {
        public int Id { get; set; }
        public string? NomeLote { get; set; }
        public string? ValorRepasse { get; set; }
        public string? Periodo { get; set; }
        public decimal? Valor { get; set; }
        public string? Status { get; set; }
        public string? Delay { get; set; }
        public DateTime DataPagamento { get; set; }
        public decimal? ValorInformado { get; set; }

    }

    public class CriarRepasseDTO
    {

        public string? NomeLote { get; set; }
        public string? ValorRepasse { get; set; }
        public string? Periodo { get; set; }
        public decimal? Valor { get; set; }
        public string? Status { get; set; }
        public string? Delay { get; set; }
        public DateTime DataPagamento { get; set; }
        public decimal? ValorInformado { get; set; }

    }
}