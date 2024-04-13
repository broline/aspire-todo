using System;

namespace Todo.Abstractions;

public class TodoItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid TodoListId { get; set; } = default;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
