using Microsoft.EntityFrameworkCore;
using Todo.Data.Records;

namespace Todo.Data.DbContexts;
public class TodoDbContext : DbContext
{
    public DbSet<TodoRecord> Todos { get; set; }
    public DbSet<TodoListRecord> TodoLists { get; set; }

    public TodoDbContext() { }

    public TodoDbContext(DbContextOptions Options)
        : base(Options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer();
        }
    }
}
