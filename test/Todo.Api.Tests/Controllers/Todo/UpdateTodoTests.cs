using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using Todo.Abstractions.Requests;
using Todo.Api.Tests.Fixtures;
using Todo.Client;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;

[Trait("Category", "Todo")]
[Trait("Scenario", "UpdateTodo")]
[Collection(nameof(ApiCollection))]
public class UpdateTodoTests : ApiTest
{
    public UpdateTodoTests(ApiFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    public async Task WithInvalidId_ReturnsNotFound()
    {
        // ARRANGE

        // ACT
        var response = await _fixture.Client.Invoking(c => c.UpdateTodoAsync(Guid.NewGuid(), new()))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task WithInvalidName_ReturnsBadRequest()
    {
        // ARRANGE
        var createdTodo = await _fixture.Setup.CreateTodo();

        var request = new UpdateTodoRequest
        {
            Name = ""
        };

        // ACT
        var response = await _fixture.Client.Invoking(c => c.UpdateTodoAsync(createdTodo.Id, request))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task WithUpdatedName_ReturnsOk()
    {
        // ARRANGE
        var createdTodo = await _fixture.Setup.CreateTodo();

        var request = new UpdateTodoRequest
        {
            Name = "New name"
        };

        var modifiedAt = _fixture.Clock.AdvanceMinutes(2);

        // ACT
        var updated = await _fixture.Client.UpdateTodoAsync(createdTodo.Id, request);

        // ASSERT
        var expected = createdTodo;
        expected.Name = request.Name;
        expected.ModifiedAt = modifiedAt;

        updated.Should().BeEquivalentTo(expected);

        var todo = await _fixture.Client.GetTodoAsync(updated.Id);
        todo.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task WithUpdatedDescription_ReturnsOk()
    {
        // ARRANGE
        var createdTodo = await _fixture.Setup.CreateTodo();

        var request = new UpdateTodoRequest
        {
            Description = "New desc"
        };

        var modifiedAt = _fixture.Clock.AdvanceMinutes(2);

        // ACT
        var updated = await _fixture.Client.UpdateTodoAsync(createdTodo.Id, request);

        // ASSERT
        var expected = createdTodo;
        expected.Description = request.Description;
        expected.ModifiedAt = modifiedAt;

        updated.Should().BeEquivalentTo(expected);

        var todo = await _fixture.Client.GetTodoAsync(updated.Id);
        todo.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task WithUpdatedNameAndDescription_ReturnsOk()
    {
        // ARRANGE
        var createdTodo = await _fixture.Setup.CreateTodo();

        var request = new UpdateTodoRequest
        {
            Name = "New name",
            Description = "New desc"
        };

        var modifiedAt = _fixture.Clock.AdvanceMinutes(2);

        // ACT
        var updated = await _fixture.Client.UpdateTodoAsync(createdTodo.Id, request);

        // ASSERT
        var expected = createdTodo;
        expected.Name = request.Name;
        expected.Description = request.Description;
        expected.ModifiedAt = modifiedAt;

        updated.Should().BeEquivalentTo(expected);

        var todo = await _fixture.Client.GetTodoAsync(updated.Id);
        todo.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task WithCompleted_ReturnsOk()
    {
        // ARRANGE
        var createdTodo = await _fixture.Setup.CreateTodo();

        var request = new UpdateTodoRequest
        {
            Name = "New name",
            IsCompleted = true
        };

        var modifiedAt = _fixture.Clock.AdvanceMinutes(2);

        // ACT
        var updated = await _fixture.Client.UpdateTodoAsync(createdTodo.Id, request);

        // ASSERT
        var expected = createdTodo;
        expected.Name = request.Name;
        expected.ModifiedAt = modifiedAt;
        expected.CompletedAt = modifiedAt;

        updated.Should().BeEquivalentTo(expected);

        var todo = await _fixture.Client.GetTodoAsync(updated.Id);
        todo.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task WithSameNameAsExistingInList_ReturnsConflict()
    {
        // ARRANGE
        var createdTodo1 = await _fixture.Setup.CreateTodo(requestAction: t => t.Name = "1");
        var createdTodo2 = await _fixture.Setup.CreateTodo(createdTodo1.TodoListId, t => t.Name = "2");

        var request = new UpdateTodoRequest
        {
            Name = "1"
        };

        var modifiedAt = _fixture.Clock.AdvanceMinutes(2);

        // ACT
        var response = await _fixture.Client.Invoking(c => c.UpdateTodoAsync(createdTodo2.Id, request))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(409);
    }
}