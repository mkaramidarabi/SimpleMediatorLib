using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleMediator
{

internal sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }
    public Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _serviceProvider.GetRequiredService(handlerType);

        var behaviors = _serviceProvider
            .GetServices(behaviorType)
            .Reverse()
            .ToArray();

        RequestHandlerDelegate<TResponse> handlerDelegate = () => ((dynamic)handler).Handle((dynamic)request, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () =>
                ((dynamic)behavior!).Handle((dynamic)request, cancellationToken, next);
        }

        return handlerDelegate();
    }

    public Task Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification == null) throw new ArgumentNullException(nameof(notification));

        return PublishInternal(notification, cancellationToken);
    }

    private async Task PublishInternal<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        var handlers = _serviceProvider
            .GetServices<INotificationHandler<TNotification>>()
            .ToArray();

        foreach (var handler in handlers)
        {
            await handler.Handle(notification, cancellationToken).ConfigureAwait(false);
        }
    }
}
}

