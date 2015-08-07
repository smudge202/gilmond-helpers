using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace Gilmond.Helpers.ListReferences
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddCoreServices(this IServiceCollection services)
		{
			services.TryAdd(GetDefaultServices());
			return services;
		}

		public static IEnumerable<ServiceDescriptor> GetDefaultServices()
		{
			yield break;
		}
	}
}
