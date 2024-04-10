using Todo.Abstractions.Requests;
using Todo.Data.Records;

namespace Todo.Api.Mappings;

public static class TodoListMappings
{
    public static Abstractions.TodoList ToAbstraction(this TodoListRecord record)
    {
        return new Abstractions.TodoList
        {
            Id = record.Id,
            Name = record.Name,
            Todos = record.Todos.Select(x => x.ToAbstraction()),
            CreatedAt = record.CreatedAt,
            ModifiedAt = record.ModifiedAt,
            DeletedAt = record.DeletedAt
        };
    }

    public static TodoListRecord ToRecord(this CreateTodoListRequest request)
    {
        return new TodoListRecord
        {
            Name = request.Name
        };
    }
}
