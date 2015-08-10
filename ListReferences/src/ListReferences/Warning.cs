using System;

namespace Gilmond.Helpers.ListReferences
{
	public struct Warning
	{
		public string Consumer { get; set; }
		public string Dependency { get; set; }
		public string ExceptionType { get; set; }
	}
}
