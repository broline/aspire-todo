using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using Todo.Api.Tests.Fixtures;
using Todo.Client;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;

[Trait("Category", "Todo")]
[Trait("Scenario", "GetTodo")]
[Collection(nameof(ApiCollection))]
public class GetTodoTests : ApiTest
{
    public GetTodoTests(ApiFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    public async Task WithInvalidId_ReturnsNotFound()
    {
        // ARRANGE

        // ACT
        var response = await _fixture.Client.Invoking(c => c.GetTodoAsync(Guid.NewGuid()))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task WithValidId_ReturnsOk()
    {
        // ARRANGE
        var createdTodo = await _fixture.Setup.CreateTodo();

        // ACT
        var todo = await _fixture.Client.GetTodoAsync(createdTodo.Id);

        // ASSERT
        todo.Should().BeEquivalentTo(createdTodo);
    }
}