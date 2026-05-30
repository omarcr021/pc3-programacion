using System.Net.Http.Json;
using TaskAnalysisAPI.Models.DTOs;

namespace TaskAnalysisAPI.Services
{
    public class ExternalTodoService : IExternalTodoService
    {
        private readonly HttpClient _httpClient;

        public ExternalTodoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ExternalTodoDTO>> GetExternalTodosAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var todos = await _httpClient.GetFromJsonAsync<List<JsonPlaceholderTodo>>("/todos", cancellationToken);
                if (todos == null) return Enumerable.Empty<ExternalTodoDTO>();

                return todos.Select(Map).ToList();
            }
            catch
            {
                throw; // let controller handle exceptions as controlled error
            }
        }

        public async Task<ExternalTodoDTO?> GetExternalTodoByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/todos/{id}", cancellationToken);
                if (!resp.IsSuccessStatusCode)
                {
                    if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
                    resp.EnsureSuccessStatusCode();
                }

                var todo = await resp.Content.ReadFromJsonAsync<JsonPlaceholderTodo>(cancellationToken: cancellationToken);
                if (todo == null) return null;
                return Map(todo);
            }
            catch
            {
                throw;
            }
        }

        private ExternalTodoDTO Map(JsonPlaceholderTodo t)
        {
            return new ExternalTodoDTO
            {
                ExternalId = t.id,
                Titulo = t.title,
                Completado = t.completed
            };
        }

        private class JsonPlaceholderTodo
        {
            public int userId { get; set; }
            public int id { get; set; }
            public string title { get; set; } = string.Empty;
            public bool completed { get; set; }
        }
    }
}
