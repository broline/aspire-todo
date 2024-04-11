using Aspire.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Common;

namespace Todo.AppHost;

public static class ApplicationBuilder
{
    public static IDistributedApplicationBuilder CreateBuilder(string[] args)
    => CreateBuilder(DistributedApplication.CreateBuilder(new DistributedApplicationOptions { Args = args }));

    public static IDistributedApplicationBuilder CreateBuilder(DistributedApplicationOptions options, ApplicationBuilderOptions? builderOptions = null)
    => CreateBuilder(DistributedApplication.CreateBuilder(options));

    private static IDistributedApplicationBuilder CreateBuilder(IDistributedApplicationBuilder builder, ApplicationBuilderOptions builderOptions = null)
    {
        builderOptions ??= new();

        var sql = builder.AddSqlServer(Constants.AspireResources.Sql, "MyStrongSqlPassword!", 53547);
        if (builderOptions.PersistSqlData)
        {
            sql.WithVolumeMount("todo.sql", "/var/opt/mssql");
        }
        sql.AddDatabase(Constants.Database.Name);

        var apiService = builder.AddProject<Projects.Todo_Api>(Constants.AspireResources.Api)
            .WithReference(sql);

        builder.AddProject<Projects.Todo_Web>(Constants.AspireResources.Frontend)
            .WithReference(apiService);

        return builder;
    }
}

public class ApplicationBuilderOptions
{
    public bool PersistSqlData { get; set; } = true;
}
