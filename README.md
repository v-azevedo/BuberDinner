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

#### Custom Problem Details Factory

- [GitHub][def3]: Extract from here the implementation and then add to it as
  necessary.
- Injected as such:
  `builder.Services.AddSingleton<ProblemDetailsFactory, BuberDinerProblemDetailsFactory>();`.

## Part 5

### Flow Control

#### Exceptions

- Create a interface `IServiceException.cs` to declare the necessary properties.
- Implement a exception class for each error that must be handled as such:
  `DuplicateEmailException.cs`.
- The exception class must extend the `Exception` class and implement the
  `IServiceException` interface.

#### OneOf

- `dotnet add BuberDinner.Application/ package oneof`.
- `OneOf<AuthenticationResult, DuplicateEmailError> Register`: OneOf can receive
  multiples types that are expected as result.
- Match method allows to check what was the type received by the method that
  could throw an exception.

```c#
return registerResult.Match(
  authResult => Ok(MapAuthResult(authResult)),
  _ => Problem(statusCode: StatusCodes.Status409Conflict, title: "Email already exists.")
);
```

#### FluentResults

- `dotnet add BuberDinner.Application/ package fluentResults`
- `Result<AuthenticationResult> Register`: Will always return the type passed or
  a list errors.
- Implement the IError interface from the package fluentResults,
  `public class DuplicateEmailError : IError`.
- `return Result.Fail<AuthenticationResult>(new[] { new DuplicateEmailError() });`:
  A list of errors can be returned using the method Fail.
- `registerResult.IsSuccess`: Can be use to check if the return type is not an
  error.
- `registerResult.Errors[0] is DuplicateEmailError`: Errors can be used to
  access all the errors passed.

#### ErrorOr & Domain Errors

- All expected errors are specified inside de Domain Layer, folder structure:
  `Common/Errors/Errors.User.cs`.
- Errors can be specified as such, using the `Error` from ErrorOr:

```c#
public static Error DuplicateEmail => Error.Conflict(
  code: "User.DuplicateEmail",
  description: "Email is already in use");
```

- `ErrorOr<AuthenticationResult> Register`: Will return the specified type or an
  `Error`. Simply return the error from the class inside de Domain Layer
  `Errors.User.DuplicateEmail;`.

```c#
return authResult.Match( // Use Match to get access to all thrown errors or MatchFirst for only the first one.
  authResult => Ok(MapAuthResult(authResult)),
  _ => Problem(statusCode: StatusCodes.Status409Conflict, title: "User already exists")
);
```

- Can also return a list of errors
  `return new[] { Errors.Authentication.InvalidCredentials };`.
- Controller base class can be used to handle multiple errors.

```c#
public class ApiController : ControllerBase
{
  protected IActionResult Problem(List<Error> errors)
  {
    return Problem(); // Missing implementation
  }
}
```

## Part 6

### CQRS + MediatR

#### CQS vs. CQRS

- CQS: Command Query Separation. A command(procedure) does something but does
  not return a result, A query(function or attribute) returns a result but does
  not change the state.
- CQRS: Command Query Responsibility Segregation: "..The fundamental difference
  is that CQRS objects are split into two objects, one containing the Commands
  one containing the Queries."

#### MediatR + Mediator Pattern

- Commands are registered as records and must specify the arguments values and
  the response,
  `public record Login(string Email, string Password) : IRequest<ErrorOr<AuthenticationResult>>;`
- Command handler:
  `public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>`,
  "IRequestHandler" receives the command and the type of the response.
- `services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));`:
  `typeof(DependencyInjection).Assembly`, mediatR will search inside the project
  in which this class belongs to, for "handlers, queries and commands". Any
  class that belongs to the MediatR assembly can be used.
- `private readonly ISender _mediator;`: Use ISender instead of IMediator
  interface to follow the interface segregation principle.

#### Split by Feature & Clean Architecture

- Organize the folder structure as such:

```json
"Authentication": {
  "Common": {},
  "Commands": {
    "Register": {} // Files with suffix "Command" and "CommandHandler"
  },
  "Queries": {
    "Login": {} // Files with suffix "Query" and "QueryHandler"
  }
}
```

## Part 7

### Object Mapping with Mapster

