using FluentAssertions;
using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Todo.Client;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;
[Trait("Category", "TodoEndpoints")]
[Trait("Scenario", "CreateTodo")]
public class CreateTodoTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _fixture;

    public CreateTodoTests(ApiFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WithInvalidTodoListId_ReturnsBadRequest()
    {
        // ARRANGE
        var request = new CreateTodoRequest()
        {
            Name = "Test todo",
            Description = "Description",
            TodoListId = Guid.NewGuid()
        };

        // ACT
        var response = await _fixture.Client.Invoking(c => c.CreateTodoAsync(request))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task WithInvalidName_ReturnsBadRequest()
    {
        var list = await _fixture.Client.CreateTodoListAsync(new CreateTodoListRequest() { Name = "Test List" });

        // ARRANGE
        var request = new CreateTodoRequest()
        {
            Name = "",
            Description = "Description",
            TodoListId = list.Id
        };

        // ACT
        var response = await _fixture.Client.Invoking(c => c.CreateTodoAsync(request))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task WithValidTodoListId_ReturnsOk()
    {
        // ARRANGE
        var list = await _fixture.Client.CreateTodoListAsync(new CreateTodoListRequest() { Name = "Test List" });

        var request = new CreateTodoRequest()
        {
            Name = "Test todo",
            Description = "Description",
            TodoListId = list.Id
        };

        // ACT
        var todo = await _fixture.Client.CreateTodoAsync(request);

        // ASSERT
        todo.Should().BeEquivalentTo(new TodoItem
        {
            Name = request.Name,
            Description = request.Description,
            TodoListId = request.TodoListId,
            CreatedAt = _fixture.Clock.UtcNow,
        }, opt => opt.Excluding(x => x.Id));

        todo.Id.Should().NotBeEmpty();
    }
}