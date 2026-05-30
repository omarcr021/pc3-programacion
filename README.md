# TaskAnalysisAPI

Resumen
-------
TaskAnalysisAPI es una API REST creada con .NET 8 (ASP.NET Core) para gestionar tareas y proporcionar un endpoint de análisis de sentimiento. Permite crear, listar, actualizar y eliminar tareas (CRUD), además de filtrar tareas por estado, prioridad y rango de fechas. También ofrece un servicio simple para determinar si un comentario tiene sentimiento positivo o negativo.

Stack tecnológico
-----------------
- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core (migraciones incluidas)
- Servicio de análisis de sentimiento en la carpeta `Services`
- Datos de ejemplo/entrenamiento en la carpeta `ml`

Estructura clave
-----------------
- [Controllers/TareasController.cs](Controllers/TareasController.cs) : Endpoints CRUD y filtros.
- [Controllers/MlController.cs](Controllers/MlController.cs) : Endpoint para análisis de sentimiento.
- [Data/AppDbContext.cs](Data/AppDbContext.cs) : `DbContext` y configuración de entidades.
- [Models/Tarea.cs](Models/Tarea.cs) y DTOs en [Models/DTOs](Models/DTOs).
- [Services/SentimentService.cs](Services/SentimentService.cs) : Lógica para el análisis de sentimiento.
- Migraciones en la carpeta `Migrations`.

Requisitos
----------
- .NET 8 SDK instalado: https://dotnet.microsoft.com/
- (Opcional) EF Core tools para migraciones: `dotnet tool install --global dotnet-ef`

Instalación y ejecución
----------------------
1. Restaurar paquetes:
```bash
dotnet restore
```
2. (Opcional) Aplicar migraciones a la base de datos:
```bash
dotnet ef database update
```
3. Ejecutar la API:
```bash
dotnet run
```

La consola mostrará la URL local donde escucha la API (p. ej. `http://localhost:5000` o `https://localhost:5001`).

Endpoints principales
---------------------
- Tareas (CRUD)
	- GET `/api/tareas` : Lista todas las tareas. Soporta query params opcionales: `estado`, `prioridad`, `fechaInicio` (yyyy-MM-dd), `fechaFin` (yyyy-MM-dd).
	- GET `/api/tareas/{id}` : Obtiene una tarea por id.
	- POST `/api/tareas` : Crea una tarea. Body JSON según `TareaCreateDTO`.
	- PUT `/api/tareas/{id}` : Actualiza una tarea. Body JSON según `TareaUpdateDTO`.
	- DELETE `/api/tareas/{id}` : Elimina una tarea.

- ML / Sentimiento
	- POST `/api/ml/sentimiento` : Analiza el sentimiento de un comentario.
		- Body JSON: `{ "comentario": "Texto a analizar" }`
		- Respuesta JSON: `{ "comentario": "...", "sentimiento": "Positivo"|"Negativo" }`

Ejemplos (curl)
---------------
- Crear una tarea:
```bash
curl -X POST "http://localhost:5000/api/tareas" \
	-H "Content-Type: application/json" \
	-d '{
		"titulo": "Revisar informe",
		"descripcion": "Revisar el informe mensual",
		"estado": "Pendiente",
		"prioridad": "Alta",
		"fechaVencimiento": "2026-06-10"
	}'
```

- Listar tareas filtrando por prioridad:
```bash
curl "http://localhost:5000/api/tareas?prioridad=Alta"
```

- Analizar sentimiento:
```bash
curl -X POST "http://localhost:5000/api/ml/sentimiento" \
	-H "Content-Type: application/json" \
	-d '{"comentario":"Me encanta esta funcionalidad"}'
```

Notas sobre la base de datos y migraciones
-----------------------------------------
- Las migraciones ya están en la carpeta `Migrations`. Ajusta la cadena de conexión en `appsettings.json` o `appsettings.Development.json` según el proveedor (SQLite, SQL Server, etc.).
- Para crear nuevas migraciones:
```bash
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

Información adicional
---------------------
- Revisa las validaciones y DTOs en [Models/DTOs](Models/DTOs) para conocer los campos requeridos y formatos.
- El servicio de sentimiento se implementa en [Services/SentimentService.cs](Services/SentimentService.cs) y utiliza heurística/datos en `ml/priority_train.tsv`.
