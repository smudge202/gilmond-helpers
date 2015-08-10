using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace Gilmond.Helpers.ListReferences.Tests.Integration
{
	internal static class ServiceExtensions
	{
		private const string ProjectRelativePath = "gilmond-helpers\\ListReferences";

		public static IServiceCollection ConfigureForLocalProject(this IServiceCollection services)
		{
			return services.Configure(ProjectRelativePath);
		}

		public static IServiceCollection AddTraceLogger(this IServiceCollection services)
		{
			services.TryAdd(ServiceDescriptor.Transient<ILogger, TraceLogger>());
			return services;
		}
    }
}