- `dotnet add BuberDinner.Api/ package Mapster`.
- `dotnet add BuberDinner.Api/ package Mapster.DependencyInjection`.
- `var command = _mapper.Map<RegisterCommand>(request);`: Register the values
  received by the body and maps to the register command.
- Works without configuration when both the object source and the one being
  mapped have the same properties.
- When working with two objects that don't match properties, a configuration can
  be set. One way of doing is by implementing the interface as follows:
  `public class AuthenticationMappingConfig : IRegister`.

```c#
public void Register(TypeAdapterConfig config)
{
  config.NewConfig<AuthenticationResult, AuthenticationResponse>() // Populate AuthenticationResponse with the values from AuthenticationResult
  .Map(dest => dest.Token, src => src.Token)
  // The values from the AuthenticationResponse will be populate with the values from the src inside the User,
  // will ignore the already mapped "dest.Token".
  .Map(dest => dest, src => src.User);
}
```

- Have a dependencyInjection class inside the Mapping folder so it can handle
  its own mappings.

```c#
 public static IServiceCollection AddMappings(this IServiceCollection services)
{
  var config = TypeAdapterConfig.GlobalSettings;
  config.Scan(Assembly.GetExecutingAssembly()); // Receives the assembly that will be scanned, in this case only the executing one: "AuthenticationMappingConfig".
  services.AddSingleton(config); // Add this global configuration as a singleton, and therefore only instantiated once;
  services.AddScoped<IMapper, ServiceMapper>(); // Add scoped the Service Mapper to the IMapper, is necessary for dependency injection.
  return services;
}
```

- Use the same principle from the other layers to create a dependency injection
  class inside the presentation layer that will be calling the AddMappings
  method.

## Part 8

### MediatR + [FluentValidation](https://www.youtube.com/watch?v=-ix1hzWr2ws)

#### Pipeline Behavior with MediatR

- `dotnet add BuberDinner.Application/ package FluentValidation`.

- `dotnet add BuberDinner.Application/ package FluentValidation.AspNetCore`:
  Used to avoid having to manually add each validator.

- `IPipelineBehavior<RegisterCommand, AuthenticationResult>`: Implement the
  interface to have access to the "Handle" method on which will have access to
  the "request" and the "next" that calls the actual handler.

```c#
// before the handler
var result = await next();
// after the handler
```

- `services.AddScoped<IPipelineBehavior<RegisterCommand, ErrorOr<AuthenticationResult>>, ValidateRegisterCommandBehavior>();`:
  Dependency Injection as scoped using the `IPipelineBehavior` and passing both
  the command and the type response.

- Validators files go inside the command or query folder that will be
  validating, and receive the suffix "Validator".

```c#
public class RegisterCommandValidator : AbstractValidator<RegisterCommand> // Simple validator example
{
  public RegisterCommandValidator()
  {
    RuleFor(x => x.FirstName).NotEmpty();
    RuleFor(x => x.LastName).NotEmpty();
    RuleFor(x => x.Email).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();
  }
}
```

- `services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());`: Used
  to automatically include all the validator on the executing assembly.

- `private readonly IValidator<RegisterCommand> _validator;`: Dependency
  injection must receive the type of command inside the `IValidator`.

```c#
// Used to convert from FluentValidation errors to ErrorOr
var errors = validationResult.Errors.ConvertAll(validationFailure => Error.Validation( // ConvertAll simply returns a new list of the target type
  validationFailure.PropertyName,
  validationFailure.ErrorMessage));
```

- Using the following implementation, allows for a generic validation behavior
  class.

```c#
public class ValidationBehavior<TRequest, TResponse> :
  IPipelineBehavior<TRequest, TResponse>
      where TRequest : IRequest<TResponse> // Must use the IRequest from FluentValidator
      where TResponse : IErrorOr // Whatever response type will be expected
```

- `return (dynamic)errors;`: The dynamic keyword will try to parse the values
  received into the expected response for the method, in this case `ErrorOr`.
  Must only be used when certain that the value can be parsed, otherwise a
  runtime exception will be thrown.

- `services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));`:
  Used to register a reference to a generic dependency injection.

- Instead of returning a `Problem()` for validation errors "ControllerBase" has
  the method `return ValidationProblem(modelStateDictionary);`, that receives a
  modelStateDictionary that can be implemented as such
  `modelStateDictionary.AddModelError(error.Code, error.Description);`.

