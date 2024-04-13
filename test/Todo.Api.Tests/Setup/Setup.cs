using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Todo.Client;
using Todo.Data.DbContexts;

namespace Todo.Api.Tests.Setup;

public class Setup
{
    readonly TodoDbContext _db;
    readonly ITodoClient _client;

    public Setup(TodoDbContext db, ITodoClient client)
    {
        _db = db;
        _client = client;
    }

    public CreateTodoListRequest CreateTodoListRequest => new()
    {
        Name = "Test Todo List"
    };

    public CreateTodoRequest CreateTodoRequest(Guid? todoListId = null) => new()
    {
        Name = "Test Todo",
        Description = "Some Description",
        TodoListId = todoListId ?? Guid.Empty
    };

    public Task<TodoList> CreateTodoList(Action<CreateTodoListRequest>? requestAction = null)
    {
        var request = CreateTodoListRequest;
        requestAction?.Invoke(request);

        return _client.CreateTodoListAsync(request);
    }

    public Task<TodoItem> CreateTodo(Guid? todoListId = null, Action<CreateTodoRequest>? requestAction = null)
    {
        var request = CreateTodoRequest(todoListId);
        requestAction?.Invoke(request);

        return _client.CreateTodoAsync(request);
    }
}
