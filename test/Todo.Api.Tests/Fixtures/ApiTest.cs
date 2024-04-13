using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using static Todo.Common.Constants;
using Todo.Api.Tests.Security;
using Xunit.Abstractions;
using Todo.Data;

namespace Todo.Api.Tests.Fixtures;
public class ApiTest : IAsyncLifetime
{
    protected readonly ApiFixture _fixture;
    public TestJwtToken Token { get; set; } = new();
    public Guid? PlayerId { get; private set; }

    public ApiTest(ApiFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.OutputHelper = output;

        AssertionOptions.AssertEquivalencyUsing(options =>
        {
            options.Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(100)))
                .WhenTypeIs<DateTimeOffset>();

            return options;
        });
    }

    private async Task BeforeEach()
    {
        var dbReset = _fixture.TodoDbContext.ResetDatabase();
        SetRoles(Roles.User);

        ConfigureToken(t => t.WithUserName("Test User")
        .WithEmail("TestUser@atlasreality.com"));

        _fixture.Clock.SetUtcNow(DateTimeOffset.UtcNow);

        await dbReset;
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
