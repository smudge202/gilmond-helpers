using System.Windows.Forms;

namespace Gilmond.Examples.Mvp.WinForms
{
	sealed class WindowsFormsApplicationController : ApplicationController
	{
		public void Start(object form)
		{
			Application.Run(form as Form);
		}

		public void Exit()
		{
			Application.Exit();
		}
	}
}
