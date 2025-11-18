# SimpleMediator

A simple, lightweight mediator pattern implementation for .NET with support for request/response handlers, notifications, and pipeline behaviors.

## Installation

```bash
dotnet add package SimpleMediatorLib
```

## Supported Frameworks

SimpleMediator supports a wide range of .NET implementations, including all future versions:

- **.NET Standard 2.0** - Compatible with .NET Framework 4.6.1+, .NET Core 2.0+
- **.NET Standard 2.1** - Compatible with .NET Core 2.1+, .NET 5+, .NET 6+, .NET 7+, .NET 8+, and all future .NET versions

By targeting .NET Standard, SimpleMediator automatically works with all current and future .NET implementations without requiring package updates.

## Quick Start

### 1. Register SimpleMediator

```csharp
using SimpleMediator;
using System.Reflection;

builder.Services.AddSimpleMediator(
    typeof(YourApplicationAssemblyMarker).Assembly
);
```

### 2. Create a Request

```csharp
public sealed record GetUserQuery(int UserId)
    : IRequest<User>;

public sealed class GetUserQueryHandler
    : IRequestHandler<GetUserQuery, User>
{
    public Task<User> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken)
    {
        // Your logic here
        return Task.FromResult(new User { Id = request.UserId });
    }
}
```

### 3. Send a Request

```csharp
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _mediator.Send(new GetUserQuery(id));
        return Ok(user);
    }
}
```

## Features

- **Request/Response Pattern**: Send requests and receive typed responses
- **Notifications**: Publish notifications to multiple handlers
- **Pipeline Behaviors**: Add cross-cutting concerns (logging, validation, etc.)
- **Dependency Injection**: Seamless integration with Microsoft.Extensions.DependencyInjection
- **Assembly Scanning**: Automatically discovers and registers handlers

## Pipeline Behaviors

Add cross-cutting concerns using pipeline behaviors:

```csharp
public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("Handling {RequestType}", typeof(TRequest).Name);
        
        var response = await next();
        
        _logger.LogInformation("Handled {RequestType}", typeof(TRequest).Name);
        
        return response;
    }
}
```

## Notifications

Publish notifications to multiple handlers:

```csharp
public sealed record UserCreatedEvent(int UserId, string UserName)
    : INotification;

public sealed class SendWelcomeEmailHandler
    : INotificationHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Send welcome email
        return Task.CompletedTask;
    }
}

public sealed class UpdateUserCacheHandler
    : INotificationHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Update cache
        return Task.CompletedTask;
    }
}

// Publish the notification
await _mediator.Publish(new UserCreatedEvent(userId, userName));
```

## License

MIT

