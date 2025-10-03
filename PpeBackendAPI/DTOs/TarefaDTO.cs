namespace PpeBackendAPI.DTOs
{
    public class CriarTarefaDTO
    {
        public string? descricao { get; set; }
        public string? usuarioDestino { get; set; } // ID do usuário destino
        public string? observacao { get; set; }
        public string? status { get; set; }
    }

    public class TarefaDTO
    {
        public int Id { get; set; }
        public string descricao { get; set; } = "";
        public string status { get; set; } = "";
        public DateTime dataExecucao { get; set; }
        public string observacao { get; set; } = "";
        public string usuarioDestino { get; set; } = "";
        public string usuarioDestinoNome { get; set; } = "";
        public string usuarioOrigem { get; set; } = "";
        public string usuarioOrigemNome { get; set; } = "";
    }

    public class EditarTarefaDTO
    {
        public string status { get; set; } = "";
        public string observacao { get; set; } = "";
    }
}