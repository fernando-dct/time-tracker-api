using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.Contracts.Auth;
using TimeTracker.Api.Infrastructure.Auth;
using TimeTracker.Api.Infrastructure.Data;

namespace TimeTracker.Api.Features.Auth;

public static class LoginEndpoint
{
    public static void MapLogin(this WebApplication app)
    {
        app.MapPost("/auth/login", async (
            LoginRequest request,
            AppDbContext db,
            PasswordHasher hasher,
            JwtTokenService tokenService) =>
        {
            var user = await db.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user is null || !hasher.Verify(request.Password, user.PasswordHash))
                return Results.Unauthorized();

            var token = tokenService.GenerateToken(user);
            return Results.Ok(new LoginResponse(token));
        });
    }
}