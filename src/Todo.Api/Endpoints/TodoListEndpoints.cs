using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Abstractions;
using Todo.Abstractions.Requests;
using Todo.Api.Mappings;
using Todo.Common;
using Todo.Data.DbContexts;

namespace Todo.Api.Endpoints;

public static class TodoListEndpoints
{
    public static void UseTodoListEndpoints(this WebApplication app)
    {
        app.MapPost("/todo-lists", async (
            CreateTodoListRequest request,
            [FromServices] TodoDbContext db) =>
        {
            var todoList = await db.TodoLists.AddAsync(request.ToRecord());

            await db.SaveChangesAsync();

            return Results.Ok(todoList.Entity.ToAbstraction());
        })
            .WithName("CreateTodoList")
            .Produces<TodoList>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        app.MapPatch("/todo-lists/{todoListId}", async (
            Guid todoListId,
            UpdateTodoListRequest request,
            [FromServices] TodoDbContext db,
            [FromServices] IClock clock) =>
        {
            var todoList = await db.TodoLists.FindAsync(todoListId);

            if (todoList is null)
                return Results.NotFound(new ErrorResponse("Todo list not found"));

            if (todoList.DeletedAt is not null)
                return Results.Conflict(new ErrorResponse("Todo list has been deleted"));

            if (request.Name is not null)
                todoList.Name = request.Name;

            await db.SaveChangesAsync();

            return Results.Ok(todoList.ToAbstraction());
        })
            .WithName("UpdateTodoList")
            .Produces<TodoList>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        app.MapGet("/todo-lists", async (
            [FromServices] TodoDbContext db) =>
        {
            var todoLists = await db.TodoLists.AsNoTracking().ToListAsync();

            return Results.Ok(todoLists.Select(x => x.ToAbstraction()));
        })
            .WithName("GetTodoLists")
            .Produces<IEnumerable<TodoList>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        app.MapDelete("/todo-lists/{todoListId}", async (
            Guid todoListId,
            [FromServices] TodoDbContext db,
            [FromServices] IClock clock) =>
        {
            var todoList = await db.TodoLists.FindAsync(todoListId);

            if (todoList is null)
                return Results.NotFound(new ErrorResponse("Todo not found"));

            if (todoList.DeletedAt is not null)
                return Results.Conflict(new ErrorResponse("Todo already deleted"));

            todoList.DeletedAt = clock.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(todoList.ToAbstraction());
        })
            .WithName("DeleteTodoList")
            .Produces<TodoList>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}
