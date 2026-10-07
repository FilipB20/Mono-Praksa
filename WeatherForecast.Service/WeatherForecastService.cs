using Microsoft.AspNetCore.Mvc;
using MonoPraksa.Model;
using MonoPraksa.Repository;

namespace MonoPraksa.Service
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private IWeatherForecastRepository weatherForecastRepository;

        public WeatherForecastService(IWeatherForecastRepository weatherForecastRepository)
        {
            this.weatherForecastRepository = weatherForecastRepository;
        }
        public IEnumerable<WeatherForecast> GetAllWeatherForecasts()
        {
            return weatherForecastRepository.GetAllWeatherForecasts();
        }

        public WeatherForecast GetById(int id)
        {
            var weatherForecast = weatherForecastRepository.GetById(id);
            if (weatherForecast == null) return null;
            return weatherForecast;
        }

        //Jel ovdje dobro slati body i cijeli objekt weatheForecastFilter ili preko querya da radim?
        public IEnumerable<WeatherForecast> GetWeatherForecastsByTemperature(WeatherForecastFilter weatherForecastFilter)
        {
            List<WeatherForecast> forecasts = new List<WeatherForecast>();
            IEnumerable<WeatherForecast> weatherForecasts = weatherForecastRepository.GetAllWeatherForecasts();
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
            weatherForecastRepository.Post(weatherForecast);
            return true;
        }

       public bool Delete(int id)
        {
            var weatherForecast = weatherForecastRepository.GetById(id);
            if (weatherForecast == null)
            {
                return false;
            }
            weatherForecastRepository.Delete(weatherForecast);
            return true; 
        }
        
        public bool Put(int id, String newSummary)
        {
            var item = weatherForecastRepository.GetById(id);
            if (item == null)
            {
                return false;
            }
            item.Summary = newSummary;
            return true;
        }
    }
}

