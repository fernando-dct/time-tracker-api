using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Contracts.Auth;
using TimeTracker.Api.Core.Entities;
using TimeTracker.Api.Core.Enums;
using TimeTracker.Api.Infrastructure.Auth;
using TimeTracker.Api.Infrastructure.Data;

namespace TimeTracker.Api.Features.Auth;

public static class RegisterEndpoint
{
    public static void MapRegister(this WebApplication app)
    {
        app.MapPost("/auth/register", async (
            RegisterRequest request,
            AppDbContext db,
            PasswordHasher hasher) =>
        {
            var emailExists = await db.Users
                .AnyAsync(x => x.Email == request.Email);

            if (emailExists)
                return Results.Conflict("Email já cadastrado.");

            var isFirstUser = !await db.Users.AnyAsync();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email.ToLower(),
                PasswordHash = hasher.Hash(request.Password),
                Role = isFirstUser ? UserRole.Admin : UserRole.User
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/users/{user.Id}",
                new RegisterResponse(user.Id, user.Name, user.Email)
            );
        });
    }
}