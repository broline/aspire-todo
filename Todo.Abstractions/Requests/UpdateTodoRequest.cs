using System;
using System.ComponentModel.DataAnnotations;

namespace Todo.Abstractions.Requests
{
    public class UpdateTodoRequest
    {
        [MaxLength(100)]
        public string? Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }
    }
}
