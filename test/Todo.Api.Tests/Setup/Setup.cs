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

    public UpdateTodoListRequest UpdateTodoListRequest => new()
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

    public async Task<TodoItem> CreateTodo(Guid? todoListId = null, Action<CreateTodoRequest>? requestAction = null)
    {
        if (todoListId is null)
            todoListId = (await CreateTodoList()).Id;

        var request = CreateTodoRequest(todoListId);
        requestAction?.Invoke(request);

        return await _client.CreateTodoAsync(request);
    }
    public async Task<TodoList> DeleteTodoList(Guid? todoListId = null)
    {
        if (todoListId is null)
            todoListId = (await CreateTodoList()).Id;

        return await _client.DeleteTodoListAsync(todoListId.Value);
    }

    public async Task<TodoItem> DeleteTodo(Guid? todoId = null)
    {
        if (todoId is null)
            todoId = (await CreateTodo()).Id;

        return await _client.DeleteTodoAsync(todoId.Value);
    }
}
