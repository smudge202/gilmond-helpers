using FluentAssertions;
using Moq;
using System;
using System.Windows.Forms;
using TestAttributes;

namespace Gilmond.Examples.Mvp.WinForms.Tests
{
	public class MainPresenterTests
	{
		public class Constructor
		{
			private static ApplicationController DefaultController
			{
				get
				{
					return new Mock<ApplicationController>().Object;
				}
			}

			private static MainView DefaultView
			{
				get
				{
					return new Mock<MainView>().Object;
				}
			}

			private static MainPresenter CreateTarget(
				ApplicationController controller = null,
				MainView view = null)
			{
				return new MainPresenter(
					controller ?? DefaultController,
					view ?? DefaultView);
			}

			[Unit]
			public static void WhenApplicationControllIsNullThenThrowsException()
			{
				Action act = () => new MainPresenter(null, DefaultView);
				act.ShouldThrow<ArgumentNullException>();
			}

			[Unit]
			public static void WhenMainViewIsNullThenThrowsException()
			{
				Action act = () => new MainPresenter(DefaultController, null);
				act.ShouldThrow<ArgumentNullException>();
			}

			[Unit]
			public static void WhenParametersProvidedThenDisplaysView()
			{
				var controller = new Mock<ApplicationController>();
				var view = DefaultView;
				CreateTarget(controller: controller.Object, view: view);
				controller.Verify(m => m.Start(view), Times.Once);
			}
		}
	}
}
