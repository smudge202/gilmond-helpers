using System.Collections.Generic;

namespace Gilmond.Helpers.ListReferences
{
	interface RetrieveReferencesFromProjectFile
	{
		IEnumerable<Reference> GetReferences(string path);
	}
}
