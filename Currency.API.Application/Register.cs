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
	}
}
