using Microsoft.EntityFrameworkCore;
using MonoPraksa.Model;
using MonoPraksa.Common;
using System.Collections.Generic;

namespace MonoPraksa.Repository
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly AppDbContext _AppDbContext;
        public WeatherForecastRepository(AppDbContext dbContext) {
            _AppDbContext = dbContext;
        }

        public async Task<IEnumerable<WeatherForecast>> GetAllWeatherForecastsAsync()
        {
            return await _AppDbContext.WeatherForecasts.ToListAsync();
        }

        public async Task<WeatherForecast> GetById(int id)
        {
            return await _AppDbContext.WeatherForecasts.FindAsync(id);
        }

        public async Task Post(WeatherForecast weatherForecast)
        {
            await _AppDbContext.WeatherForecasts.AddAsync(weatherForecast);
            await _AppDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(WeatherForecast weatherForecast)
        {
            _AppDbContext.WeatherForecasts.Update(weatherForecast);
            await _AppDbContext.SaveChangesAsync();
        }

        public async Task Delete(WeatherForecast weatherForecast)
        {
            var item = await _AppDbContext.WeatherForecasts.FindAsync(weatherForecast.Id);
            if (item != null)
            {
                _AppDbContext.WeatherForecasts.Remove(item);
                await _AppDbContext.SaveChangesAsync();
            }
        }
        
    }
}
