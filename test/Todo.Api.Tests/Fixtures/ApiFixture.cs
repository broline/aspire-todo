using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;
using Todo.Common;
using Todo.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.Api.Tests;

public class ApiFixture : IAsyncLifetime, ITestOutputHelperAccessor
{
    private IConfigurationRoot? _config;

    private DistributedApplication? _app;
    public ITestOutputHelper? OutputHelper { get; set; }

    public ApiFixture()
    {
        var options = new DistributedApplicationOptions
        {
            AssemblyName = typeof(ApiFixture).Assembly.FullName,
            DisableDashboard = true,

        };
        var builder = AppHost.ApplicationBuilder.CreateBuilder(options, new() { PersistSqlData = false });

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string> {
                { "BearerToken:Authority", "https://sts.windows.net/5739346e-a270-44d6-a123-50baff1a4fe5/" },
                { "BearerToken:Audience", "64085ae8-99b6-4dd0-870f-81da1b7f3071" },
            }!);

        var services = builder.Services;

        services.AddTodoApiClient(new TodoApiClientConfiguration { BaseUrl = $"http://{Constants.AspireResources.Api}" });

        services.AddFrozenClock();

        //services.Configure<JwtBearerOptions>(
        //                    JwtBearerDefaults.AuthenticationScheme,
        //                    options =>
        //                    {
        //                        options.Configuration = new OpenIdConnectConfiguration
        //                        {
        //                            Issuer = JwtTokenProvider.Issuer,
        //                        };
        //                        options.TokenValidationParameters.ValidIssuer = JwtTokenProvider.Issuer;
        //                        options.TokenValidationParameters.ValidAudience = JwtTokenProvider.Issuer;
        //                        options.Configuration.SigningKeys.Add(JwtTokenProvider.SecurityKey);
        //                    }
        //            );

        _app = builder.Build();
    }

    public async Task InitializeAsync()
    {
        await _app!.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_app is not null)
            await _app.DisposeAsync();
    }
}