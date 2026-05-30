using Microsoft.AspNetCore.Mvc;
using TaskAnalysisAPI.Models.DTOs;
using TaskAnalysisAPI.Services;

namespace TaskAnalysisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasExternasController : ControllerBase
    {
        private readonly IExternalTodoService _externalTodoService;

        public TareasExternasController(IExternalTodoService externalTodoService)
        {
            _externalTodoService = externalTodoService;
        }

        // GET: api/tareas-externas
        [HttpGet]
        public async Task<IActionResult> GetTareasExternas()
        {
            try
            {
                var todos = await _externalTodoService.GetExternalTodosAsync();
                return Ok(todos);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { mensaje = "La API externa no respondió correctamente." });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(503, new { mensaje = "La petición a la API externa excedió el tiempo de espera." });
            }
            catch
            {
                return StatusCode(503, new { mensaje = "Error al consumir la API externa." });
            }
        }

        // GET: api/tareas-externas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTareaExterna(int id)
        {
            try
            {
                var todo = await _externalTodoService.GetExternalTodoByIdAsync(id);
                if (todo == null)
                {
                    return NotFound(new { mensaje = $"No se encontró la tarea externa con Id {id}." });
                }
                return Ok(todo);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { mensaje = "La API externa no respondió correctamente." });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(503, new { mensaje = "La petición a la API externa excedió el tiempo de espera." });
            }
            catch
            {
                return StatusCode(503, new { mensaje = "Error al consumir la API externa." });
            }
        }
    }
}
