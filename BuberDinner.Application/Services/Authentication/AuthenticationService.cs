using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Entities;

namespace BuberDinner.Application.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }

    public AuthenticationResult Login(string email, string password)
    {
        if (_userRepository.GetUserByEmail(email) is not User user)
            throw new Exception("Email not found on our database");

        if (user.Password != password)
            throw new Exception("Wrong password");

        var token = _jwtTokenGenerator.TokenGenerator(user.Id, user.FirstName, user.LastName);

        return new AuthenticationResult(
          user,
          token
        );
    }

    public AuthenticationResult Register(string firstName, string lastName, string email, string password)
    {
        if (_userRepository.GetUserByEmail(email) is not null)
            throw new Exception("Email already in use");

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        };

        _userRepository.AddUser(user);

        var token = _jwtTokenGenerator.TokenGenerator(user.Id, firstName, lastName);

        return new AuthenticationResult(
            user,
            token
        );
    }
}