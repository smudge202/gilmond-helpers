using Compose;
using System;

namespace Mvp.WinForms
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			var app = new WindowsFormsApplication();
			app.UseServices(services =>
			{
				services
					.AddWinForms();
			});
			app.UseMvpExample();
			app.Execute();
		}
	}
}
