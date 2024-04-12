using Todo.Data;
using Microsoft.AspNetCore.Mvc;
using Todo.Abstractions;
using Todo.Common;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        x.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, TodoApiSerializationContext.Default);
    });

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

if (builder.Environment.EnvironmentName != Constants.EnvironmentNames.OpenApi)
    builder.AddDb();

var app = builder.Build();

if (app.Environment.EnvironmentName != Constants.EnvironmentNames.OpenApi)
    await app.UseDb();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.MapControllers();

app.Run();
