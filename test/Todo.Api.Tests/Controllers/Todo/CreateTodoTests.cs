using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Todo.Api.Tests.Fixtures;
using Todo.Client;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;

[Trait("Category", "Todo")]
[Trait("Scenario", "CreateTodo")]
[Collection(nameof(ApiCollection))]
public class CreateTodoTests : ApiTest
{
    public CreateTodoTests(ApiFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

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
        var list = await _fixture.Setup.CreateTodoList();

        // ARRANGE
        var request = _fixture.Setup.CreateTodoRequest(list.Id);
        request.Name = "";

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
        var list = await _fixture.Setup.CreateTodoList();

        var request = _fixture.Setup.CreateTodoRequest(list.Id);

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