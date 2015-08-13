using Compose;

namespace Gilmond.Examples.Mvp.WinForms
{
	using System.Windows.Forms;
	static class ApplicationExtensions
	{
		public static void UseMvpExample(this Executable app)
		{
			app.OnExecute<MainPresenter>(presenter => { });
		}
	}
}
