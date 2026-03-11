using Microsoft.Extensions.DependencyInjection;

namespace EngramMcp.Infrastructure;

public static class InfrastructureExtensions
{
    extension(IServiceCollection services)
    {
	    public IServiceCollection AddInfrastructure() => services;

	    public IServiceCollection AddInterfacesOf<T>() where T : class
	    {
		    services.AddSingleton(provider => ActivatorUtilities.CreateInstance<T>(provider));

		    foreach (var service in typeof(T).GetInterfaces())
		    {
			    if (services.Any(s => s.ServiceType == service))
				    throw new ArgumentException($"{service} already registered!");

			    services.AddSingleton(service, provider => provider.GetRequiredService<T>());
		    }

		    return services;
	    }
    }
}
