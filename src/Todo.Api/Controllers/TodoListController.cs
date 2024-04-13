using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Todo.Api.Mappings;
using Todo.Common;
using Todo.Data.DbContexts;

namespace Todo.Api.Controllers;

[ApiController]
[Route("/todo-lists")]
public class TodoListController : ControllerBase
{
    readonly ILogger<TodoController> _logger;
    readonly TodoDbContext _db;
    readonly IClock _clock;

    public TodoListController(ILogger<TodoController> logger, TodoDbContext dbContext, IClock clock)
    {
        _logger = logger;
        _db = dbContext;
        _clock = clock;
    }

    [HttpPost(Name = nameof(CreateTodoList))]
    [ProducesResponseType<TodoList>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IResult> CreateTodoList(CreateTodoListRequest request)
    {
        if (_db.TodoLists.Any(t => t.Name == request.Name))
            return Results.Conflict(new ErrorResponse($"Todo list name must be unique"));

        var todoList = await _db.TodoLists.AddAsync(request.ToRecord());

        await _db.SaveChangesAsync();

        return Results.Ok(todoList.Entity.ToAbstraction());
    }

    [HttpPatch("{todoListId}", Name = nameof(UpdateTodoList))]
    [ProducesResponseType<TodoList>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IResult> UpdateTodoList(Guid todoListId, UpdateTodoListRequest request)
    {
        var todoList = await _db.TodoLists.FindAsync(todoListId);

        if (todoList is null)
            return Results.NotFound(new ErrorResponse("Todo list not found"));

        if (todoList.DeletedAt is not null)
            return Results.Conflict(new ErrorResponse("Todo list has been deleted"));

        if (request.Name is not null)
        {
            if (_db.TodoLists.Any(t => t.Name == request.Name))
                return Results.Conflict(new ErrorResponse($"Todo list name must be unique"));

            todoList.Name = request.Name;
        }

        await _db.SaveChangesAsync();

        return Results.Ok(todoList.ToAbstraction());
    }

    [HttpGet(Name = nameof(GetTodoLists))]
    [ProducesResponseType<IEnumerable<TodoList>>(StatusCodes.Status200OK)]
    public IResult GetTodoLists()
    {
        var todoLists = _db.TodoLists.AsNoTracking()
            .Include(l => l.Todos.Where(t => t.DeletedAt == null).OrderBy(t => t.CreatedAt))
            .Where(l => l.DeletedAt == null)
            .OrderByDescending(l => l.CreatedAt);

        return Results.Ok(todoLists.Select(x => x.ToAbstraction()));
    }

    [HttpDelete("{todoListId}", Name = nameof(DeleteTodoList))]
    [ProducesResponseType<TodoList>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IResult> DeleteTodoList(Guid todoListId)
    {
        var todoList = await _db.TodoLists.FindAsync(todoListId);

        if (todoList is null)
            return Results.NotFound(new ErrorResponse("Todo not found"));

        if (todoList.DeletedAt is not null)
            return Results.Conflict(new ErrorResponse("Todo already deleted"));

        todoList.DeletedAt = _clock.UtcNow;

        await _db.SaveChangesAsync();

        return Results.Ok(todoList.ToAbstraction());
    }
}
