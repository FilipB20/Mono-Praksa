using Microsoft.AspNetCore.Mvc;
using MonoPraksa.Model;
using MonoPraksa.Repository;

namespace MonoPraksa.Service
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly IWeatherForecastRepository _weatherForecastRepository;

        public WeatherForecastService(IWeatherForecastRepository weatherForecastRepository)
        {
            this._weatherForecastRepository = weatherForecastRepository;
        }
        public async Task<IEnumerable<WeatherForecast>> GetAllWeatherForecastsAsync()
        {
            return await _weatherForecastRepository.GetAllWeatherForecastsAsync();
        }

        public async Task<WeatherForecast> GetById(int id)
        {
            var weatherForecast = await _weatherForecastRepository.GetById(id);
            if (weatherForecast == null)
            {
                return null;
            }

            return weatherForecast;
        }

        //Jel ovdje dobro slati body i cijeli objekt weatheForecastFilter ili preko querya da radim?
        //treba refaktorirat metodu, ostalo jos dok sam radio sa statickom listom
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsByTemperature(WeatherForecastFilter weatherForecastFilter)
        {
            List<WeatherForecast> forecasts = new List<WeatherForecast>();
            IEnumerable<WeatherForecast> forecastsEnum = await _weatherForecastRepository.GetAllWeatherForecastsAsync();
            List<WeatherForecast> weatherForecasts = forecastsEnum.ToList();

            foreach (var forecast in weatherForecasts)
            {
                if (forecast.TemperatureC >= weatherForecastFilter.TemperatureC && forecast.Date.CompareTo(weatherForecastFilter.Date) > 0)
                {
                    forecasts.Add(forecast);
                }
            }
            return forecasts;
        }

        public async Task<bool> Post(WeatherForecast weatherForecast)
        {
            if(weatherForecast == null) return false;
            await _weatherForecastRepository.Post(weatherForecast);
            return true;
        }

       public async Task<bool> Delete(int id)
        {
            var weatherForecast = await _weatherForecastRepository.GetById(id);
            if (weatherForecast == null)
            {
                return false;
            }
            await _weatherForecastRepository.Delete(weatherForecast);
            return true; 
        }
        
        public async Task<bool> Put(int id, String newSummary)
        {
            var item = await _weatherForecastRepository.GetById(id);
            if (item == null)
            {
                return false;
            }
            item.Summary = newSummary;
            await _weatherForecastRepository.Update(item);
            return true;
        }
    }
}

