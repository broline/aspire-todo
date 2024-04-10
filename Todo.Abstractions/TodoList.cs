using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Abstractions
{
    public class TodoList
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IEnumerable<Todo> Todos { get; set; } = new List<Todo>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
