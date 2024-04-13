using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Todo.Api.Tests.Fixtures;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;

[Trait("Category", "TodoList")]
[Trait("Scenario", "CreateTodoList")]
[Collection(nameof(ApiCollection))]
public class CreateTodoListTests : ApiTest
{
    public CreateTodoListTests(ApiFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    public async Task WithValidRequest_ReturnsNewTodoList()
    {
        // ARRANGE
        var request = new CreateTodoListRequest()
        {
            Name = "New list"
        };

        // ACT
        var list = await _fixture.Client.CreateTodoListAsync(request);

        // ASSERT
        list.Should().BeEquivalentTo(new TodoList
        {
            Name = request.Name,
            CreatedAt = _fixture.Clock.UtcNow,
        }, opt => opt.Excluding(x => x.Id));

        list.Id.Should().NotBeEmpty();
    }
}