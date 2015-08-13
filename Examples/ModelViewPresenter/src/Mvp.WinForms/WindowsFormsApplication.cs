using Compose;

namespace Gilmond.Examples.Mvp.WinForms
{
	using System.Windows.Forms;
	internal sealed class WindowsFormsApplication : Executable
	{
		public override void Execute()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			base.Execute();
		}
	}
}
