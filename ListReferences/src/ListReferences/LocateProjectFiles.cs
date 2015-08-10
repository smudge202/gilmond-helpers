using System.Collections.Generic;

namespace Gilmond.Helpers.ListReferences
{
	interface LocateProjectFiles
	{
		IEnumerable<string> GetProjectFilePaths();
	}
}
