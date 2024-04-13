using System;
using System.ComponentModel.DataAnnotations;

namespace Todo.Abstractions.Requests;

public class UpdateTodoRequest
{
    [StringLength(100, MinimumLength = 1)]
    public string? Name { get; set; } = null;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public bool? IsCompleted { get; set; }
}
