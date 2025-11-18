using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleMediator
{

public static class SimpleMediatorServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleMediator(
        this IServiceCollection services,
        params Assembly[] assembliesToScan)
    {
        if (assembliesToScan == null || assembliesToScan.Length == 0)
            throw new ArgumentException(@"You must provide at least one assembly to scan.", nameof(assembliesToScan));

        services.AddScoped<IMediator, Mediator>();

        var allTypes = assembliesToScan
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToArray();

        foreach (var type in allTypes)
        {
            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType) continue;

                var genericDef = @interface.GetGenericTypeDefinition();

                if (genericDef == typeof(IRequestHandler<,>) ||
                    genericDef == typeof(INotificationHandler<>) ||
                    genericDef == typeof(IPipelineBehavior<,>))
                {
                    services.AddTransient(@interface, type);
                }
            }
        }

        return services;
    }
}

}
