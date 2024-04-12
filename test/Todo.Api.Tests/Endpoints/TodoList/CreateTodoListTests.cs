using FluentAssertions;
using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;
[Trait("Category", "TodoListEndpoints")]
[Trait("Scenario", "CreateTodoList")]
public class CreateTodoListTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _fixture;

    public CreateTodoListTests(ApiFixture fixture, ITestOutputHelper output)
    { 
        _fixture = fixture;
    }

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