## Part 9

### JWT Bearer Authentication

#### Include authentication for the dinners controller

- In the simplest way, the idea is to change the value of `isAuthenticated` from
  httpContext to `true`, and later only allow users with this updated status to
  access the list of dinners.
- Within the program pipeline, the `app.UseAuthentication()` will access the
  authentication header from the coming request and then redirect for the
  correct authentication handler where the token will be validated and if
  necessary extracted.
- `UseAuthentication` will use objects that must be added as dependency
  injections references, inside the infrastructure layer. There will be
  configured the dependencies and what to validate from the token.
- `services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme);`:
  Responsible for adding the necessary dependencies, receives a default schema.
  [Further Explanation](https://youtu.be/7ILCRfPmQxQ?si=4nem-B4CHiG04cP9&t=455).

```c#
// Add the parameters that will be validated
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters()
  {
    ValidateIssuer = true, // Validate this parameter
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = jwtSettings.Issuer, // Uses this value to validate against
    ValidAudience = jwtSettings.Audience,
    IssuerSigningKey = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(jwtSettings.Secret)
    )
  };
});
```

- Explanation of how authorization pipeline will work, [link][authorization]. In
  short, the authorization middleware will decide if the user has authorization
  that access the request.

- `[Authorize]` attribute tells the controller that only requests from
  authorized users will be accepted and `[AllowAnonymous]` will allow any user,
  useful for auth requests when passing the `[Authorize]` attribute to all
  controllers via inheritance.

## Part 10

### Mapping Software Logic Using Process Modeling

- [Youtube](https://www.youtube.com/watch?v=1pBGc7kKOAA&list=PLzYkqgWkHPKBcDIP5gzLfASkQyTdy0t4k&index=10)

## Part 11

### 3 Steps for Modeling a Complex Domain

- [Youtube](https://www.youtube.com/watch?v=f6G46rqkePc&list=PLzYkqgWkHPKBcDIP5gzLfASkQyTdy0t4k&index=11)

## Part 12

### Implementing AggregateRoot, Entity and ValueObject

- Why implement the interface IEquatable: [StackOverflow][def6]

- [ValueObject][def4]

- [Entity][def5]

## Part 13

### Domain Layer Structure & Skeleton

- Each Aggregate receives is own folder, that will contain the Entities,
  ValueObjects and any other type that may be attached to the particular
  aggregate.

```json
"Menu":
{
  "Entities": ["MenuItem", "MenuSection"],
  "ValueObjects": ["MenuId", "MenuItemId", "MenuSectionId"],
  "Menu"
}

```

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
- Declare a inline switch statement when passing values to a variable:

```c#
var (statusCode, message) = exception switch
{
  DuplicateEmailException => (StatusCodes.Status409Conflict, "Email already exists."),
  _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
};
```

- Use this `HttpContext.Items["errors"] = errors;` to inject data inside the
  http context to be used elsewhere.

- When using dependency injection in which the received argument may be null, it
  must be explicitly set.

```c#
public ValidationBehavior(IValidator<TRequest>? validator = null)
{
  _validator = validator;
}
```

- A way to get access to settings inside the dependency injection class itself.

```c#
var jwtSettings = new JwtSettings();
configuration.Bind(JwtSettings.SectionName, jwtSettings);

services.AddSingleton(Options.Create(jwtSettings));
// Same as services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName)), but allows access to the settings.
```

- Event Storming: [Figma][figma]

[def]: https://datatracker.ietf.org/doc/html/rfc7807#section-3
[def2]: https://https://datatracker.ietf.org/doc/html/rfc7231#section-6
[def3]:
  https://github.com/dotnet/aspnetcore/blob/main/src/Mvc/Mvc.Core/src/Infrastructure/DefaultProblemDetailsFactory.cs
[authorization]: https://youtu.be/7ILCRfPmQxQ?si=VaE5X_6wyUtIM9d-&t=835
[figma]: https://www.figma.com/community/file/1153317295146512523/event-storming
[def4]:
  https://learn.microsoft.com/pt-br/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects
[def5]: https://youtu.be/weGLBPky43U?si=1JHCbM0x1Q6kzdXV&t=303
[def6]:
  https://stackoverflow.com/questions/2734914/whats-the-difference-between-iequatable-and-just-overriding-object-equals
