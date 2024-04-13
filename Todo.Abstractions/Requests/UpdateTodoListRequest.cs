using System.ComponentModel.DataAnnotations;

namespace Todo.Abstractions.Requests;

public class UpdateTodoListRequest
{
    [StringLength(100, MinimumLength = 1)]
    public string? Name { get; set; } = null;
}
