namespace TaskAnalysisAPI.Models.DTOs
{
    public class SentimentResponseDTO
    {
        public string Comentario { get; set; } = string.Empty;
        public string Sentimiento { get; set; } = string.Empty; // "Positivo" | "Negativo"
    }
}
