using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using Todo.Abstractions.Requests;
using Todo.Api.Tests.Fixtures;
using Todo.Client;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;

[Trait("Category", "Todo")]
[Trait("Scenario", "DeleteTodo")]
[Collection(nameof(ApiCollection))]
public class DeleteTodoTests : ApiTest
{
    public DeleteTodoTests(ApiFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    public async Task WithInvalidId_ReturnsNotFound()
    {
        // ARRANGE

        // ACT
        var response = await _fixture.Client.Invoking(c => c.DeleteTodoAsync(Guid.NewGuid()))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task WithValidId_ReturnsOk()
    {
        // ARRANGE
        var createdTodo = await _fixture.Setup.CreateTodo();

        var modifiedTime = _fixture.Clock.AdvanceMinutes(2);

        // ACT
        var deleted = await _fixture.Client.DeleteTodoAsync(createdTodo.Id);

        // ASSERT
        var expected = createdTodo;
        expected.DeletedAt = modifiedTime;
        expected.ModifiedAt = modifiedTime;

        deleted.Should().BeEquivalentTo(expected);

        var todo = await _fixture.Client.GetTodoAsync(deleted.Id);
        todo.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task WithAlreadyDeletedId_ReturnsConflict()
    {
        // ARRANGE
        var deletedTodo = await _fixture.Setup.DeleteTodo();

        // ACT
        var response = await _fixture.Client.Invoking(c => c.DeleteTodoAsync(deletedTodo.Id))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(409);
    }
}