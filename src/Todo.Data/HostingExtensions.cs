using Microsoft.EntityFrameworkCore;
using Todo.Common;
using Todo.Data.DbContexts;
using Todo.Data.Interceptors;

namespace Todo.Data;

public static class HostingExtensions
{
    public static IHostApplicationBuilder AddAspireDb(this IHostApplicationBuilder builder, bool addContext = false)
    {
        builder.Services.AddSingleton<IAuditableInterceptor>();

        builder.Services.AddSystemClock();

        builder.AddSqlServerDbContext<TodoDbContext>(Constants.Database.Name, settings =>
        {
        }, (cfg) =>
        {
            cfg.UseSqlServer(opt =>
            {
                opt.MigrationsAssembly(typeof(TodoDbContext).Assembly);
            })
            .AddInterceptors([new IAuditableInterceptor()]);
        });

        return builder;
    }

    public static IServiceCollection AddDb(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IAuditableInterceptor>();

        services.AddFrozenClock();

        services.AddDbContext<TodoDbContext>(
            options => options.UseSqlServer(connectionString)
            .AddInterceptors([new IAuditableInterceptor()]));

        return services;
    }

    public static void UseDb(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        context.Database.Migrate();
    }
}
