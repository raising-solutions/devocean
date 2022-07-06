namespace Devocean.Core.Domain.common;

public class AuditableValueObjectBase : ValueObjectBase, IAuditableEntity
{
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}