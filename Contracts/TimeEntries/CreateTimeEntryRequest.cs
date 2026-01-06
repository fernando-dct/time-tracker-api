using TimeTracker.Api.Core.Enums;

namespace TimeTracker.Api.Contracts.TimeEntries;

public record CreateTimeEntryRequest(
    TimeEntryType Type
);
