using System;
using System.Windows.Forms;

namespace Mvp.WinForms
{
	sealed class MainPresenter
	{
		public MainPresenter(ApplicationController controller, MainView view)
		{
			if (controller == null)
				throw new ArgumentNullException(nameof(controller));
			if (view == null)
				throw new ArgumentNullException(nameof(view));
			controller.Start(view);
		}
	}
}
