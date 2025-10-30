namespace PpeFrontend.DTOs
{
    public class ConvenioRegistroDTO
    {
        public int Id { get; set; }
        public string? ConvenioNome { get; set; }
        public string Cpf { get; set; } = "";
        public string Matricula { get; set; } = "";
        public string Nome { get; set; } = "";
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }
        public string Sexo { get; set; } = "";
        public string Situacao { get; set; } = "";
        public string Categoria { get; set; } = "";
        public string Funcao { get; set; } = "";
    }

}