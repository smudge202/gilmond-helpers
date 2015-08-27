using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeGate.Tfs.ApplicationTier
{
	interface TfsServer
	{
		ReadOnlyCollection<ProjectCollection> GetProjectCollections();
	}
}
