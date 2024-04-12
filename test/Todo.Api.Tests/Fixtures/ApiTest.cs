using FluentAssertions;
using Todo.Api.Tests.Security;
using Todo.Common;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Fixtures;

public class ApiTest : IClassFixture<ApiFixture>, IAsyncLifetime
{
    protected readonly ApiFixture _fixture;
    public TestJwtToken Token { get; set; } = new();

    public ApiTest(ApiFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.OutputHelper = output;

        AssertionOptions.AssertEquivalencyUsing(options =>
        {
            options
            .Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
            .WhenTypeIs<DateTimeOffset>();
            return options;
        });
    }

    private Task BeforeEach()
    {
        _fixture.Client.WithHttpClient(_fixture.CreateClient());

        SetRoles(Constants.Roles.User);

        ConfigureToken(t => t.WithUserName("Test User")
        .WithEmail("TestUser@todoapp.com"));

        _fixture.Clock.SetUtcNow(DateTimeOffset.UtcNow);

        // TODO reset database
        return Task.CompletedTask;
    }

    public TestJwtToken ConfigureToken(Action<TestJwtToken> configure)
    {
        Token ??= new TestJwtToken();

        configure(Token);
        var token = Token.Build();
        _fixture.Client.WithToken(token);

        return Token;
    }

    public void SetRoles(params string[] roles)
    {
        ConfigureToken(t => t.WithRoles(roles));
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await BeforeEach();
    }
}
