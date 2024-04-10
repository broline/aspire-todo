namespace Todo.Data;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset? ModifiedAt { get; set; }
}