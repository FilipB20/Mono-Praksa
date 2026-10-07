using Microsoft.AspNetCore.Mvc;
using MonoPraksa.Model;
using MonoPraksa.Repository;

namespace MonoPraksa.Service
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private IWeatherForecastRepository weatherForecastRepository;
        private static readonly List<WeatherForecast> weatherForecasts = new()
        {
            new WeatherForecast { Id = 0, Date = DateOnly.FromDateTime(DateTime.Now), TemperatureC = 20, Summary = "Mild" },
            new WeatherForecast { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), TemperatureC = 25, Summary = "Warm" }
        };

        public WeatherForecastService(IWeatherForecastRepository weatherForecastRepository)
        {
            this.weatherForecastRepository = weatherForecastRepository;
        }
        public IEnumerable<WeatherForecast> GetAllWeatherForecasts()
        {
            return weatherForecasts;
        }

        public WeatherForecast GetById(int id)
        {
            var weatherForecast = weatherForecasts.FirstOrDefault(w => w.Id == id);
            if (weatherForecast == null) return null;
            return weatherForecast;
        }

        //Jel ovdje dobro slati body i cijeli objekt weatheForecastFilter ili preko querya da radim?
        public IEnumerable<WeatherForecast> GetWeatherForecastsByTemperature(WeatherForecastFilter weatherForecastFilter)
        {
            List<WeatherForecast> forecasts = new List<WeatherForecast>();
            foreach (var forecast in weatherForecasts)
            {
                if (forecast.TemperatureC >= weatherForecastFilter.TemperatureC && forecast.Date.CompareTo(weatherForecastFilter.Date) > 0)
                {
                    forecasts.Add(forecast);
                }
            }
            return forecasts;
        }

        public bool Post(WeatherForecast weatherForecast)
        {
            if(weatherForecast == null) return false;
            weatherForecasts.Add(weatherForecast);
            return true;
        }

       public bool Delete(int id)
        {
            var weatherForecast = weatherForecasts.FirstOrDefault(w => w.Id == id);
            if (weatherForecast == null)
            {
                return false;
            }
            weatherForecasts.Remove(weatherForecast);
            return true;
            
        }
        
        public bool Put(int id, String newSummary)
        {
            var item = weatherForecasts.FirstOrDefault(w => w.Id == id);
            if (item == null)
            {
                return false;
            }
            item.Summary = newSummary;
            return true;
        }
    }
}

