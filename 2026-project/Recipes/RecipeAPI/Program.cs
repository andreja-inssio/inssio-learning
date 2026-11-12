using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RecipeAPI.Data;
using RecipeAPI.Endpoints;
using RecipeAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5094", "https://localhost:7066"];

builder.Services.AddCors(options => options.AddPolicy("BlazorClient",
    policy => policy.WithOrigins(
            allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDbContext<RecipeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Recipes")
        ?? throw new InvalidOperationException("Connection string 'Recipes' is required.")));
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IRecipeHistoryService, RecipeHistoryService>();
builder.Services.AddScoped<IRecipeReportService, RecipeReportService>();
builder.Services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors("BlazorClient");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapCategoryEndpoints();
app.MapIngredientEndpoints();
app.MapRecipeEndpoints();
app.MapRecipeHistoryEndpoints();
app.MapRecipeReportEndpoints();
app.MapRecipeIngredientEndpoints();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (CancellationToken cancellationToken) =>
{
    cancellationToken.ThrowIfCancellationRequested();
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return TypedResults.Ok(forecast);
})
.WithName("GetWeatherForecast")
.WithSummary("Get sample weather forecasts")
.WithDescription("Returns five sample forecasts from the Web API starter template.");

app.Run();

/// <summary>Represents a sample weather forecast.</summary>
sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
