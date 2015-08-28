using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace CodeGate.Tfs.ApplicationTier
{
	static class ServiceExtensions
	{
		public static IServiceCollection AddCli(this IServiceCollection services)
		{
			services.TryAdd(GetDefaultServices());
			return services
				.AddOptions()
				.ConfigureDefaults();
		}

		public static IEnumerable<ServiceDescriptor> GetDefaultServices()
		{
			yield return ServiceDescriptor.Transient<Microsoft.Framework.Logging.ILogger, ColouredConsoleLogger>();
			yield return ServiceDescriptor.Transient<ListAllProjects, DefaultListAllProjects>();
			yield return ServiceDescriptor.Transient<TfsServer, TfsServer2013>();
		}

		static IServiceCollection ConfigureDefaults(this IServiceCollection services)
		{
			return services.Configure<Connection>(connection =>
			{
				connection.IsSecure = true;
				connection.Port = 8080;
				connection.Path = "tfs";
			});
		}
	}
}
