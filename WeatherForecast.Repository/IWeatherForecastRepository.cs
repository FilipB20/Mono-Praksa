using MonoPraksa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoPraksa.Repository
{
    public interface IWeatherForecastRepository
    {
        public IEnumerable<WeatherForecast> GetAllWeatherForecasts();
        public WeatherForecast GetById(int id);
        public void Post(WeatherForecast weatherForecast);
        public void Delete(WeatherForecast weatherForecast);
    }
}
