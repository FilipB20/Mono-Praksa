using Microsoft.AspNetCore.Mvc;

namespace Mono_Praksa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly List<WeatherForecast> weatherForecasts = new()
        {
            new WeatherForecast { Id = 0, Date = DateOnly.FromDateTime(DateTime.Now), TemperatureC = 20, Summary = "Mild" },
            new WeatherForecast { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), TemperatureC = 25, Summary = "Warm" }
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return weatherForecasts;
        }

        [HttpGet("{id}",Name = "GetWeatherForecastById")]
        public WeatherForecast GetById(int id)
        {
            var weatherForecast = weatherForecasts.FirstOrDefault(w => w.Id == id);
            if (weatherForecast == null) return null;
            return weatherForecast;
        }

        //Jel ovdje dobro slati body i cijeli objekt weatheForecastFilter ili preko querya da radim?
        [HttpGet("above-temperature",Name ="GetWeatherForecastsByTemperatureAbove")]
        public IEnumerable<WeatherForecast> GetWeatherForecastsByTemperature([FromBody]WeatherForecastFilter weatherForecastFilter)
        {
            List<WeatherForecast> forecasts = new List<WeatherForecast>();
            foreach(var forecast in weatherForecasts)
            {
                if (forecast.TemperatureC >= weatherForecastFilter.TemperatureC && forecast.Date.CompareTo(weatherForecastFilter.Date) > 0)
                {
                    forecasts.Add(forecast);
                }
            }
            return forecasts;
        }

        [HttpPost(Name ="PostWeatherForecast")]
        public IActionResult Post([FromBody]WeatherForecast weatherForecast)
        {
            weatherForecasts.Add(weatherForecast);
            return Ok();
        }

        [HttpDelete("{id}",Name = "DeleteWeatherForecast")]
        public IActionResult Delete(int id)
        {
            var weatherForecast = weatherForecasts.FirstOrDefault(w => w.Id == id);
            if (weatherForecast == null)
            {
                return NotFound($"Ne postoji izvještaj sa ID: {id}!\n");
            }
            weatherForecasts.Remove(weatherForecast);
            return Ok();
        }

        [HttpPut("{id}",Name ="PutWeatherForecast")]
        public IActionResult Update(int id, [FromBody]String newSummary)
        {
            var item = weatherForecasts.FirstOrDefault(w => w.Id == id);
            if (item == null)
            {
                return NotFound($"Ne postoji izvještaj s ID: {id}!");
            }
            item.Summary = newSummary;
            return Ok();
        } 
    }


}
