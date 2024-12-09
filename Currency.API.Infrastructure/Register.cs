using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Currency.API.Utilities.Extensions;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace Currency.API.Infrastructure
{
	public static class Register
	{
		public static void RegisterScopedRepository(IServiceCollection services)
		{
			services.Scan(scan => scan.FromCallingAssembly()
			.AddClasses(classes => classes.Where(t => t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase)
				&& !t.Name.StartsWith("Cache", StringComparison.OrdinalIgnoreCase)))
			.AsImplementedInterfaces()
			.WithScopedLifetime());

			// 動態反射注入Decorate Cache Repository
			Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(p => p.Name.StartsWith("Cache") && p.Name.EndsWith("Repository"))
				.ForEach(type =>
				{
					var cacheInterface = type.GetInterfaces().Single(p => p.Name.EndsWith("Repository"));
					services.Decorate(cacheInterface, type);
				});
		}

		public static void RegisterScopedDBConnection(IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IDbConnection>(provider =>
			{
				var connection = configuration.GetConnectionString("DBConnection");
				var conn = new SqlConnection
				{
					ConnectionString = connection
				};
				return conn;
			});
		}
	}
}
