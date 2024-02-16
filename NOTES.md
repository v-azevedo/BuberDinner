# Clean Architecture & Domain Driven Design (DDD) - [Video Tutorial](https://www.youtube.com/watch?v=fhM0V2N1GpY&list=PLzYkqgWkHPKBcDIP5gzLfASkQyTdy0t4k)

## Outside --> Inside: (Presentation + Infrastructure --> DB) --> Application --> Domain

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

## Part 3

### Repository Pattern

- Entities are stored inside the Domain Layer: `User.cs`.
- The Application Layer will hold the interface that represents the repository.
- Inside the Infrastructure Layer will be the actual implementation for the
  repository.

## Part 4

### Global Error Handling

#### Middleware

- Middleware folder inside the Presentation Layer.
- Get access to the request: `private readonly RequestDelegate _next;`.
- Invoke method that will be responsible for interjecting the request and
  handling any exception.

```c#
  public async Task Invoke(HttpContext context)
{
  try
  {
    await _next(context);
  }
  catch (Exception ex)
  {
    await HandleExceptionAsync(context, ex);
  }
}
```

- Method to handle the exceptions.

```c#
private static Task HandleExceptionAsync(HttpContext context, Exception exception)
{
  var code = HttpStatusCode.InternalServerError;
  var result = JsonSerializer.Serialize(new { error = "An error occurred while processing your request" });
  context.Response.ContentType = "application/json";
  context.Response.StatusCode = (int)code;
  return context.Response.WriteAsync(result);
}
```

- Attach through the `app.UseMiddleware<ErrorHandlingMiddleware>();`.

#### Exception Filter Attribute

- Filters folder inside the Presentation Layer.
- Suffix "FilterAttribute".
- Extends class `ExceptionFilterAttribute`;
- Implements exception handling by overriding `OnException`;

```c#
public override void OnException(ExceptionContext context)
{
  var exception = context.Exception;
  context.Result = new ObjectResult(new { error = "An error occurred while processing your request" })
  {
    StatusCode = 500
  };
  context.ExceptionHandled = true;
}
```

- Attach by adding an option
  `builder.Services.AddControllers(opt => opt.Filters.Add<ErrorHandlingFilterAttribute>());`.

#### Problem Details

- Problem Details for HTTP APIs: [RFC][def]
- Built in class to help generating Problem Details according to the
  specification.

```c#
new ProblemDetails
{
  Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
  Title = "An error occurred while processing your request",
  Status = (int)HttpStatusCode.InternalServerError,
};
```

- Response status codes types: [Section 6][def2]

#### Error Endpoint

- `app.UseExceptionHandler("/error");`: Adds a middleware to the pipeline that
  will catch exceptions, log them, reset the request path, and re-execute the
  request.

- Needs a controller that will also extend from the `ControllerBase` class and
  the route specified above.

- Implements by default the RFC Problem Details when returning
  `return Problem();`.

## Part 5

### Flow Control

#### Exceptions

#### OneOf

#### FluentResults

#### ErrorOr & Domain Errors

## TIPS

- `dotnet sln add $(ls -r **/*.csproj)`: Includes all projects to the solution
  file.
- Records can be created as follows:
  `public record RegisterRequest(Guid Id, string FirstName, string LastName);`
- Multiple methods can be chained when registering a service:
  `builder.Services.AddApplication().AddInfrastructure();`
- When working with settings, within the class that holds the properties, a
  const can be used to hold the section name.
- [Declaration Pattern](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns#declaration-and-type-patterns):
  `if (_userRepository.GetUserByEmail(email) is not User user)` allows the
  declaration of a variable after passing the pattern check.

[def]: https://datatracker.ietf.org/doc/html/rfc7807#section-3
[def2]: https://https://datatracker.ietf.org/doc/html/rfc7231#section-6
