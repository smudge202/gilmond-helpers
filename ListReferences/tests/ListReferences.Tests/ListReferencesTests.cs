using FluentAssertions;
using Moq;
using System;
using System.Linq;
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

		[Unit]
		public static void WhenReferencesDuplicatedThenDistinctsResult()
		{
			var files = new Mock<LocateProjectFiles>();
			files.Setup(m => m.GetProjectFilePaths()).Returns(new[] { GenerateRandomString, GenerateRandomString});
			var reader = new Mock<RetrieveReferencesFromProjectFile>();
			var duplicateReference = new Reference { FullName = GenerateRandomString, Location = GenerateRandomString };
			reader.Setup(m => m.GetReferences(It.IsAny<string>())).Returns(new[] { duplicateReference });

			var result = CreateTarget(files: files.Object, reader: reader.Object).GetDistinctReferences();

			result.Should().Contain(duplicateReference);
			result.Should().OnlyHaveUniqueItems();
		}
	}
}
