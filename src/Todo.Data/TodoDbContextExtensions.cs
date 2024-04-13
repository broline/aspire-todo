using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime.CompilerServices;
using Todo.Data.DbContexts;

[assembly: InternalsVisibleTo("Todo.Api.Tests")]
namespace Todo.Data;

internal static class TodoDbContextExtensions
{
    public static async Task ResetDatabase(this TodoDbContext db)
    {
        var clearScriptStream = Assembly.GetAssembly(typeof(TodoDbContext))!
            .GetManifestResourceStream("Todo.Data.Scripts.Clear.sql");

        using StreamReader reader = new StreamReader(clearScriptStream!);
        var clearSql = reader.ReadToEnd();

        await db.Database.ExecuteSqlRawAsync(clearSql);
    }
}
