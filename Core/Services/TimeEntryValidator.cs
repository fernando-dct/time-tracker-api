using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Common.Exceptions;
using TimeTracker.Api.Core.Enums;
using TimeTracker.Api.Infrastructure.Data;

namespace TimeTracker.Api.Core.Services;

public class TimeEntryValidator(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task ValidateAsync(Guid userId, TimeEntryType newType)
    {
        var today = DateTimeOffset.UtcNow.Date;

        var lastEntry = await _db.TimeEntries
            .Where(x => x.UserId == userId && x.Timestamp.Date == today)
            .OrderByDescending(x => x.Timestamp)
            .FirstOrDefaultAsync();

        if (lastEntry is null)
        {
            if (newType != TimeEntryType.ClockIn)
                throw new BusinessException(
                    "O primeiro registro do dia deve ser ClockIn."
                );

            return;
        }

        switch (lastEntry.Type, newType)
        {
            case (TimeEntryType.ClockIn, TimeEntryType.ClockIn):
                throw new BusinessException("Você já registrou entrada.");

            case (TimeEntryType.ClockIn, TimeEntryType.BreakEnd):
                throw new BusinessException(
                    "Você precisa iniciar o intervalo antes de encerrá-lo."
                );

            case (TimeEntryType.BreakStart, TimeEntryType.BreakStart):
                throw new BusinessException("Intervalo já iniciado.");

            case (TimeEntryType.BreakStart, TimeEntryType.ClockOut):
                throw new BusinessException(
                    "Finalize o intervalo antes de sair."
                );

            case (TimeEntryType.BreakEnd, TimeEntryType.BreakStart):
                throw new BusinessException("Você já voltou do intervalo.");

            case (TimeEntryType.ClockOut, _):
                throw new BusinessException("A jornada já foi encerrada.");
        }
    }
}
