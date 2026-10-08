using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoPraksa.Model;
using Microsoft.AspNetCore.Mvc;

namespace MonoPraksa.Service
{
    public interface IWeatherForecastService
    {
        Task<IEnumerable<WeatherForecast>> GetAllWeatherForecastsAsync();
        Task<WeatherForecast> GetById(int id);
        Task<IEnumerable<WeatherForecast>> GetWeatherForecastsByTemperature(WeatherForecastFilter weatherForecastFilter);
        Task<bool> Post(WeatherForecast weatherForecast);
        Task<bool> Put(int id, string weatherForecast);
        Task<bool> Delete(int id);

    }
}
