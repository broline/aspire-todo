using System;
using System.ComponentModel.DataAnnotations;

namespace Todo.Abstractions.Requests;

public class CreateTodoRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public Guid TodoListId { get; set; } = default;
}
