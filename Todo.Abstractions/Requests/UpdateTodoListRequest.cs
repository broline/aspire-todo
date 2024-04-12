using System;
using System.ComponentModel.DataAnnotations;

namespace Todo.Abstractions.Requests;

public class UpdateTodoListRequest
{
    [MaxLength(50)]
    public string? Name { get; set; } = string.Empty;
}
