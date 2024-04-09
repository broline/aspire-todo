using Todo.Data.DbContexts;
using Todo.Data;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddDb();

var app = builder.Build();

await app.UseDb();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();


app.MapGet("/todos/{todoId}", async (
    [FromRoute] Guid todoId,
    [FromServices] TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(todoId);
    return todo;
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
