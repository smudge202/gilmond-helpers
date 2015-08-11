using System.Windows.Forms;

namespace Mvp.WinForms
{
	sealed class WindowsFormsApplicationController : IApplicationController
	{
		public void Exit()
		{
			Application.Exit();
		}
	}
}
