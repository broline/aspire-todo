using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using Todo.Api.Tests.Fixtures;
using Todo.Client;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;

[Trait("Category", "TodoList")]
[Trait("Scenario", "DeleteTodoList")]
[Collection(nameof(ApiCollection))]
public class DeleteTodoListTests : ApiTest
{
    public DeleteTodoListTests(ApiFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    public async Task WithInvalidId_ReturnsNotFound()
    {
        // ARRANGE

        // ACT
        var response = await _fixture.Client.Invoking(c => c.DeleteTodoListAsync(Guid.NewGuid()))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task WithValidId_ReturnsOk()
    {
        // ARRANGE
        var created = await _fixture.Setup.CreateTodoList();

        var modifiedTime = _fixture.Clock.AdvanceMinutes(2);

        // ACT
        var deleted = await _fixture.Client.DeleteTodoListAsync(created.Id);

        // ASSERT
        var expected = created;
        expected.DeletedAt = modifiedTime;
        expected.ModifiedAt = modifiedTime;

        deleted.Should().BeEquivalentTo(expected);

        var todo = await _fixture.Client.GetTodoListsAsync();
        todo.Should().BeEmpty();
    }

    [Fact]
    public async Task WithAlreadyDeletedId_ReturnsConflict()
    {
        // ARRANGE
        var deleted = await _fixture.Setup.DeleteTodoList();

        // ACT
        var response = await _fixture.Client.Invoking(c => c.DeleteTodoListAsync(deleted.Id))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(409);
    }
}