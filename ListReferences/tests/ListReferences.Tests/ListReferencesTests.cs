using FluentAssertions;
using Moq;
using System;
using TestAttributes;

namespace Gilmond.Helpers.ListReferences.Tests
{
	public class ListReferencesTests
	{
		private static ListReferences CreateTarget(
			LocateProjectFiles files = null)
		{
			return new DefaultListReferences(
				files ?? DefaultFiles);
		}

		private static LocateProjectFiles DefaultFiles
		{
			get
			{
				return new Mock<LocateProjectFiles>().Object;
			}
		}

		[Unit]
		public static void WhenFileLocatorNotProvidedThenThrowsException()
		{
			Action act = () => new DefaultListReferences(null);
			act.ShouldThrow<ArgumentNullException>();
		}

		[Unit]
		public static void WhenInvokedThenGetsProjectPaths()
		{
			var files = new Mock<LocateProjectFiles>();
			CreateTarget(files: files.Object).GetDistinctReferences();
			files.Verify(m => m.GetProjectFilePaths(), Times.Once);
		}
	}
}
