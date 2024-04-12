using FluentAssertions;
using Todo.Abstractions.Requests;
using Todo.Api.Tests.Fixtures;
using Todo.Client;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;
[Trait("Category", "TodoEndpoints")]
[Trait("Scenario", "CreateTodo")]
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
}