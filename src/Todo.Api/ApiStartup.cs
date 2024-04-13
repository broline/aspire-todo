using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using static Todo.Common.Constants;
using Todo.Abstractions;
using Todo.Data;
using Microsoft.AspNetCore.Builder;

namespace Todo.Api;

public class ApiStartup
{
    private readonly IWebHostEnvironment _env;
    private IConfiguration _configuration = default!;

    public ApiStartup(IWebHostEnvironment env, IConfiguration config)
    {
        _env = env;
        _configuration = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument();
        services.AddSwaggerGen();

        // Add services to the container.
        services.AddProblemDetails();

        services.AddControllers()
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                x.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, TodoApiSerializationContext.Default);
            });

        services.Configure<ApiBehaviorOptions>(o =>
        {
            o.InvalidModelStateResponseFactory = actionContext =>
            {
                return new BadRequestObjectResult(new ErrorResponse
                {
                    Message = string.Join(" ", actionContext.ModelState.Values.SelectMany(v => v.Errors.Select(s => s.ErrorMessage)))
                });
            };
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        if (_env.EnvironmentName != EnvironmentNames.OpenApi)
            app.UseDb();

        if (!_env.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseExceptionHandler();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

public static class HostingExtensions
{
    public static IServiceCollection AddOptionsWithStartupValidation<T>(this IServiceCollection services, string binding)
        where T : class
    {
        services.AddOptions<T>()
            .BindConfiguration(binding)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static bool IsDebug(this IWebHostEnvironment env)
    {
#if DEBUG
        return true;
#endif
#pragma warning disable CS0162 // Unreachable code detected
        return false;
#pragma warning restore CS0162 // Unreachable code detected
    }

    public static bool IsIntegrationTest(this IWebHostEnvironment env)
    {
        return env.EnvironmentName.ToLower() == "test";
    }
}