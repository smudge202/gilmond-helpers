using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gilmond.Helpers.ListReferences
{
	public static class ServiceExtensions
	{
		private const string ProjectsRootVariableName = "PROJECTS_ROOT";
        private const string InvalidConfigurationTemplate = "You must define \"" + ProjectsRootVariableName + 
			"\" system environment variable containing the path to the directory you keep your projects, " +
			"for which the assumption is we can find:\r\n{0}";
		// NB: You may need to restart VS after adding the variable for it to be detected

		public static IServiceCollection Configure(this IServiceCollection services, string path)
		{
			var configuration = new ConfigurationBuilder()
				.AddEnvironmentVariables()
				.Build();
			var solutionPath = Path.Combine(configuration["PROJECTS_ROOT"] ?? string.Empty, path);
			if (!Directory.Exists(solutionPath))
				throw new InvalidOperationException(string.Format(InvalidConfigurationTemplate, path));

			return services
				.AddOptions()
				.Configure<SolutionConfiguration>(config =>
				{
					config.SolutionPath = solutionPath;
				});
		}

		public static IServiceCollection AddCoreServices(this IServiceCollection services)
		{
			services.TryAdd(GetDefaultServices());
			return services;
		}

		public static IEnumerable<ServiceDescriptor> GetDefaultServices()
		{
			yield return ServiceDescriptor.Transient<RetrieveReferencesFromProjectFile, XmlLinqProjectFileReader>();
			yield return ServiceDescriptor.Transient<LocateProjectFiles, DirectoryGetFilesLocator>();
			yield return ServiceDescriptor.Transient<ListReferences, DefaultListReferences>();
		}
	}
}
