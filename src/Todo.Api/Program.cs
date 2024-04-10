using Todo.Data.DbContexts;
using Todo.Data;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Endpoints;
using Todo.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.Configure<ApiBehaviorOptions>(o =>
{
    o.InvalidModelStateResponseFactory = actionContext =>
    {
        return new BadRequestObjectResult(new ErrorResponse
        {
            Message = string.Join(" ", actionContext.ModelState.Values.SelectMany(v => v.Errors.Select(s => s.ErrorMessage)))
        });
    };
});

builder.AddDb();

var app = builder.Build();

await app.UseDb();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseTodoEndpoints();

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
