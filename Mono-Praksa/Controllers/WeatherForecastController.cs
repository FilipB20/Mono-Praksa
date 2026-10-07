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

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return weatherForecastService.GetAllWeatherForecasts();
        }

        [HttpGet("{id}",Name = "GetWeatherForecastById")]
        public WeatherForecast GetById(int id)
        {
            return weatherForecastService.GetById(id);
        }

        //Jel ovdje dobro slati body i cijeli objekt weatheForecastFilter ili preko querya da radim?
        [HttpGet("above-temperature",Name ="GetWeatherForecastsByTemperatureAbove")]
        public IEnumerable<WeatherForecast> GetWeatherForecastsByTemperature([FromBody]WeatherForecastFilter weatherForecastFilter)
        {
            return weatherForecastService.GetWeatherForecastsByTemperature(weatherForecastFilter);
        }

        [HttpPost(Name ="PostWeatherForecast")]
        public IActionResult Post([FromBody]WeatherForecast weatherForecast)
        {
            if (weatherForecastService.Post(weatherForecast)){
                return Ok("Uspješno dodan novi izvještaj!\n");
            }
            return NotFound("Nije moguce dodati novi vremenski izvještaj!\n");
        }

        [HttpDelete("{id}",Name = "DeleteWeatherForecast")]
        public IActionResult Delete(int id)
        {
            if (weatherForecastService.Delete(id))
            {
                return Ok($"Uspješno obrisan izvještaj s ID: {id}!\n");
            }
            return NotFound($"Ne postoji izvjestaj sa ID: {id}!\n");
        }

        [HttpPut("{id}", Name = "PutWeatherForecast")]
        public IActionResult Put(int id, [FromBody] String newSummary)
        {
            if (weatherForecastService.Put(id, newSummary)){
                return Ok($"Uspješno izmijenjen izvještaj s ID {id} i dodan je summary:{newSummary}\n");
            }
            return NotFound($"Ne postoji izvještaj s ID: {id}!");
        }
    }


}
