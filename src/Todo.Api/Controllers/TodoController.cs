using Microsoft.AspNetCore.Mvc;
using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Todo.Api.Mappings;
using Todo.Common;
using Todo.Data.DbContexts;

namespace Todo.Api.Controllers;

[ApiController]
[Route("/todos")]
public class TodoController : ControllerBase
{
    readonly ILogger<TodoController> _logger;
    readonly TodoDbContext _db;
    readonly IClock _clock;

    public TodoController(ILogger<TodoController> logger, TodoDbContext dbContext, IClock clock)
    {
        _logger = logger;
        _db = dbContext;
        _clock = clock;
    }

    [HttpPost(Name = nameof(CreateTodo))]
    [ProducesResponseType<TodoItem>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IResult> CreateTodo(CreateTodoRequest request)
    {
        if (request.TodoListId == Guid.Empty || await _db.TodoLists.FindAsync(request.TodoListId) is null)
            return Results.BadRequest(new ErrorResponse("Invalid todo list"));

        if (_db.Todos.Any(t => t.Name == request.Name && t.TodoListId == request.TodoListId))
            return Results.Conflict(new ErrorResponse($"Todo name must be unique in list"));

        var todo = await _db.Todos.AddAsync(request.ToRecord());

        await _db.SaveChangesAsync();

        return Results.Ok(todo.Entity.ToAbstraction());
    }

    [HttpPatch("{todoId}", Name = nameof(UpdateTodo))]
    [ProducesResponseType<TodoItem>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateTodo(Guid todoId, UpdateTodoRequest request)
    {
        var todo = await _db.Todos.FindAsync(todoId);

        if (todo is null)
            return Results.NotFound(new ErrorResponse("Todo not found"));

        if (todo.DeletedAt is not null)
            return Results.Conflict(new ErrorResponse("Todo has been deleted"));

        if (request.Name is not null)
        {
            if (_db.Todos.Any(t => t.Name == request.Name && t.TodoListId == todo.TodoListId))
                return Results.Conflict(new ErrorResponse($"Todo name must be unique in list"));

            todo.Name = request.Name;
        }

        if (request.Description is not null)
            todo.Description = request.Description;

        if (request.IsCompleted is true && todo.CompletedAt is null)
            todo.CompletedAt = _clock.UtcNow;

        if (request.IsCompleted is false && todo.CompletedAt is not null)
            todo.CompletedAt = null;

        await _db.SaveChangesAsync();

        return Results.Ok(todo.ToAbstraction());
    }

    [HttpGet("{todoId}", Name = nameof(GetTodo))]
    [ProducesResponseType<TodoItem>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetTodo(Guid todoId)
    {
        var todo = await _db.Todos.FindAsync(todoId);

        if (todo is null)
            return Results.NotFound(new ErrorResponse("Todo not found"));

        return Results.Ok(todo.ToAbstraction());
    }

    [HttpDelete("{todoId}", Name = nameof(DeleteTodo))]
    [ProducesResponseType<TodoItem>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IResult> DeleteTodo(Guid todoId)
    {
        var todo = await _db.Todos.FindAsync(todoId);

        if (todo is null)
            return Results.NotFound(new ErrorResponse("Todo not found"));

        if (todo.DeletedAt is not null)
            return Results.Conflict(new ErrorResponse("Todo already deleted"));

        todo.DeletedAt = _clock.UtcNow;

        await _db.SaveChangesAsync();

        return Results.Ok(todo.ToAbstraction());
    }
}
