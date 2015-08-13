using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace $rootnamespace$
{
	static class $safeitemrootname$
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.TryAdd(GetDefaultServices());
			return services;
		}

		public static IEnumerable<ServiceDescriptor> GetDefaultServices()
		{
			//yield return ServiceDescriptor.Transient<Abstraction, Implementation>();
			yield break;
		}
	}
}
