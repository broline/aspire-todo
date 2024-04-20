using Todo.Common;

const string ENV_KEY = "DOTNET_ENVIRONMENT";

var builder = DistributedApplication.CreateBuilder(args);

var environment = builder.AddParameter("dotnet-environment");

IResourceBuilder<SqlServerServerResource> sql = default!; 

if (builder.ExecutionContext.IsRunMode)
{
    var sqlPwd = builder.AddParameter("SqlPassword");

    sql = builder.AddSqlServer(Constants.AspireResources.Sql, sqlPwd, 53547)
        .WithDataVolume("todo.sql.7");
}
else
{
    sql = builder.AddSqlServer(Constants.AspireResources.Sql)
        .PublishAsAzureSqlDatabase();
}

var sqlDb = sql
    .AddDatabase(Constants.Database.Name);

var apiService = builder.AddProject<Projects.Todo_Api>(Constants.AspireResources.Api)
    .WithEnvironment(ENV_KEY, environment)
    .WithReference(sqlDb);

builder.AddProject<Projects.Todo_Web>(Constants.AspireResources.Frontend)
    // TODO: In order to make the frontend publicly accessible, these settings need to be added to the manifest
    // However, this replaces the whole manifest for this resource which is not what I awnt to do. Need to find out how to append
    //.WithManifestPublishingCallback(ctx =>
    //{
    //    ctx.Writer.WriteStartObject("configuration");
    //    ctx.Writer.WriteStartObject("ingress");
    //    ctx.Writer.WriteBoolean("external", true);
    //    ctx.Writer.WriteEndObject();
    //    ctx.Writer.WriteEndObject();
    //})
    .WithEnvironment(ENV_KEY, environment)
    .WithReference(apiService);

builder.Build().Run();
