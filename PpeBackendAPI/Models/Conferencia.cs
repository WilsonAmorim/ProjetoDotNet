namespace PpeBackendAPI.Models
{
    public class Conferencias
    {
        public int Id { get; set; } // Id da Conferência (chave primária)

        // 🔗 Chaves estrangeiras
        public int ConvenioId { get; set; }
        public int DocumentoId { get; set; }
        public int OcorrenciaId { get; set; }
        public int RegistroOcorrenciasId { get; set; }

        // 🔗 Propriedades de navegação
        public Convenio? Convenio { get; set; }
        public Documentos? Documento { get; set; }
        public Ocorrencias? Ocorrencia { get; set; }
        public RegistroOcorrencias? Descricao { get; set; }

        // 📄 Outros campos
        public string? ConvenioNome { get; set; }
        public string? Status { get; set; }
        public DateTime DataRetorno { get; set; }
        public string? LinkOcorrencia { get; set; }
        public string? Usuario { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
