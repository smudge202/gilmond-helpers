using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeGate.Tfs.ApplicationTier
{
	sealed class ProjectCollection
	{
		public string Name { get; set; }
		public ReadOnlyCollection<Project> Projects { get; internal set; }
	}
}
