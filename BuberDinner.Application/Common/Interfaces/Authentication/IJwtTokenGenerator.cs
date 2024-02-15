namespace BuberDinner.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string TokenGenerator(Guid userId, string firstName, string lastName);
}