using Microsoft.Extensions.DependencyInjection;

namespace Blazor.DOMEvents;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDOMEventManagement(this IServiceCollection services)
    {
        services.AddTransient<DOMEventManager>();
        return services;
    }
}
