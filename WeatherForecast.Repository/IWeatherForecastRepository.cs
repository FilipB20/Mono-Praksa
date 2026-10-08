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
        public Task<IEnumerable<WeatherForecast>> GetAllWeatherForecastsAsync();
        public Task<WeatherForecast> GetById(int id);
        public Task Post(WeatherForecast weatherForecast);
        public Task Delete(WeatherForecast weatherForecast);
        public Task Update(WeatherForecast weatherForecast);
    }
}
