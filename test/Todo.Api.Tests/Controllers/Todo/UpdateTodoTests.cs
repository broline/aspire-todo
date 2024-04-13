using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using Todo.Abstractions;
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

        var modifiedTime = _fixture.Clock.AdvanceMinutes(2);

        // ACT
        var updated = await _fixture.Client.UpdateTodoAsync(createdTodo.Id, request);

        // ASSERT
        var todo = await _fixture.Client.GetTodoAsync(updated.Id);

        var expected = createdTodo;
        expected.Name = request.Name;
        expected.ModifiedAt = modifiedTime;

        todo.Should().BeEquivalentTo(expected);
    }
}