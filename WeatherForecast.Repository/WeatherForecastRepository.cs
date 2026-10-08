using Microsoft.EntityFrameworkCore;
using MonoPraksa.Model;
using MonoPraksa.Common;
using System.Collections.Generic;

namespace MonoPraksa.Repository
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly AppDbContext AppDbContext;
        public WeatherForecastRepository(AppDbContext dbContext) {
            AppDbContext = dbContext;
        }

        public async Task<IEnumerable<WeatherForecast>> GetAllWeatherForecastsAsync()
        {
            return await AppDbContext.WeatherForecasts.ToListAsync();
        }

        public async Task<WeatherForecast> GetById(int id)
        {
            return await AppDbContext.WeatherForecasts.FindAsync(id);
        }

        public async Task Post(WeatherForecast weatherForecast)
        {
            await AppDbContext.WeatherForecasts.AddAsync(weatherForecast);
            await AppDbContext.SaveChangesAsync();
        }
        public async Task Update(WeatherForecast weatherForecast)
        {
            AppDbContext.WeatherForecasts.Update(weatherForecast);
            await AppDbContext.SaveChangesAsync();
        }

        public async Task Delete(WeatherForecast weatherForecast)
        {
            var item = await AppDbContext.WeatherForecasts.FindAsync(weatherForecast.Id);
            if (item != null)
            {
                AppDbContext.WeatherForecasts.Remove(item);
                await AppDbContext.SaveChangesAsync();
            }
        }
        
    }
}
