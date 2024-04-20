using Azure.Core;
using Azure.Identity;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("Todo.Api.Tests")]
namespace Todo.Client;

public partial class TodoClient
{
    public string? AuthScope { get; set; } = null;

    internal Guid? PlayerId { get; set; }

    private AccessToken? _accessToken = null;
    private readonly object _tokenLock = new object();

    internal TodoClient WithToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Remove("Authorization");

        if (token != null)
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        return this;
    }

    internal TodoClient WithHttpClient(HttpClient client)
    {
        _httpClient = client;
        return this;
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    {
        if (AuthScope == null || _httpClient.DefaultRequestHeaders.Authorization is not null)
            return;

        if (_accessToken == null || _accessToken.Value.ExpiresOn < DateTimeOffset.UtcNow)
        {
            lock (_tokenLock)
            {
                if (_accessToken == null || _accessToken.Value.ExpiresOn < DateTimeOffset.UtcNow)
                {
                    var credential = new DefaultAzureCredential();
                    _accessToken = credential.GetToken(new TokenRequestContext(new[]
                    {
                   AuthScope
                }));
                }
            }
        }

        request.Headers.Add("Authorization", $"Bearer {_accessToken.Value.Token}");
    }

    partial void UpdateJsonSerializerSettings(System.Text.Json.JsonSerializerOptions settings)
    {
        settings.Converters.Add(new JsonStringEnumConverter());
        settings.PropertyNameCaseInsensitive = true;
        settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    }
}
