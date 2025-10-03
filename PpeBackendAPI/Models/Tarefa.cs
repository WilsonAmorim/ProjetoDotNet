namespace PpeBackendAPI.Models
{
    public class Tarefa
    {
        public int Id { get; set; }
        public string? descricao { get; set; }
        public string? usuarioDestino { get; set; }
        public DateTime dataCriacao { get; set; }
        public DateTime dataExecucao { get; set; }
        public string? feedback { get; set; }
        public string? usuarioOrigem { get; set; }
        public string? status { get; set; }
        public string? observacao { get; set; }

    }

}
