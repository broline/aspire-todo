using Todo.Common;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer(Constants.AspireResources.Sql, "MyStrongSqlPassword!", 53547)
                 .WithVolumeMount("todo.data.1", "/var/opt/mssql")
                 .AddDatabase(Constants.Database.Name);

var apiService = builder.AddProject<Projects.Todo_Api>(Constants.AspireResources.Api)
    .WithReference(sql);

builder.AddProject<Projects.Todo_Web>(Constants.AspireResources.Frontend)
    .WithReference(apiService);

builder.Build().Run();
