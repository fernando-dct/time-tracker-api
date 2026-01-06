using TimeTracker.Api.Core.Enums;

namespace TimeTracker.Api.Core.Entities;

public class AdjustmentRequest
{
    public Guid Id { get; set; }
    public Guid TimeEntryId { get; set; }
    public Guid RequestedByUserId { get; set; }
    public string Reason { get; set; } = default!;
    public AdjustmentStatus Status { get; set; } = AdjustmentStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}