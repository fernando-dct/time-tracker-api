using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeTracker.Api.Contracts.TimeEntries;
using TimeTracker.Api.Core.Entities;
using TimeTracker.Api.Core.Services;
using TimeTracker.Api.Infrastructure.Data;

namespace TimeTracker.Api.Features.TimeEntries;

public static class TimeEntriesEndpoints
{
    public static void MapTimeEntries(this WebApplication app)
    {
        app.MapPost("/time-entries", async (
            CreateTimeEntryRequest request,
            ClaimsPrincipal user,
            AppDbContext db,
            TimeEntryValidator validator) =>
                {
                    var userId = Guid.Parse(
                        user.FindFirstValue(ClaimTypes.NameIdentifier)!
                    );

                    await validator.ValidateAsync(userId, request.Type);

                    var entry = new TimeEntry
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Type = request.Type,
                        Timestamp = DateTimeOffset.UtcNow
                    };

                    db.TimeEntries.Add(entry);
                    await db.SaveChangesAsync();

                    return Results.Created(
                        $"/time-entries/{entry.Id}",
                        new TimeEntryResponse(entry.Id, entry.Type, entry.Timestamp)
                    );
                })
        .RequireAuthorization();


        // 🔹 GET /time-entries/me → histórico do usuário
        app.MapGet("/time-entries/me", async (
            ClaimsPrincipal user,
            AppDbContext db) =>
        {
            var userId = Guid.Parse(
                user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub")!
            );

            var entries = await db.TimeEntries
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Timestamp)
                .Select(x => new TimeEntryResponse(
                    x.Id,
                    x.Type,
                    x.Timestamp
                ))
                .ToListAsync();

            return Results.Ok(entries);
        })
        .RequireAuthorization();
    }
}