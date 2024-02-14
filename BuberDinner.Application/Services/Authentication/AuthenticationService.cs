namespace BuberDinner.Application.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    public AuthenticationResult Login(string email, string password)
    {
        var response = new AuthenticationResult(
            Guid.NewGuid(),
            "firstName",
            "lastName",
            email,
            "token"
        );

        return response;
    }

    public AuthenticationResult Register(string firstName, string lastName, string email, string password)
    {
        var response = new AuthenticationResult(
            Guid.NewGuid(),
            firstName,
            lastName,
            email,
            "token"
        );

        return response;
    }
}