namespace Todo.Data;

public interface IDeleteable : IAuditable
{
    DateTimeOffset? DeletedAt { get; set; }

    bool IsDeleted => DeletedAt.HasValue;
}

