using System.Net.Http.Json;

namespace Domain;

public sealed class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public static async Task<WeatherForecast[]?> GetWeatherForecasts(HttpClient Http)
    {
        return await Http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
    }
}