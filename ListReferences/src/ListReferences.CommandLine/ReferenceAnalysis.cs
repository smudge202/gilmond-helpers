using System.Collections.Generic;
using System.Linq;

namespace Gilmond.Helpers.ListReferences.CommandLine
{
	struct ReferenceAnalysis
	{
		public string FullName { get; set; }
		public IEnumerable<Reference> References { get; set; }
		public string ExampleLocation
		{
			get
			{
				return References?.FirstOrDefault().Location;
			}
		}
		public int Count
		{
			get
			{
				return References?.Count() ?? 0;
			}
		}
	}
}
