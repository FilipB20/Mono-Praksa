# Endpoint skeletons

Worked example: get all forecasts for a city id.
Adapt names and types; keep the layering.

## 1. Repository interface

`WeatherForecast.Repository/IWeatherForecastRepository.cs`

```csharp
using MonoPraksa.Model;
// ... existing usings

namespace MonoPraksa.Repository
{
    public interface IWeatherForecastRepository
    {
        // ... existing members
        public Task<IEnumerable<WeatherForecast>> GetByCityIdAsync(int cityId);
    }
}
```

## 2. Repository implementation

`WeatherForecast.Repository/WeatherForecastRepository.cs`

```csharp
public async Task<IEnumerable<WeatherForecast>> GetByCityIdAsync(int cityId)
{
    return await _AppDbContext.WeatherForecasts
        .Where(forecast => forecast.CityId == cityId)
        .ToListAsync();
}
```

Writes follow the existing shape:

```csharp
public async Task Post(WeatherForecast weatherForecast)
{
    await _AppDbContext.WeatherForecasts.AddAsync(weatherForecast);
    await _AppDbContext.SaveChangesAsync();
}
```

## 3. Service interface

`WeatherForecast.Service/IWeatherForecastService.cs`

```csharp
using MonoPraksa.Model;
// ... existing usings

namespace MonoPraksa.Service
{
    public interface IWeatherForecastService
    {
        // ... existing members
        Task<IEnumerable<WeatherForecast>> GetByCityId(int cityId);
    }
}
```

## 4. Service implementation

`WeatherForecast.Service/WeatherForecastService.cs`

```csharp
public async Task<IEnumerable<WeatherForecast>> GetByCityId(int cityId)
{
    if (cityId <= 0)
    {
        return new List<WeatherForecast>();
    }

    return await _weatherForecastRepository.GetByCityIdAsync(cityId);
}
```

`ProjectRuleException` cannot be used here: its constructor has no access
modifier, so it is private and cannot be thrown from another class. Make it
public first if you really want a custom rule exception, and remember that
nothing maps it to an HTTP status yet.

Reads that must fail hard:

```csharp
public async Task<WeatherForecast> GetById(int id)
{
    var weatherForecast = await _weatherForecastRepository.GetById(id);
    if (weatherForecast == null)
    {
        throw new NotFoundException($"Nije moguce pronaci izvjestaj sa ID: {id}\n");
    }

    return weatherForecast;
}
```

Mutations return `bool` instead, so the controller can answer `400`/`404`:

```csharp
public async Task<bool> Post(WeatherForecast weatherForecast)
{
    if (weatherForecast == null) return false;

    await _weatherForecastRepository.Post(weatherForecast);
    return true;
}
```

## 5. Controller action

`Mono-Praksa/Controllers/WeatherForecastController.cs`

GET with a route parameter:

```csharp
[AllowAnonymous]
[HttpGet("city/{cityId}", Name = "GetWeatherForecastsByCity")]
public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetByCity(int cityId)
{
    _logger.LogInformation("Dohvacanje izvjestaja za grad sa ID: {cityId}.\n", cityId);

    var forecasts = await _weatherForecastService.GetByCityId(cityId);
    if (!forecasts.Any())
    {
        _logger.LogWarning("Ne postoje izvjestaji za grad sa ID: {cityId}!\n", cityId);
        return NotFound($"Ne postoje izvjestaji za grad sa ID: {cityId}!\n");
    }

    return Ok(forecasts.ToList());
}
```

POST with a body:

```csharp
[HttpPost(Name = "PostWeatherForecast")]
public async Task<IActionResult> Post([FromBody] WeatherForecast weatherForecast)
{
    _logger.LogInformation("Dodavanje novog vremenskog izvjestaja.\n");

    var isSuccess = await _weatherForecastService.Post(weatherForecast);
    if (!isSuccess)
    {
        _logger.LogWarning("Nije moguce dodati novi vremenski izvjestaj!\n");
        return BadRequest("Nije moguce dodati novi vremenski izvjestaj!\n");
    }

    return Created();
}
```

DELETE:

```csharp
[HttpDelete("{id}", Name = "DeleteWeatherForecast")]
public async Task<IActionResult> Delete(int id)
{
    _logger.LogInformation("Brisanje vremenskog izvjestaja sa ID: {id}.\n", id);

    var deleted = await _weatherForecastService.Delete(id);
    if (!deleted)
    {
        return NotFound($"Ne postoji izvjestaj sa ID: {id}!\n");
    }

    return NoContent();
}
```

## 6. Optional DTO

`WeatherForecast.Model/WeatherForecastRequest.cs`

```csharp
namespace MonoPraksa.Model;

public class WeatherForecastRequest
{
    public DateOnly Date { get; set; }
    public int? TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int? CityId { get; set; }
}
```

There is no validation-attribute usage in this repo yet. If you add
`[Required]`/`[Range]`, plain `[ApiController]` will return `400` automatically
with problem details.

## 7. DI registration (new types only)

`Mono-Praksa/Program.cs`

```csharp
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
```

## 8. Verification commands

Build:

```sh
dotnet build Mono-Praksa.sln
```

GET:

```sh
curl.exe -k "https://localhost:7263/WeatherForecast/city/1"
```

POST (PowerShell-safe: body from a file):

```powershell
$file = Join-Path $env:TEMP "forecast.json"
'{"date":"2026-11-01","temperatureC":15,"summary":"Test"}' | Set-Content $file -Encoding ascii
curl.exe -k -s -o NUL -w "%{http_code}" -X POST "https://localhost:7263/WeatherForecast" `
  -H "Content-Type: application/json" --data-binary "@$file"
Remove-Item $file
```

Expected codes: `201` for `Created()`, `204` for `NoContent()`, `200` for `Ok()`,
`400` for bad input, `404` for a missing row (only if the controller handles it).
