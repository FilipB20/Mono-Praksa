using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonoPraksa.Model;
using MonoPraksa.Service;

namespace MonoPraksa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        IWeatherForecastService _weatherForecastService;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IWeatherForecastService weatherForecastService)
        {
            _logger = logger;
            this._weatherForecastService= weatherForecastService;
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetAll()
        {
            _logger.LogInformation("Dohvacanje svih podataka.\n");

            var forecasts = await _weatherForecastService.GetAllWeatherForecastsAsync();

            return Ok(forecasts);
        }

        //[Authorize]
        [HttpGet("{id}",Name = "GetWeatherForecastById")]
        public async Task<ActionResult<WeatherForecast>> GetById(int id)
        {
            _logger.LogInformation($"Dohvacanje podataka za ID: {id}.\n");

            var item = await _weatherForecastService.GetById(id);
            if (item == null)
            {
                _logger.LogWarning("Ne postoji izvjestaj sa ID: {id}!\n");
                return NotFound($"Ne postoji izvjestaj sa ID: {id}!\n");
            }
            return Ok(item);
        }

        [HttpGet("above-temperature",Name ="GetWeatherForecastsByTemperatureAbove")]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetWeatherForecastsByTemperature([FromQuery]WeatherForecastFilter weatherForecastFilter)
        {
            _logger.LogInformation("Dohvacanje izvjestaja sa temperaturom iznad {temperature} i datumom nakon {date}.\n", weatherForecastFilter.TemperatureC, weatherForecastFilter.Date);

            var forecasts = await _weatherForecastService.GetWeatherForecastsByTemperature(weatherForecastFilter);
            if (!forecasts.Any())
            {
                _logger.LogWarning("Ne postoje izvjestaji sa navedenim vrijednostima!\n");
                return NotFound("Ne postoje izvjestaji sa navedenim vrijednostima!\n");
            }
            return Ok(forecasts.ToList());
        }

        //[Authorize(Roles = "Admin")]
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

        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}",Name = "DeleteWeatherForecast")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Brisanje vremenskog izvjestaja sa ID: {id}.\n");

            var item = await _weatherForecastService.Delete(id);
            if (!item)
            {
                return NotFound($"Ne postoji izvjestaj sa ID: {id}!\n");
            }
            _logger.LogInformation($"Uspjesno obrisan izvjestaj sa ID: {id}.\n");
            return NoContent();
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name = "PutWeatherForecast")]
        public async Task<IActionResult> Put(int id, [FromBody] String newSummary)
        {
            _logger.LogInformation($"Ažuriranje vremenskog izvještaja sa ID: {id}.\n");

            var updated = await _weatherForecastService.Put(id, newSummary);
            if (!updated){
                _logger.LogWarning("Ne postoji izvjestaj s ID: {id}!\n");
                return BadRequest($"Ne postoji izvjestaj s ID: {id}!");
            }
            return NoContent();
        }
    }


}
