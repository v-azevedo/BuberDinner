# Clean Architecture & Domain Driven Design (DDD) - [Vídeo Tutorial](https://www.youtube.com/watch?v=fhM0V2N1GpY&list=PLzYkqgWkHPKBcDIP5gzLfASkQyTdy0t4k)

### Outsite --> Inside: (Presentation + Infrastructure --> DB) --> Application --> Domain

- Presentation: Web API, MVC, Razor Pages, SAP(Angular, React, Vue)
- Infrastructure: Persistence, Identity, File System, System Clock
- Application: Interfaces, Models, Logic, Commands/Queries, Validators,
  Exceptions
- Domain: Entities, Value Objects, Enumerations, Logic, Exceptions

### Technologies

#### Presentation

- Libraries: Mapster
- Concepts/Patterns: Mediator

#### Infrastructure

- Libraries: EF Core, BCrypt
- Concepts/Patterns: ORM

#### Application

- Libraries: MediatR, FluentValidation
- Concepts/Patterns: CQRS, Repository, Unity of Work

#### Domain

- Libraries: ErrorOr
- Concepts/Patterns: Aggregates, Aggregates Roots, ValueObjects, Entities,
  Domain Errors, Domain Events

## Part 1

### Dependency Injection in Clean Architecture

- File `DependencyInjection.cs` created inside the Application Layer with the
  objective of registering all dependencies injections related to this layer.
- Requires the package `Microsoft.Extensions.DependencyInjection.Abstractions`.

```c#
public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        return services;
    }
```

- `builder.Services.AddApplication();`

## Part 2

- #### JWT Token Generator
- #### Options Pattern for Injecting JWT Settings
- #### Using `dotnet user-secrets` for Development
- #### Sneak-peak at Debugging in VSCode

### JWT Token Generator

- Inside the Application Layer, only the interface will be referenced. The
  actual implementation of the token generator will reside in the Infrastructure
  Layer.
- The dependency injection static class inside the Infrastructure Layer will be
  the one responsible for connecting the interface to the implementation.
- [Claims](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/claims?view=aspnetcore-8.0):
  Used for authorization.

```c#
var claims = new[] {
  new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
  new Claim(JwtRegisteredClaimNames.GivenName, firstName),
  new Claim(JwtRegisteredClaimNames.FamilyName, lastName),
  new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
};
```

- Implementation necessary to initialize the signing credentials.

```c#
var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("secret")), // Must have at least 16 characters
            SecurityAlgorithms.HmacSha256);
```

- Implementation for generating the security token

```c#
var securityToken = new JwtSecurityToken(
  issuer: "BuberDinner",
  expires: DateTime.Now.AddDays(1), // Replace with Injected IDateTime provider
  claims: claims,
  signingCredentials: signingCredentials
);
```

- Lastly return a generated token
  `return new JwtSecurityTokenHandler().WriteToken(securityToken);`

### Options Pattern for Injecting JWT Settings

- Injecting Jwt settings:

```c#
public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
{
  services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
  ...
}
```

- `IOptions<JwtSettings> jwtOptions`: IOptions necessary when injecting, also
  `_jwtSettings = jwtOptions.Value;` to get the actual settings in
  `JwtSettings`.

### Using `dotnet user-secrets` for Development

- `dotnet user-secrets init --project BuberDinner.Api/`: Initialize
  user-secrets.
- `dotnet user-secrets set --project BuberDinner.Api/ "JwtSettings:Secret" "super-secret-key-from-user-secrets"`:
  Used to set a key with a value, key will match inside the `appsettings.json`.
- `dotnet user-secrets list --project BuberDinner.Api/`: Used to check all the
  secrets stored.

## TIPS

- `dotnet sln add $(ls -r **/*.csproj)`: Includes all projects to the solution
  file.
- Records can be created as follows:
  `public record RegisterRequest(Guid Id, string FirstName, string LastName);`
- Multiple methods can be chained when registering a service:
  `builder.Services.AddApplication().AddInfrastructure();`
- When working with settings, within the class that holds the properties, a
  const can be used to hold the section name.
