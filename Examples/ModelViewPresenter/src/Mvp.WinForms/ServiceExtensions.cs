using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;
using System.Windows.Forms;

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
			yield return ServiceDescriptor.Transient<MainView, MainForm>();
			yield return ServiceDescriptor.Transient<ApplicationController, WindowsFormsApplicationController>();
			yield return ServiceDescriptor.Transient<MainPresenter, MainPresenter>();
		}
	}
}
