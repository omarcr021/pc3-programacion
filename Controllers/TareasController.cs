using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAnalysisAPI.Data;
using TaskAnalysisAPI.Models;
using TaskAnalysisAPI.Models.DTOs;
using System.Globalization;

namespace TaskAnalysisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TareasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tareas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas([FromQuery] string? estado, [FromQuery] string? prioridad, [FromQuery] string? fechaInicio, [FromQuery] string? fechaFin)
        {
            // Validaciones y parseo de parámetros
            bool hasEstado = false;
            bool hasPrioridad = false;
            EstadoTarea estadoEnum = default;
            PrioridadTarea prioridadEnum = default;

            DateTime? inicio = null;
            DateTime? fin = null;

            if (!string.IsNullOrWhiteSpace(estado))
            {
                if (!Enum.TryParse<EstadoTarea>(estado, true, out estadoEnum))
                {
                    return BadRequest(new { mensaje = "El estado no es válido." });
                }
                hasEstado = true;
            }

            if (!string.IsNullOrWhiteSpace(prioridad))
            {
                if (!Enum.TryParse<PrioridadTarea>(prioridad, true, out prioridadEnum))
                {
                    return BadRequest(new { mensaje = "La prioridad no es válida." });
                }
                hasPrioridad = true;
            }

            if (!string.IsNullOrWhiteSpace(fechaInicio))
            {
                if (!DateTime.TryParse(fechaInicio, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fi))
                {
                    return BadRequest(new { mensaje = "fechaInicio no tiene un formato válido. Use yyyy-MM-dd." });
                }
                inicio = fi.Date;
            }

            if (!string.IsNullOrWhiteSpace(fechaFin))
            {
                if (!DateTime.TryParse(fechaFin, CultureInfo.InvariantCulture, DateTimeStyles.None, out var ff))
                {
                    return BadRequest(new { mensaje = "fechaFin no tiene un formato válido. Use yyyy-MM-dd." });
                }
                fin = ff.Date;
            }

            if (inicio.HasValue && fin.HasValue && inicio.Value > fin.Value)
            {
                return BadRequest(new { mensaje = "fechaInicio no puede ser mayor que fechaFin." });
            }

            var query = _context.Tareas.AsQueryable();

            if (hasEstado)
            {
                query = query.Where(t => t.Estado == estadoEnum);
            }

            if (hasPrioridad)
            {
                query = query.Where(t => t.Prioridad == prioridadEnum);
            }

            if (inicio.HasValue && fin.HasValue)
            {
                query = query.Where(t => t.FechaVencimiento.HasValue && t.FechaVencimiento.Value.Date >= inicio.Value && t.FechaVencimiento.Value.Date <= fin.Value);
            }
            else if (inicio.HasValue)
            {
                query = query.Where(t => t.FechaVencimiento.HasValue && t.FechaVencimiento.Value.Date >= inicio.Value);
            }
            else if (fin.HasValue)
            {
                query = query.Where(t => t.FechaVencimiento.HasValue && t.FechaVencimiento.Value.Date <= fin.Value);
            }

            var tareas = await query.ToListAsync();
            return Ok(tareas);
        }

        // GET: api/tareas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound(new { mensaje = $"No se encontró la tarea con Id {id}." });
            }

            return Ok(tarea);
        }

        // POST: api/tareas
        [HttpPost]
        public async Task<ActionResult<Tarea>> CreateTarea([FromBody] TareaCreateDTO tareaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que la fecha de vencimiento no sea menor a la fecha actual
            if (tareaDto.FechaVencimiento.HasValue && tareaDto.FechaVencimiento.Value < DateTime.Now)
            {
                return BadRequest(new { mensaje = "La fecha de vencimiento no puede ser menor a la fecha actual." });
            }

            var tarea = new Tarea
            {
                Titulo = tareaDto.Titulo,
                Descripcion = tareaDto.Descripcion,
                Estado = tareaDto.Estado,
                Prioridad = tareaDto.Prioridad,
                FechaCreacion = DateTime.Now,
                FechaVencimiento = tareaDto.FechaVencimiento
            };

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTarea), new { id = tarea.Id }, tarea);
        }

        // PUT: api/tareas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTarea(int id, [FromBody] TareaUpdateDTO tareaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound(new { mensaje = $"No se encontró la tarea con Id {id}." });
            }

            // Validar que la fecha de vencimiento no sea menor a la fecha actual
            if (tareaDto.FechaVencimiento.HasValue && tareaDto.FechaVencimiento.Value < DateTime.Now)
            {
                return BadRequest(new { mensaje = "La fecha de vencimiento no puede ser menor a la fecha actual." });
            }

            tarea.Titulo = tareaDto.Titulo;
            tarea.Descripcion = tareaDto.Descripcion;
            tarea.Estado = tareaDto.Estado;
            tarea.Prioridad = tareaDto.Prioridad;
            tarea.FechaVencimiento = tareaDto.FechaVencimiento;

            await _context.SaveChangesAsync();

            return Ok(tarea);
        }

        // DELETE: api/tareas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound(new { mensaje = $"No se encontró la tarea con Id {id}." });
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
