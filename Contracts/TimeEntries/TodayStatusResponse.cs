using TimeTracker.Api.Core.Enums;

namespace TimeTracker.Api.Contracts.TimeEntries;

public record TodayStatusResponse(
    WorkDayStatus Status,
    DateTimeOffset? LastEntryAt,
    TimeEntryType? LastEntryType
);