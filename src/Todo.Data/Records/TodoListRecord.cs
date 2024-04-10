using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace Todo.Data.Records;


public class TodoListRecord : IDeleteable
{
    public Guid Id { get; }
    public string Name { get; set; } = string.Empty;
    public ICollection<TodoRecord> Todos { get; set; } = new List<TodoRecord>();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

public class TodoListRecordConfiguration : IEntityTypeConfiguration<TodoListRecord>
{
    public void Configure(EntityTypeBuilder<TodoListRecord> builder)
    {
        builder.ToTable("TodoList");

        builder.HasKey(t => t.Id);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(x => x.Todos)
            .WithOne(x => x.TodoList)
            .HasForeignKey(x => x.TodoListId);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}