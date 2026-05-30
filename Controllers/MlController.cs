using Microsoft.AspNetCore.Mvc;
using TaskAnalysisAPI.Models.DTOs;
using TaskAnalysisAPI.Services;

namespace TaskAnalysisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MlController : ControllerBase
    {
        private readonly ISentimentService _sentimentService;

        public MlController(ISentimentService sentimentService)
        {
            _sentimentService = sentimentService;
        }

        // POST: api/ml/sentimiento
        [HttpPost("sentimiento")]
        public async Task<IActionResult> AnalizarSentimiento([FromBody] SentimentRequestDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Comentario))
            {
                return BadRequest(new { mensaje = "El campo 'comentario' es requerido." });
            }

            try
            {
                var isPositive = await _sentimentService.IsPositiveAsync(request.Comentario);
                var response = new SentimentResponseDTO
                {
                    Comentario = request.Comentario,
                    Sentimiento = isPositive ? "Positivo" : "Negativo"
                };
                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al analizar el sentimiento." });
            }
        }
    }
}
