﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Gilmond.Helpers.ListReferences
{
	internal class DefaultListReferences : ListReferences
	{
		private readonly LocateProjectFiles _files;
		private readonly RetrieveReferencesFromProjectFile _reader;

		public DefaultListReferences(LocateProjectFiles files, RetrieveReferencesFromProjectFile reader)
		{
			if (files == null)
				throw new ArgumentNullException(nameof(files));
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));
			_files = files;
			_reader = reader;
		}

		public IReadOnlyCollection<Reference> GetDistinctReferences()
		{
			return _files
				.GetProjectFilePaths()
				.SelectMany(projectFilePath => _reader.GetReferences(projectFilePath))
				.ToList()
				.AsReadOnly();
		}
	}
}
