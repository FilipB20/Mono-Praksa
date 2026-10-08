using System;
using System.Collections.Generic;

namespace MonoPraksa.Model;

public partial class City
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public virtual ICollection<WeatherForecast> WeatherForecasts { get; set; } = new List<WeatherForecast>();
}
