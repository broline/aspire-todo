using Todo.Web;
using Todo.Web.Components;
using Todo.Common;
using Todo.Client;
using MudBlazor.Services;
using Todo.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddMudServices();

builder.Services.AddTodoApiClient(new() { BaseUrl = $"http://{Constants.AspireResources.Api}" });

builder.Services.AddScoped<ILoadingService, LoadingService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();

app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
