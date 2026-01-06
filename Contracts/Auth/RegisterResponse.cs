namespace TimeTracker.Api.Contracts.Auth;

public record RegisterResponse(
    Guid Id,
    string Name,
    string Email
);