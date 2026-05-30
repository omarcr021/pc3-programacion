using System.ComponentModel.DataAnnotations;

namespace TaskAnalysisAPI.Models.DTOs
{
    public class TareaUpdateDTO
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public EstadoTarea Estado { get; set; }

        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        public PrioridadTarea Prioridad { get; set; }

        public DateTime? FechaVencimiento { get; set; }
    }
}
