using System.ComponentModel.DataAnnotations;

namespace Todo.Data.Records;

public class TodoRecord
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset CompletedAt { get; set; }
}