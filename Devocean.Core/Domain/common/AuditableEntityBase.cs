namespace Devocean.Core.Domain.common;

public class AuditableEntityBase<T> : EntityBase<T>, IAuditableEntity
{
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}