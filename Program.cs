using Microsoft.EntityFrameworkCore;
using TaskAnalysisAPI.Data;
using TaskAnalysisAPI.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configurar EF Core con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serializar enums como string en las respuestas JSON
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Registrar servicio para consumir JSONPlaceholder
builder.Services.AddHttpClient<IExternalTodoService, ExternalTodoService>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar servicio de ML (sentiment)
builder.Services.AddSingleton<ISentimentService, SentimentService>();

// (Nota) Registro de servicio externo no agregado aquí — puede añadirse cuando el servicio exista.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Página principal simple que muestra el funcionamiento y ejemplos de uso
app.MapGet("/", async context =>
{
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(@"<!doctype html>
<html lang=""es""> 
<head>
    <meta charset=""utf-8""> 
    <title>TaskAnalysisAPI - Inicio</title>
    <style>
        body { font-family: Arial, Helvetica, sans-serif; margin: 2rem; color: #222 }
        h1 { color: #0366d6 }
        code { background: #f6f8fa; padding: .2rem .4rem; border-radius: 4px }
        pre { background: #f6f8fa; padding: .8rem; border-radius: 6px }
    </style>
</head>
<body>
    <h1>TaskAnalysisAPI</h1>
    <p>API REST para gestionar tareas. Endpoints principales:</p>
    <ul>
        <li><strong>GET</strong> <code>/api/tareas</code> — Listar tareas (acepta filtros: <code>?estado=Pendiente</code>, <code>?prioridad=Alta</code>, <code>?fechaInicio=yyyy-MM-dd&amp;fechaFin=yyyy-MM-dd</code>)</li>
        <li><strong>GET</strong> <code>/api/tareas/{id}</code> — Obtener tarea por id</li>
        <li><strong>GET</strong> <code>/api/tareas-externas</code> — Lista mapeada desde JSONPlaceholder</li>
        <li><strong>GET</strong> <code>/api/tareas-externas/{id}</code> — Obtener tarea externa por id</li>
        <li><strong>POST</strong> <code>/api/ml/sentimiento</code> — Analizar sentimiento. Body: <code>{ ""comentario"": ""texto"" }</code></li>
        <li><strong>Swagger UI</strong>: <a href=""/swagger"">/swagger</a> (solo en modo Development)</li>
    </ul>

    <h2>Ejemplos rápidos (curl)</h2>
    <pre>curl http://localhost:5000/api/tareas
curl ""http://localhost:5000/api/tareas?estado=Pendiente""
curl -X POST http://localhost:5000/api/ml/sentimiento -H ""Content-Type: application/json"" -d '{""comentario"":""La tarea fue completada correctamente""}'</pre>

    <p>Para más detalles, abre <a href=""/swagger"">Swagger UI</a> cuando ejecutes la aplicación en Development.</p>
</body>
</html>");
});

app.MapControllers();

app.Run();
