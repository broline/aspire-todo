using Microsoft.AspNetCore.Mvc;
using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Todo.Api.Mappings;
using Todo.Common;
using Todo.Data.DbContexts;

namespace Todo.Api.Endpoints;

public static class TodoEndpoints
{
    public static void UseTodoEndpoints(this WebApplication app)
    {
        app.MapPost("/todos", async (
            CreateTodoRequest request,
            [FromServices] TodoDbContext db) =>
        {
            if (await db.TodoLists.FindAsync(request.TodoListId) is null)
                return Results.BadRequest(new ErrorResponse("Invalid todo list"));

            var todo = await db.Todos.AddAsync(request.ToRecord());

            await db.SaveChangesAsync();

            return Results.Ok(todo.Entity.ToAbstraction());
        })
            .WithName("CreateTodo")
            .Produces<Abstractions.TodoItem>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        app.MapPatch("/todos/{todoId}", async (
            Guid todoId,
            UpdateTodoRequest request,
            [FromServices] TodoDbContext db,
            [FromServices] IClock clock) =>
        {
            var todo = await db.Todos.FindAsync(todoId);

            if (todo is null)
                return Results.NotFound(new ErrorResponse("Todo not found"));

            if (todo.DeletedAt is not null)
                return Results.Conflict(new ErrorResponse("Todo has been deleted"));

            if (request.Name is not null)
                todo.Name = request.Name;

            if (request.Description is not null)
                todo.Description = request.Description;

            if (request.IsCompleted is true && todo.CompletedAt is null)
            {
                todo.CompletedAt = clock.UtcNow;
            }

            if (request.IsCompleted is false && todo.CompletedAt is not null)
            {
                todo.CompletedAt = null;
            }

            await db.SaveChangesAsync();

            return Results.Ok(todo.ToAbstraction());
        })
            .WithName("UpdateTodo")
            .Produces<Abstractions.TodoItem>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        app.MapGet("/todos/{todoId}", async (
            Guid todoId,
            [FromServices] TodoDbContext db) =>
        {
            var todo = await db.Todos.FindAsync(todoId);

            if (todo is null)
                return Results.NotFound(new ErrorResponse("Todo not found"));

            return Results.Ok(todo.ToAbstraction());
        })
            .WithName("GetTodo")
            .Produces<Abstractions.TodoItem>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        app.MapDelete("/todos/{todoId}", async (
            Guid todoId,
            [FromServices] TodoDbContext db,
            [FromServices] IClock clock) =>
        {
            var todo = await db.Todos.FindAsync(todoId);

            if (todo is null)
                return Results.NotFound(new ErrorResponse("Todo not found"));

            if (todo.DeletedAt is not null)
                return Results.Conflict(new ErrorResponse("Todo already deleted"));

            todo.DeletedAt = clock.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(todo.ToAbstraction());
        })
            .WithName("DeleteTodo")
            .Produces<Abstractions.TodoItem>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}
