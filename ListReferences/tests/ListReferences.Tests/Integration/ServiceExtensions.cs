using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;
using System.IO;

namespace Gilmond.Helpers.ListReferences.Tests.Integration
{
	internal static class ServiceExtensions
	{
		private const string InvalidConfiguration = "You must have defined a \"PROJECTS_ROOT\" system environment variable" +
			"containing the path to the directory you keep your projects, for which the assumption is we can find:\r\n" +
			ProjectRelativePath;
		// NB: You may need to restart VS after adding the variable for it to be detected

		private const string ProjectRelativePath = "gilmond-helpers\\ListReferences";

		public static IServiceCollection ConfigureForLocalProject(this IServiceCollection services)
		{
			var configuration = new ConfigurationBuilder()
				.AddEnvironmentVariables()
				.Build();
			var solutionPath = Path.Combine(configuration["PROJECTS_ROOT"], ProjectRelativePath);
			if (!Directory.Exists(solutionPath))
				throw new InvalidOperationException(InvalidConfiguration);

			return services
				.AddOptions()
				.Configure<SolutionConfiguration>(config =>
			{
				config.SolutionPath = solutionPath;
			});
		}

		public static IServiceCollection AddTraceLogger(this IServiceCollection services)
		{
			services.TryAdd(ServiceDescriptor.Transient<ILogger, TraceLogger>());
			return services;
		}
    }
}
