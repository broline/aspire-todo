using System.ComponentModel.DataAnnotations;

namespace Todo.Abstractions.Requests;

public class CreateTodoListRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
