# AGENTS.md

## Layout & naming
- Solution: `Mono-Praksa.sln`, all projects target `net8.0`.
- Layers: Web API `Mono-Praksa/MonoPraksa.WebApi.csproj` -> Service `WeatherForecast.Service/MonoPraksa.Service.csproj` -> Repository `WeatherForecast.Repository/MonoPraksa.Repository.csproj` -> Model/Common `WeatherForecast.Model` / `WeatherForecast.Common`.
- Folders keep legacy `WeatherForecast.*` names, but project/assembly/namespace names are `MonoPraksa.*`. Use `MonoPraksa.*` namespaces in code.
- `WeatherForecast.WebApi/` is a stray scaffold project not in the solution; ignore it.
- No test project or CI. Verification is `dotnet build Mono-Praksa.sln`.

## Commands
- Build: `dotnet build Mono-Praksa.sln`
- Run API: `dotnet run --project Mono-Praksa/MonoPraksa.WebApi.csproj --launch-profile http` (Swagger at `http://localhost:5220/swagger`). HTTPS profile uses `https://localhost:7263`.
- Required config is not committed; set it with user secrets (`UserSecretsId` is already in the WebApi csproj):
  - `dotnet user-secrets set "Jwt:Key" "<32+ char secret>" --project Mono-Praksa/MonoPraksa.WebApi.csproj`
  - `dotnet user-secrets set "ConnectionStrings:DefaultConnectionString" "Host=...;Database=...;Username=...;Password=..." --project Mono-Praksa/MonoPraksa.WebApi.csproj`
- `appsettings.json` has only `Jwt:Issuer`/`Jwt:Audience`; missing `Jwt:Key` crashes startup because `Program.cs` passes it straight to `Encoding.UTF8.GetBytes`.
- Connection string is read as `ConnectionStrings:DefaultConnectionString` by `UseNpgsql`.

## Data & EF
- PostgreSQL via Npgsql. `AppDbContext` is DB-first scaffolded and maps existing snake_case tables `cities`, `weather_forecasts`, `users` (including FK `city_fk` with cascade delete and unique index `uq_users_username`).
- There is no `Migrations/` directory; the schema is assumed to exist externally. Do not run `dotnet ef migrations add` unless you intentionally want to introduce migrations.
- Entities use `partial`, `virtual` navigation properties and explicit `HasName`/`HasColumnName` mappings. Changing names casually will break against the existing database.

## API behavior gotchas
- Routes: `AuthController` -> `/api/auth/register` and `/api/auth/login`; `WeatherForecastController` -> `/WeatherForecast`.
- Both auth endpoints have `[Authorize]` and no `[AllowAnonymous]`, so even login/register require a valid bearer token. This is likely unintended.
- Registration always creates role `"User"`; there is no admin-creation endpoint. Admin-only `POST`/`PUT`/`DELETE /WeatherForecast` require manually promoting a user in the DB.
- JWT claims: `NameIdentifier`, `Name`, `Role`; tokens expire after 2 hours. CORS only allows `http://localhost:5173`.
- Service methods throw `NotFoundException`, but there is no exception-handling middleware. Missing IDs therefore return 500, and the controllers' `null` checks for `GetById`/`Delete`/`Put` are unreachable.
- `PUT /WeatherForecast/{id}` binds `[FromBody] string newSummary`; send a JSON string (e.g. `"Sunny"`), not `{ "summary": "Sunny" }`.
- Known defect: `AuthService` constructor assigns `userRepository = userRepository` and `configuration = configuration` instead of the `_`-prefixed fields, so register/login throw `NullReferenceException` until fixed.
- Swagger UI is enabled only in Development.
