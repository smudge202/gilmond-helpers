using System;
using System.Collections.Generic;

namespace Gilmond.Helpers.ListReferences.Tests
{
	public class GetReferences : ListReferencesTests
	{
		protected override IReadOnlyCollection<Reference> Invoke(ListReferences target)
		{
			return target.GetReferences();
		}
	}
}
