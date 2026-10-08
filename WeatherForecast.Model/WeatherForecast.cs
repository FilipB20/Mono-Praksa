using System;
using System.Collections.Generic;

namespace MonoPraksa.Model;

public partial class WeatherForecast
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int? TemperatureC { get; set; }

    public string? Summary { get; set; }

    public int? CityId { get; set; }

    public virtual City? City { get; set; }
}
