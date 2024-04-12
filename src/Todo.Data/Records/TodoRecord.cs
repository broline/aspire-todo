using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Todo.Data.Records;

public class TodoRecord : IDeleteable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid TodoListId { get; set; } = new();
    public TodoListRecord TodoList { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}

public class TodoRecordConfiguration : IEntityTypeConfiguration<TodoRecord>
{
    public void Configure(EntityTypeBuilder<TodoRecord> builder)
    {
        builder.ToTable("Todo");

        builder.HasKey(t => t.Id)
            .IsClustered();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}