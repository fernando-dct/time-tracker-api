using TimeTracker.Api.Core.Enums;

namespace TimeTracker.Api.Core.Entities;

public class TimeEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public TimeEntryType Type { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    // Navegação (opcional agora, útil depois)
    public User? User { get; set; }
}