using Todo.Abstractions.Requests;
using Todo.Data.Records;

namespace Todo.Api.Mappings;

public static class TodoMappings
{
    public static Abstractions.Todo ToAbstraction(this TodoRecord record)
    {
        return new Abstractions.Todo
        {
            Id = record.Id,
            Name = record.Name,
            Description = record.Description,
            TodoListId = record.TodoListId,
            CreatedAt = record.CreatedAt,
            ModifiedAt = record.ModifiedAt,
            DeletedAt = record.DeletedAt,
            CompletedAt = record.CompletedAt
        };
    }

    public static TodoRecord ToRecord(this CreateTodoRequest request)
    {
        return new TodoRecord
        {
            Name = request.Name,
            Description = request.Description,
            TodoListId = request.TodoListId
        };
    }
}
