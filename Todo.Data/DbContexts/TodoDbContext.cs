using Microsoft.EntityFrameworkCore;
using Todo.Data.Records;

namespace Todo.Data.DbContexts;
public class TodoDbContext : DbContext
{
    public DbSet<TodoRecord> TodoRecords { get; set; }

    public TodoDbContext(DbContextOptions Options)
        : base(Options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
        }
    }
}
