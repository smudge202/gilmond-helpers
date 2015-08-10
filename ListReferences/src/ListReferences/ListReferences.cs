using System.Collections.Generic;

namespace Gilmond.Helpers.ListReferences
{
	public interface ListReferences
	{
		IReadOnlyCollection<Reference> GetReferences();

		IReadOnlyCollection<Reference> GetDistinctReferences();
	}
}
