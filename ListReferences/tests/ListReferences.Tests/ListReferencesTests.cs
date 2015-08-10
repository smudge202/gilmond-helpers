using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TestAttributes;

namespace Gilmond.Helpers.ListReferences.Tests
{
	public abstract class ListReferencesTests
	{
		internal static ListReferences CreateTarget(
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

		protected static string GenerateRandomString
		{
			get
			{
				return Guid.NewGuid().ToString();
			}
		}

		protected abstract IReadOnlyCollection<Reference> Invoke(ListReferences target);

		[Unit]
		public void WhenFileLocatorNotProvidedThenThrowsException()
		{
			Action act = () => new DefaultListReferences(null, DefaultReader);
			act.ShouldThrow<ArgumentNullException>();
		}

		[Unit]
		public void WhenFileReaderNotProvidedThenThrowsException()
		{
			Action act = () => new DefaultListReferences(DefaultFiles, null);
			act.ShouldThrow<ArgumentNullException>();
		}

		[Unit]
		public void WhenInvokedThenGetsProjectPaths()
		{
			var files = new Mock<LocateProjectFiles>();
			CreateTarget(files: files.Object).GetDistinctReferences();
			files.Verify(m => m.GetProjectFilePaths(), Times.Once);
		}

		[Unit]
		public void WhenPathYieldedThenPassesToDeserializer()
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
