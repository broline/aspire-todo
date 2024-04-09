using Microsoft.EntityFrameworkCore;
using Todo.Common;
using Todo.Data.DbContexts;

namespace Todo.Data;

public static class HostingExtensions
{
    public static IHostApplicationBuilder AddDb(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<TodoDbContext>(Constants.Database.Name);

        return builder;
    }

    public static async Task<WebApplication> UseDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        await context.Database.EnsureCreatedAsync();
        await context.Database.MigrateAsync();

        return app;
    }
}
