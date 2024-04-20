using Todo.Common;

var builder = DistributedApplication.CreateBuilder(args);

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
    .WithReference(sqlDb);

builder.AddProject<Projects.Todo_Web>(Constants.AspireResources.Frontend)
    .WithReference(apiService);

builder.Build().Run();
