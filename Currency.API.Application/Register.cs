using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Currency.API.Application
{
	public static class Register
	{
		public static void RegisterMediator(IServiceCollection services)
		{
			services.AddMediatR(cfg =>
			{
				cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
			});
		}

		public static void RegisterScopedServices(IServiceCollection services)
		{
			services.Scan(scan => scan.FromCallingAssembly()
			.AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase)))
			.AsImplementedInterfaces()
			.WithScopedLifetime());

		}
	}
}
