using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Azure.Monitor.OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

var azureConnectionString = builder.Configuration["AzureMonitor:ConnectionString"] 
    ?? Environment.GetEnvironmentVariable("AZURE_MONITOR_CONNECTION_STRING");

builder.Services.AddOpenTelemetry()
    .WithMetrics(metricsBuilder =>
    {
        metricsBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ApiContactos"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
            
        if (!string.IsNullOrEmpty(azureConnectionString))
        {
            metricsBuilder.AddAzureMonitorMetricExporter(o =>
            {
                o.ConnectionString = azureConnectionString;
            });
        }
    })
    .WithTracing(tracingBuilder =>
    {
        tracingBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ApiContactos"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
            
        if (!string.IsNullOrEmpty(azureConnectionString))
        {
            tracingBuilder.AddAzureMonitorTraceExporter(o =>
            {
                o.ConnectionString = azureConnectionString;
            });
        }
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/obtenercontactos", () =>
{
    var contactos = new List<string> { "Amin Espinoza", "Oscar Barajas", "Pepe Rodelo" };
    return contactos;
})
.WithName("ObtenerContactos")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public partial class Program { }