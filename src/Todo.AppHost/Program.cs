using Todo.Common;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer(Constants.AspireResources.Sql, "MyStrongSqlPassword!", 1433)
                 .WithVolumeMount("VolumeMount.sqlserver.data", "/var/opt/mssql")
                 .WithEndpoint(1433, 1433)
                 .AddDatabase(Constants.Database.Name);

var apiService = builder.AddProject<Projects.Todo_Api>(Constants.AspireResources.Api)
    .WithReference(sql);

builder.AddProject<Projects.Todo_Web>(Constants.AspireResources.Frontend)
    .WithReference(apiService);

builder.Build().Run();
