using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Todo.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Todo.Data.Interceptors;

public class IAuditableInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext context)
    {
        var clock = context.GetService<IClock>();
        var auditableEntities = context.ChangeTracker.Entries<IAuditable>().ToList();
        var now = clock.UtcNow;

        foreach (EntityEntry<IAuditable> entry in auditableEntities)
        {
            if (entry.State == EntityState.Added)
            {
                SetCurrentPropertyValue(
                    entry, nameof(IAuditable.CreatedAt), now);
            }

            if (entry.State == EntityState.Modified)
            {
                SetCurrentPropertyValue(
                    entry, nameof(IAuditable.ModifiedAt), now);
            }
        }

        static void SetCurrentPropertyValue(
        EntityEntry entry,
        string propertyName,
        DateTimeOffset utcNow) =>
        entry.Property(propertyName).CurrentValue = utcNow;
    }
}
