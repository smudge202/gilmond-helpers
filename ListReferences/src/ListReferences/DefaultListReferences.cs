using System;

namespace Gilmond.Helpers.ListReferences
{
	internal class DefaultListReferences : ListReferences
	{
		private readonly LocateProjectFiles _files;

		public DefaultListReferences(LocateProjectFiles files)
		{
			if (files == null)
				throw new ArgumentNullException();
			_files = files;
		}

		public void GetDistinctReferences()
		{
			_files.GetProjectFilePaths();
		}
	}
}
