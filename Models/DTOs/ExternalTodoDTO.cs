namespace TaskAnalysisAPI.Models.DTOs
{
    public class ExternalTodoDTO
    {
        public int ExternalId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public bool Completado { get; set; }
    }
}
