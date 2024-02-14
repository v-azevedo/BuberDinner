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

## TIPS

- `dotnet sln add $(ls -r **/*.csproj)`: Includes all projects to the solution
  file.