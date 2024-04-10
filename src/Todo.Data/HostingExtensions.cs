using Microsoft.EntityFrameworkCore;
using Todo.Common;
using Todo.Data.DbContexts;
using Todo.Data.Interceptors;

namespace Todo.Data;

public static class HostingExtensions
{
    public static IHostApplicationBuilder AddDb(this IHostApplicationBuilder builder)
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

    public static async Task<WebApplication> UseDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        await context.Database.EnsureCreatedAsync();

        if (context.Database.HasPendingModelChanges())
        {
            await context.Database.MigrateAsync();
        }

        return app;
    }
}
