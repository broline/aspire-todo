using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Todo.Data.Records;


public class TodoListRecord : IDeleteable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<TodoRecord> Todos { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

public class TodoListRecordConfiguration : IEntityTypeConfiguration<TodoListRecord>
{
    public void Configure(EntityTypeBuilder<TodoListRecord> builder)
    {
        builder.ToTable("TodoList");

        builder.HasKey(t => t.Id)    
            .IsClustered();

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