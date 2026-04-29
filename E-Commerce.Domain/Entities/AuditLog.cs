using E_Commerce.Domain.Common;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid ActorUserId { get; set; }
    public User ActorUser { get; set; } = default!;
    public AuditAction Action { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public string EntityId { get; set; } = default!; // keep as string (some entities may be GUID strings)
    public JsonText OldValuesJson { get; set; }
    public JsonText NewValuesJson { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}