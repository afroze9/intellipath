using IntelliPath.Orchestrator.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IntelliPath.Orchestrator.Data;

public class AuditableEntitySaveChangesInterceptor()
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        foreach (EntityEntry<EntityBase> entry in context.ChangeTracker.Entries<EntityBase>())
        {
            DateTime currentDateTime = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                // Set created by and created on properties
                entry.Entity.CreatedAt = currentDateTime;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified ||
                HasChangedOwnedEntities(entry))
            {
                // Set modified by and modified on properties
                entry.Entity.ModifiedAt = currentDateTime;
            }
        }
    }

    private static bool HasChangedOwnedEntities(EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}