namespace BuberDinner.Contracts.Authentication;

public record AuthenticationResponse(
    Guid Id,
    string FirstName,
    string Lastname,
    string Email,
    string Password,
    string Token
);