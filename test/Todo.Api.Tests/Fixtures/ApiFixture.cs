using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using System.Data;
using Testcontainers.MsSql;
using Todo.Client;
using Todo.Common;
using Todo.Api;
using Todo.Api.Tests.Security;
using Todo.Data;
using Todo.Api.Tests.Setup;
using Todo.Data.DbContexts;

namespace AR.Events.Api.Tests.Fixtures;
public class ApiFixture : IAsyncLifetime, ITestOutputHelperAccessor
{
    private const string API_URL = "http://localhost";
    private const int SQL_PORT = 60122;

    private IConfigurationRoot? _config;

    private readonly MsSqlContainer _sqlContainer;

    private string _sqlConnectionString = string.Empty;

    public TestServer Server { get; private set; } = default!;
    public ITestOutputHelper? OutputHelper { get; set; }
    public HttpClient HttpClient { get; private set; } = new();
    public IServiceProvider Services => Server.Services;
    public TodoClient Client => (Services.GetRequiredService<ITodoClient>() as TodoClient)!;
    public FrozenClock Clock => (Services.GetRequiredService<IClock>() as FrozenClock)!;
    public Setup Setup => Services.GetRequiredService<Setup>();
    public TodoDbContext TodoDbContext => Services.GetRequiredService<TodoDbContext>();
    public IDbConnection GetDbConnection() => new SqlConnection(_sqlConnectionString);

    public ApiFixture()
    {
        _sqlContainer = new MsSqlBuilder()
            .WithPassword("MyStrongPassword!")
            .WithExposedPort(SQL_PORT)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        _sqlConnectionString = new SqlConnectionStringBuilder(_sqlContainer.GetConnectionString())
        {
            InitialCatalog = Constants.Database.Name
        }.ToString();

        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddInMemoryCollection(new Dictionary<string, string> {
                { "BearerToken:Authority", "https://sts.windows.net/5739346e-a270-44d6-a123-50baff1a4fe5/" },
                { "BearerToken:Audience", "64085ae8-99b6-4dd0-870f-81da1b7f3071" },
            }!)
            .Build();

        Server = new TestServer(new WebHostBuilder()
                .UseConfiguration(_config)
                .ConfigureLogging(l => l.AddXUnit(this).SetMinimumLevel(LogLevel.Warning))
                .UseStartup<ApiStartup>()
                .ConfigureTestServices(services =>
                {
                    services.AddTodoApiClient(new TodoApiClientConfiguration() { BaseUrl = API_URL });
                    services.AddSingleton<Setup>();

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

                    services.AddDb(_sqlConnectionString);

                    services.AddFrozenClock();
                })
                .UseEnvironment(Constants.EnvironmentNames.Test));

        HttpClient = Server.CreateClient();

        Client.WithHttpClient(HttpClient);
    }

    public async Task DisposeAsync()
    {
        Server?.Dispose();
        HttpClient?.Dispose();

        if (_sqlContainer is not null)
            await _sqlContainer.DisposeAsync().AsTask();
    }
}