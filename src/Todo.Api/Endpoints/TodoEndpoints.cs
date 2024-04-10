using Microsoft.AspNetCore.Mvc;
using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Todo.Api.Mappings;
using Todo.Data.DbContexts;

namespace Todo.Api.Endpoints;

public static class TodoEndpoints
{
    public static void UseTodoEndpoints(this WebApplication app)
    {
        app.MapPost("/todos", async (
            CreateTodoRequest request,
            TodoDbContext db) =>
        {
            if (await db.TodoLists.FindAsync(request.TodoListId) is null)
                return Results.BadRequest(new ErrorResponse("Invalid todo list"));

            var todo = await db.Todos.AddAsync(request.ToRecord());

            return Results.Ok(todo.Entity.ToAbstraction());
        })
            .WithName("CreateTodo")
            .Produces<Abstractions.Todo>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        app.MapGet("/todos/{todoId}", async (
            Guid todoId,
            TodoDbContext db) =>
        {
            var todo = await db.Todos.FindAsync(todoId);

            if (todo is null)
                return Results.NotFound(new ErrorResponse("Todo not found"));

            return Results.Ok(todo.ToAbstraction());
        })
            .WithName("GetTodo")
            .Produces<Abstractions.Todo>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}
