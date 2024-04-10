using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Data.Records;

namespace Todo.Data;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset? ModifiedAt { get; }
}