using MonoPraksa.Model;

namespace MonoPraksa.Repository
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        public WeatherForecastRepository() { }

        private static readonly List<WeatherForecast> weatherForecasts = new()
        {
            new WeatherForecast { Id = 0, Date = DateOnly.FromDateTime(DateTime.Now), TemperatureC = 20, Summary = "Mild" },
            new WeatherForecast { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), TemperatureC = 25, Summary = "Warm" }
        };

        public IEnumerable<WeatherForecast> GetAllWeatherForecasts()
        {
            return weatherForecasts;
        }

        public WeatherForecast GetById(int id)
        {
            return weatherForecasts.FirstOrDefault(w => w.Id == id);
        }

        public void Post(WeatherForecast weatherForecast)
        {
            weatherForecasts.Add(weatherForecast);
        }

        public void Delete(WeatherForecast weatherForecast)
        {
            weatherForecasts.Remove(weatherForecast);

        }
    }
}
