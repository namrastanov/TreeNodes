using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TreeNodes.Application.Common.Behaviors;

namespace TreeNodes.Application;

/// <summary>
/// Service registration extensions for Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds MediatR with validators and pipeline behaviors.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}


