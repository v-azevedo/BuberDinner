namespace BuberDinner.Contracts.Authentication;

public record RegisterRequest(
    string FirstName,
    string Lastname,
    string Email,
    string Password
);