using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;

namespace Gilmond.Helpers.ListReferences.CommandLine
{
	static class ServiceExtensions
	{
		public static IServiceCollection AddConsoleServices(this IServiceCollection services)
		{
			services.TryAdd(GetDefaultServices());
			return services;
		}

		public static IServiceCollection ConfigureArguments(this IServiceCollection services, string[] args)
		{
			if (args == null || args.Length != 1)
				throw new InvalidOperationException("Must supply solution root directory as only argument.");
			return services.Configure(args[0]);
		}

		public static IEnumerable<ServiceDescriptor> GetDefaultServices()
		{
			yield return ServiceDescriptor.Transient<ILogger, ColouredConsoleLogger>();
		}
	}
}
