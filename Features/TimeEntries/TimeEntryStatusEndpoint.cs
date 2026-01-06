using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeTracker.Api.Contracts.TimeEntries;
using TimeTracker.Api.Core.Enums;
using TimeTracker.Api.Infrastructure.Data;

namespace TimeTracker.Api.Features.TimeEntries;

public static class TimeEntryStatusEndpoint
{
    public static void MapTimeEntryStatus(this WebApplication app)
    {
        app.MapGet("/time-entries/today/status", async (
            ClaimsPrincipal user,
            AppDbContext db) =>
        {
            var userId = Guid.Parse(
                user.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var today = DateTimeOffset.UtcNow.Date;

            var lastEntry = await db.TimeEntries
                .Where(x => x.UserId == userId && x.Timestamp.Date == today)
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync();

            if (lastEntry is null)
            {
                return Results.Ok(new TodayStatusResponse(
                    WorkDayStatus.NotStarted,
                    null,
                    null
                ));
            }

            var status = lastEntry.Type switch
            {
                TimeEntryType.ClockIn => WorkDayStatus.Working,
                TimeEntryType.BreakStart => WorkDayStatus.OnBreak,
                TimeEntryType.BreakEnd => WorkDayStatus.Working,
                TimeEntryType.ClockOut => WorkDayStatus.Finished,
                _ => WorkDayStatus.NotStarted
            };

            return Results.Ok(new TodayStatusResponse(
                status,
                lastEntry.Timestamp,
                lastEntry.Type
            ));
        })
        .RequireAuthorization();
    }
}