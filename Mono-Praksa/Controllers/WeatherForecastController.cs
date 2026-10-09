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
        IWeatherForecastService weatherForecastService;
        private readonly ILogger<WeatherForecastController> _logger;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IWeatherForecastService weatherForecastService)
        {
            _logger = logger;
            this.weatherForecastService= weatherForecastService;
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> GetAll()
        {
            return await weatherForecastService.GetAllWeatherForecastsAsync();
        }

        [Authorize]
        [HttpGet("{id}",Name = "GetWeatherForecastById")]
        public async Task<WeatherForecast> GetById(int id)
        {
            return await weatherForecastService.GetById(id);
        }

        //Jel ovdje dobro slati body i cijeli objekt weatheForecastFilter ili preko querya da radim?
        [HttpGet("above-temperature",Name ="GetWeatherForecastsByTemperatureAbove")]
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsByTemperature([FromBody]WeatherForecastFilter weatherForecastFilter)
        {
            return await weatherForecastService.GetWeatherForecastsByTemperature(weatherForecastFilter);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost(Name ="PostWeatherForecast")]
        public async Task<IActionResult>Post([FromBody]WeatherForecast weatherForecast)
        {
            if (await weatherForecastService.Post(weatherForecast)){
                return Ok("Uspješno dodan novi izvještaj!\n");
            }
            return NotFound("Nije moguce dodati novi vremenski izvještaj!\n");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}",Name = "DeleteWeatherForecast")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await weatherForecastService.Delete(id))
            {
                return Ok($"Uspješno obrisan izvještaj s ID: {id}!\n");
            }
            return NotFound($"Ne postoji izvjestaj sa ID: {id}!\n");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name = "PutWeatherForecast")]
        public async Task<IActionResult> Put(int id, [FromBody] String newSummary)
        {
            if (await weatherForecastService.Put(id, newSummary)){
                return Ok($"Uspješno izmijenjen izvještaj s ID {id} i dodan je summary:{newSummary}\n");
            }
            return NotFound($"Ne postoji izvještaj s ID: {id}!");
        }
    }


}
