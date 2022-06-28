using System.Reflection;
using Devocean.Core.Application.Interfaces;
using Devocean.Core.Domain.common;
using Devocean.Core.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Devocean.Core.Infrastructure.Persistence.Data;

public abstract class DbContextBase<TDbContext> : DbContext, IDbContext
    where TDbContext : DbContext
{
    private readonly IDateTime? _dateTime;
    private readonly string? _schema;

    protected DbContextBase(DbContextOptions<TDbContext> options,
        IDateTime? dateTime = null, string? schema = null) : base(options)
    {
        _dateTime = dateTime ?? new DateTimeService();
        _schema = schema;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var dateProperties = this.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(property => property.ClrType == typeof(DateTime))
            .Select(datetimeProperty => new
            {
                ParentName = datetimeProperty.DeclaringEntityType.Name,
                PropertyName = datetimeProperty.Name
            }).ToList();

        var changedEntities = this.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in changedEntities)
        {
            var entity = entry.Entity;

            if (entity is IAuditableEntity auditableEntity)
            {
                FillAuditData(entry, auditableEntity);
            }

            var entityFields = dateProperties
                .Where(d => d.ParentName == entity.GetType().FullName);

            foreach (var property in entityFields)
            {
                var prop = entity.GetType().GetProperty(property.PropertyName);

                if (prop == null)
                    continue;

                var originalValue = prop.GetValue(entity) as DateTime?;
                if (originalValue == null)
                    continue;

                prop.SetValue(entity, DateTime.SpecifyKind(originalValue.Value, DateTimeKind.Utc));
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    private void FillAuditData(EntityEntry entry, IAuditableEntity entity)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                entity.CreatedBy = null;
                entity.Created = _dateTime.Now;
                break;
            case EntityState.Modified:
                entity.LastModifiedBy = null;
                entity.LastModified = _dateTime.Now;
                break;
            case EntityState.Detached:
                break;
            case EntityState.Unchanged:
                break;
            case EntityState.Deleted:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrEmpty(_schema))
            modelBuilder.HasDefaultSchema(_schema);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetCallingAssembly());
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null) modelBuilder.ApplyConfigurationsFromAssembly(entryAssembly);

        base.OnModelCreating(modelBuilder);
    }
}