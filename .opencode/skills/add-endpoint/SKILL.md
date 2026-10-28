---
name: Add Endpoint
description: Use when adding, changing, or wiring an API endpoint in the MonoPraksa ASP.NET Core solution, including the controller action plus the matching service and repository methods.
---

# Add Endpoint

The solution has four layers and every endpoint walks through all of them:

```
Mono-Praksa/Controllers/*.cs        HTTP, status codes, logging
        ↓
WeatherForecast.Service/*.cs        business rules, validation, exceptions
        ↓
WeatherForecast.Repository/*.cs     EF Core queries, SaveChangesAsync
        ↓
WeatherForecast.Model/*.cs          entities and DTOs (PostgreSQL / DB-first)
```

Folders are named `WeatherForecast.*` but assemblies and namespaces are
`MonoPraksa.*`. Always use `MonoPraksa.*` namespaces.

## 1. Pin down the contract first

Before editing, decide and state:

- HTTP method and route (existing style: `[Route("[controller]")]`, so
  `WeatherForecastController` → `/WeatherForecast`)
- request source: `[FromBody]`, `[FromQuery]`, or route parameter
- response type and success code: `200 Ok`, `201 Created`, or `204 NoContent`
- failure codes: `400` bad input, `404` missing row, `401`/`403` auth
- auth: `[AllowAnonymous]` or `[Authorize(Roles = "Admin")]`

If the user has not specified these, ask before writing code.

## 2. Model or DTO

- Reuse an existing entity in `WeatherForecast.Model/` when the endpoint reads
  or writes that table.
- Add a new class in `WeatherForecast.Model/` with `namespace MonoPraksa.Model;`
  for request/response shapes. Do not copy `WeatherForecast.Model/UserDTO.cs`,
  which has no namespace.
- Entities are DB-first scaffolds with `virtual` navigation properties and
  explicit snake_case mappings in `AppDbContext`. Do not rename properties and
  do not add `Migrations/`.

## 3. Repository

Add the method to both `IWeatherForecastRepository` and
`WeatherForecastRepository` (or the matching repository for another entity).

- Namespace: `MonoPraksa.Repository`
- Injected dependency: `AppDbContext`
- Keep it to EF Core work only: `ToListAsync`, `FindAsync`, `AddAsync`,
  `Update`, `Remove`, then `await _AppDbContext.SaveChangesAsync()`.
- Register new repository types in `Program.cs` with `AddScoped`.

## 4. Service

Add the method to the interface in `WeatherForecast.Service/I*.cs` and implement
it in the matching service class.

- Namespace: `MonoPraksa.Service`
- Business rules, filtering, and validation live here, not in the controller.
- Reads throw `NotFoundException` from `MonoPraksa.Common` when a row is
  missing. Mutations in this repo return `Task<bool>` instead.
- Do not return `IActionResult` from a service.

## 5. Controller

Add the action to the controller in `Mono-Praksa/Controllers/`.

Follow the existing shape:

```csharp
[HttpGet("{id}", Name = "GetWeatherForecastById")]
public async Task<ActionResult<WeatherForecast>> GetById(int id)
{
    _logger.LogInformation($"Dohvacanje podataka za ID: {id}.\n");

    var item = await _weatherForecastService.GetById(id);
    if (item == null)
    {
        return NotFound($"Ne postoji izvjestaj sa ID: {id}!\n");
    }
    return Ok(item);
}
```

- Keep `[ApiController]` on the class and one explicit HTTP verb attribute per
  action.
- Give every action a unique `Name = "..."`; duplicates break Swagger.
- Log once on entry and once on failure, matching nearby Croatian messages.
- Prefer placeholders (`{id}`) over string interpolation in log calls.
- `Created()` returns `201` **with an empty body**; callers must not parse JSON.
- Add the route to Swagger by doing nothing else — `AddSwaggerGen()` is already
  configured.

## 6. Program.cs

Only touch `Program.cs` when you introduce a new service or repository type;
add a matching `builder.Services.AddScoped<IInterface, Implementation>();`.
No other wiring is needed for a new action on an existing controller.

## 7. Build and verify

```sh
dotnet build Mono-Praksa.sln
```

If the build fails with "file is locked by MonoPraksa.WebApi", stop the running
API first — the build itself is fine.

Run the API and exercise the endpoint:

```sh
dotnet run --project Mono-Praksa/MonoPraksa.WebApi.csproj --launch-profile http
```

Swagger: `http://localhost:5220/swagger`. HTTPS profile:
`https://localhost:7263` (ignore the self-signed certificate with `curl -k`).

For POST/PUT bodies, write the JSON to a temp file and use
`--data-binary "@file"`. PowerShell mangles inline double quotes and causes a
`400` about an invalid start of a property name.

Verify all three paths: success code, a missing/invalid id, and a malformed
body. Delete any test rows you created.

## Gotchas

- There is no exception-handling middleware. A `NotFoundException` thrown from a
  service becomes a `500`, and the controller's `null` checks never run. Either
  return `null`/`false` to the controller, or add middleware as a separate task.
- `PUT /WeatherForecast/{id}` binds `[FromBody] string`, so the body is a bare
  JSON string (`"Sunny"`), not an object.
- CORS allows only `http://localhost:5173`. New methods and headers need no
  change; a new origin does.
- `UseAuthentication()` runs on every request, so `Jwt:Key` must be configured
  even for anonymous endpoints.
- Never add EF migrations; the schema is managed outside this repo.

See `references/template.md` for copy-ready skeletons of the repository, service,
and controller pieces.
