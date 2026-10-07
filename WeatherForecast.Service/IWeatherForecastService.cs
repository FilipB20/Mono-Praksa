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
        IEnumerable<WeatherForecast> GetAllWeatherForecasts();
        WeatherForecast GetById(int id);
        IEnumerable<WeatherForecast> GetWeatherForecastsByTemperature(WeatherForecastFilter weatherForecastFilter);
        bool Post(WeatherForecast weatherForecast);
        bool Put(int id, string weatherForecast);
        bool Delete(int id);

    }
}
