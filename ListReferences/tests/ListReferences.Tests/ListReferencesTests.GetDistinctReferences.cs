using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using TestAttributes;

namespace Gilmond.Helpers.ListReferences.Tests
{
	public class GetDistinctReferences : ListReferencesTests
	{
		protected override IReadOnlyCollection<Reference> Invoke(ListReferences target)
		{
			return target.GetDistinctReferences();
		}

		[Unit]
		public static void WhenReferencesDuplicatedThenDistinctsResult()
		{
			var files = new Mock<LocateProjectFiles>();
			files.Setup(m => m.GetProjectFilePaths()).Returns(new[] { GenerateRandomString, GenerateRandomString });
			var reader = new Mock<RetrieveReferencesFromProjectFile>();
			var duplicateReference = new Reference { FullName = GenerateRandomString, Location = GenerateRandomString };
			reader.Setup(m => m.GetReferences(It.IsAny<string>())).Returns(new[] { duplicateReference });

			var result = CreateTarget(files: files.Object, reader: reader.Object).GetDistinctReferences();

			result.Should().Contain(duplicateReference);
			result.Should().OnlyHaveUniqueItems();
		}
	}
}
