using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Todo.Client;

public static class HostingExtensions
{
    public static IServiceCollection AddTodoApiClient(this IServiceCollection services, TodoApiClientConfiguration config, Action<HttpClient>? httpConfig = null)
    {
        services.AddHttpClient();

        services.AddSingleton<ITodoClient, TodoClient>(svc =>
        {
            var httpClient = svc.GetRequiredService<IHttpClientFactory>().CreateClient();
            httpClient.BaseAddress = new Uri(config.BaseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpConfig?.Invoke(httpClient);

            var client = new TodoClient(httpClient);
            client.AuthScope = config.AuthScope;

            return client;
        });

        return services;
    }
}
