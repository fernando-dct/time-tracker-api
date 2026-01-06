using TimeTracker.Api.Core.Enums;

namespace TimeTracker.Api.Contracts.TimeEntries;

public record TimeEntryResponse(
    Guid Id,
    TimeEntryType Type,
    DateTimeOffset Timestamp
);
