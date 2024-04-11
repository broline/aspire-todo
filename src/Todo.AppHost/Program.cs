using Microsoft.AspNetCore.Builder;
using Todo.Common;

var builder = Todo.AppHost.ApplicationBuilder.CreateBuilder(args);

builder.Build().Run();
