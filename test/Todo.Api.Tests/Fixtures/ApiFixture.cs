using MartinCostello.Logging.XUnit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;
using Todo.Common;
using Todo.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Todo.Api.Tests.Security;

namespace Todo.Api.Tests;

public class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime, ITestOutputHelperAccessor
{
    private IHost? _app;
    public IResourceBuilder<SqlServerDatabaseResource> _sql { get; private set; }

    public FrozenClock Clock => (Services.GetRequiredService<IClock>() as FrozenClock)!;
    public TodoClient Client => (Services.GetRequiredService<ITodoClient>() as TodoClient)!;
    public ITestOutputHelper? OutputHelper { get; set; }

    public ApiFixture()
    {
        var options = new DistributedApplicationOptions
        {
            AssemblyName = typeof(ApiFixture).Assembly.FullName,
            DisableDashboard = false,
        };
        var builder = DistributedApplication.CreateBuilder(options);

        _sql = builder.AddSqlServer(Constants.AspireResources.Sql)
            .AddDatabase(Constants.Database.Name);

        builder.AddProject<Projects.Todo_Api>(Constants.AspireResources.Api)
            .WithReference(_sql);

        _app = builder.Build();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "BearerToken:Authority", "https://sts.windows.net/5739346e-a270-44d6-a123-50baff1a4fe5/" },
                { "BearerToken:Audience", "64085ae8-99b6-4dd0-870f-81da1b7f3071" },
                { $"ConnectionStrings:{_sql.Resource.Name}", _sql.Resource.GetConnectionString() }
            });
        });

        builder.ConfigureServices(services =>
        {
            services.Configure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Configuration = new OpenIdConnectConfiguration
                        {
                            Issuer = JwtTokenProvider.Issuer,
                        };
                        options.TokenValidationParameters.ValidIssuer = JwtTokenProvider.Issuer;
                        options.TokenValidationParameters.ValidAudience = JwtTokenProvider.Issuer;
                        options.Configuration.SigningKeys.Add(JwtTokenProvider.SecurityKey);
                    }
            );

            services.AddTodoApiClient(new TodoApiClientConfiguration { BaseUrl = $"http://{Constants.AspireResources.Api}" });

            services.AddFrozenClock();
        });

        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _app!.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _app!.StopAsync();
        if (_app is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            _app!.Dispose();
        }
    }
}