namespace TaskAnalysisAPI.Services
{
    public interface ISentimentService
    {
        /// <summary>
        /// Analiza el texto y devuelve true si es positivo, false si es negativo.
        /// </summary>
        Task<bool> IsPositiveAsync(string text);
    }
}
