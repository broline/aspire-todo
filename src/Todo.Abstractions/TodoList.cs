using System;
using System.Collections.Generic;

namespace Todo.Abstractions;

public class TodoList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<TodoItem> Todos { get; set; } = new List<TodoItem>();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
