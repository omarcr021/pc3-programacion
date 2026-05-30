using TaskAnalysisAPI.Models.DTOs;

namespace TaskAnalysisAPI.Services
{
    public interface IExternalTodoService
    {
        Task<IEnumerable<ExternalTodoDTO>> GetExternalTodosAsync(CancellationToken cancellationToken = default);
        Task<ExternalTodoDTO?> GetExternalTodoByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
