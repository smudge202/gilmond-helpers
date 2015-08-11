 using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace Mvp.WinForms
{
	static class ServiceExtensions
	{
		public static IServiceCollection AddWinForms(this IServiceCollection services)
		{
			services.TryAdd(GetDefaultViews());
			return services;
		}

		public static IEnumerable<ServiceDescriptor> GetDefaultViews()
		{
			yield return ServiceDescriptor.Transient<IApplicationController, WindowsFormsApplicationController>();
			yield return ServiceDescriptor.Transient<MainForm, MainForm>();
		}
	}
}
