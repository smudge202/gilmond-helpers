using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System.Collections.Generic;

namespace Gilmond.Helpers.ListReferences.CommandLine
{
	static class ServiceExtensions
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.TryAdd(GetDefaultServices());
			return services;
		}

		public static IServiceCollection ConfigureArguments(this IServiceCollection services, string[] args)
		{
			services.AddOptions();
			services.Configure<UnparsedArguments>(config => config.Arguments = args);
			return services;
		}

		public static IEnumerable<ServiceDescriptor> GetDefaultServices()
		{
			yield return ServiceDescriptor.Transient<ILogger, ColouredConsoleLogger>();
		}
	}
}
