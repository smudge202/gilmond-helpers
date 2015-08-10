using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gilmond.Helpers.ListReferences
{
	sealed class DirectoryGetFilesLocator : LocateProjectFiles
	{
		private readonly string _solutionDirectory;

		public DirectoryGetFilesLocator(IOptions<SolutionConfiguration> config)
		{
			if (config == null || config.Options == null || string.IsNullOrWhiteSpace(config.Options.SolutionPath))
				throw new ArgumentException("SolutionConfiguration must be supplied with Solution Path.");
			if (!Directory.Exists(config.Options.SolutionPath) && !File.Exists(config.Options.SolutionPath))
				throw new DirectoryNotFoundException($"Configured Solution File not found:\r\n\t{config.Options.SolutionPath}");
			_solutionDirectory = Path.GetDirectoryName(config.Options.SolutionPath);
		}

		public IEnumerable<string> GetProjectFilePaths()
		{
			return 
				Directory.GetFiles(_solutionDirectory, "*.csproj", SearchOption.AllDirectories)
					.Union(
				Directory.GetFiles(_solutionDirectory, "*.vbproj", SearchOption.AllDirectories));
		}
	}
}
