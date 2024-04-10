namespace Todo.Data;

public interface IDeleteable : IAuditable
{
    DateTimeOffset? DeletedAt { get; set; }

    public bool IsDeleted => DeletedAt.HasValue;
}

