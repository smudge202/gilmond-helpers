using Compose;
using Microsoft.Framework.DependencyInjection;

namespace Mvp.WinForms
{
	using System.Windows.Forms;
	static class ApplicationExtensions
	{
		public static void UseMvpExample(this Executable app)
		{
			app.OnExecute(() =>
			{
				Application.Run(app.ApplicationServices.GetRequiredService<MainForm>());
			});
		}
	}
}
