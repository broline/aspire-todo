using AR.Events.Api.Tests.Fixtures;
using FluentAssertions;
using Todo.Abstractions;
using Todo.Api.Tests.Fixtures;
using Xunit.Abstractions;

namespace Todo.Api.Tests.Endpoints.Todo;

[Trait("Category", "TodoList")]
[Trait("Scenario", "GetTodo")]
[Collection(nameof(ApiCollection))]
public class GetTodoListsTests : ApiTest
{
    public GetTodoListsTests(ApiFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    public async Task WithNoLists_ReturnsOk()
    {
        // ARRANGE

        // ACT
        var lists = await _fixture.Client.GetTodoListsAsync();

        // ASSERT
        lists.Should().BeEmpty();
    }

    [Fact]
    public async Task WithLists_ReturnsOk()
    {
        // ARRANGE
        var list1 = await _fixture.Setup.CreateTodoList();
        _fixture.Clock.AdvanceMinutes(10);
        var list2 = await _fixture.Setup.CreateTodoList(l => { l.Name = "2"; });

        // ACT
        var lists = await _fixture.Client.GetTodoListsAsync();

        // ASSERT
        lists.Should().HaveCount(2)
            .And.Subject
            .Should().BeEquivalentTo(new List<TodoList> { list2, list1 });
    }

    [Fact]
    public async Task WithListsHavingTodoItems_ReturnsOk()
    {
        // ARRANGE
        var todo1 = await _fixture.Setup.CreateTodo();
        _fixture.Clock.AdvanceMinutes(10);
        var todo2 = await _fixture.Setup.CreateTodo(todo1.TodoListId, t => { t.Name = "2"; });

        // ACT
        var lists = await _fixture.Client.GetTodoListsAsync();

        // ASSERT
        lists.Should().HaveCount(1)
            .And.Subject.First().Todos
            .Should().HaveCount(2)
            .And.Subject
            .Should().BeEquivalentTo(new List<TodoItem> { todo1, todo2});

    }

    [Fact]
    public async Task WithListsHavingDeletedTodoItems_ReturnsOk()
    {
        // ARRANGE
        var todo1 = await _fixture.Setup.CreateTodo();
        _fixture.Clock.AdvanceMinutes(10);
        var todo2 = await _fixture.Setup.CreateTodo(todo1.TodoListId, t => { t.Name = "2"; });
        _fixture.Clock.AdvanceMinutes(10);
        await _fixture.Setup.DeleteTodo(todo1.Id);

        // ACT
        var lists = await _fixture.Client.GetTodoListsAsync();

        // ASSERT
        lists.Should().HaveCount(1)
            .And.Subject.First().Todos
            .Should().HaveCount(1)
            .And.Subject
            .Should().BeEquivalentTo(new List<TodoItem> { todo2 });
    }
}