using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Todo.Api.Tests.Fixtures;
using Todo.Client;
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
    public async Task WithValidRequest_ReturnsOk()
    {
        // ARRANGE
        var request = new CreateTodoListRequest()
        {
            Name = "New list"
        };

        // ACT
        var list = await _fixture.Client.CreateTodoListAsync(request);

        // ASSERT
        var expected = new TodoList
        {
            Name = request.Name,
            CreatedAt = _fixture.Clock.UtcNow
        };

        list.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.Id));

        list.Id.Should().NotBeEmpty();

        var lists = await _fixture.Client.GetTodoListsAsync();

        lists.Should().HaveCount(1)
            .And.Subject.First().Should()
            .BeEquivalentTo(expected, opt => opt.Excluding(x => x.Id));
    }

    [Fact]
    public async Task WithInvalidName_ReturnsBadRequest()
    {
        // ARRANGE
        var request = _fixture.Setup.CreateTodoListRequest;
        request.Name = "";

        // ACT
        var response = await _fixture.Client.Invoking(c => c.CreateTodoListAsync(request))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task WithSameNameAsExisting_ReturnsOk()
    {
        // ARRANGE
        var existing = await _fixture.Setup.CreateTodoList();

        var request = _fixture.Setup.CreateTodoListRequest;
        request.Name = existing.Name;

        // ACT
        var response = await _fixture.Client.CreateTodoListAsync(request);
    }
}