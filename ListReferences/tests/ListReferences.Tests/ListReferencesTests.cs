using FluentAssertions;
using Moq;
using System;
using TestAttributes;

namespace Gilmond.Helpers.ListReferences.Tests
{
	public class ListReferencesTests
	{
		private static ListReferences CreateTarget(
			LocateProjectFiles files = null,
			RetrieveReferencesFromProjectFile reader = null)
		{
			return new DefaultListReferences(
				files ?? DefaultFiles,
				reader ?? DefaultReader);
		}

		private static LocateProjectFiles DefaultFiles
		{
			get
			{
				return new Mock<LocateProjectFiles>().Object;
			}
		}

		private static RetrieveReferencesFromProjectFile DefaultReader
		{
			get
			{
				return new Mock<RetrieveReferencesFromProjectFile>().Object;
			}
		}

		private static string GenerateRandomString
		{
			get
			{
				return Guid.NewGuid().ToString();
			}
		}

		[Unit]
		public static void WhenFileLocatorNotProvidedThenThrowsException()
		{
			Action act = () => new DefaultListReferences(null, DefaultReader);
			act.ShouldThrow<ArgumentNullException>();
		}

		[Unit]
		public static void WhenFileReaderNotProvidedThenThrowsException()
		{
			Action act = () => new DefaultListReferences(DefaultFiles, null);
			act.ShouldThrow<ArgumentNullException>();
		}

		[Unit]
		public static void WhenInvokedThenGetsProjectPaths()
		{
			var files = new Mock<LocateProjectFiles>();
			CreateTarget(files: files.Object).GetDistinctReferences();
			files.Verify(m => m.GetProjectFilePaths(), Times.Once);
		}

		[Unit]
		public static void WhenPathYieldedThenPassesToDeserializer()
		{
			var files = new Mock<LocateProjectFiles>();
			var path = GenerateRandomString;
			files.Setup(m => m.GetProjectFilePaths()).Returns(new[] { path });
			var reader = new Mock<RetrieveReferencesFromProjectFile>();

			CreateTarget(files: files.Object, reader: reader.Object).GetDistinctReferences();

			reader.Verify(m => m.GetReferences(path), Times.Once);
		}
	}
}
