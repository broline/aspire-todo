using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using Todo.Abstractions.Requests;
using Todo.Api.Tests.Fixtures;
using Todo.Client;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;

[Trait("Category", "TodoList")]
[Trait("Scenario", "CreateTodoList")]
[Collection(nameof(ApiCollection))]
public class UpdateTodoListTests : ApiTest
{
    public UpdateTodoListTests(ApiFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    public async Task WithInvalidId_ReturnsNotFound()
    {
        // ARRANGE

        // ACT
        var response = await _fixture.Client.Invoking(c => c.UpdateTodoListAsync(Guid.NewGuid(), new()))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(404);
    }


    [Fact]
    public async Task WithValidRequest_ReturnsOk()
    {
        // ARRANGE
        var existing = await _fixture.Setup.CreateTodoList();

        var request = new UpdateTodoListRequest()
        {
            Name = "New name"
        };

        var modifiedAt = _fixture.Clock.AdvanceMinutes(2);

        // ACT
        var updated = await _fixture.Client.UpdateTodoListAsync(existing.Id, request);

        // ASSERT
        var expected = existing;
        expected.Name = request.Name;
        expected.ModifiedAt = modifiedAt;
        updated.Should().BeEquivalentTo(expected);

        var lists = await _fixture.Client.GetTodoListsAsync();

        lists.Should().HaveCount(1)
            .And.Subject.First().Should()
            .BeEquivalentTo(expected);
    }

    [Fact]
    public async Task WithInvalidName_ReturnsBadRequest()
    {
        // ARRANGE
        var existing = await _fixture.Setup.CreateTodoList();

        var request = _fixture.Setup.UpdateTodoListRequest;
        request.Name = "";

        // ACT
        var response = await _fixture.Client.Invoking(c => c.UpdateTodoListAsync(existing.Id, request))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task WithSameNameAsExisting_ReturnsConflict()
    {
        // ARRANGE
        var existing = await _fixture.Setup.CreateTodoList(t => { t.Name = "1"; });
        var sut = await _fixture.Setup.CreateTodoList(t => { t.Name = "2"; });

        var request = _fixture.Setup.UpdateTodoListRequest;
        request.Name = existing.Name;

        // ACT
        var response = await _fixture.Client.Invoking(c => c.UpdateTodoListAsync(sut.Id, request))
            .Should().ThrowAsync<TodoApiException>();

        // ASSERT
        response.Subject.Single().StatusCode.Should().Be(409);
    }
}