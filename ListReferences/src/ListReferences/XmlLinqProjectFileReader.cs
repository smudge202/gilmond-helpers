using Microsoft.Framework.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Gilmond.Helpers.ListReferences
{
	sealed class XmlLinqProjectFileReader : RetrieveReferencesFromProjectFile
	{
		// Assembly Path -> Project Path -> Reference
		private static ConcurrentDictionary<string, ConcurrentDictionary<string, Reference>> Cache
			= new ConcurrentDictionary<string, ConcurrentDictionary<string, Reference>>();

		private readonly ILogger _logger;
		private readonly IList<Warning> _warnings;

		public XmlLinqProjectFileReader(ILogger logger, IList<Warning> warnings)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			if (warnings == null) throw new ArgumentException(nameof(warnings));
			_logger = logger;
			_warnings = warnings;
		}

		public IEnumerable<Reference> GetReferences(string path)
		{
			var msbuildNamespace = "{http://schemas.microsoft.com/developer/msbuild/2003}";
			XDocument project = XDocument.Load(path);
			return project
				.Elements(msbuildNamespace + "Project")
				.Elements(msbuildNamespace + "ItemGroup")
				.Elements(msbuildNamespace + "Reference")
				.Elements(msbuildNamespace + "HintPath")
				.Select(element => TryConstructReference(path, element))
				.Where(reference => !reference.Equals(default(Reference)));
		}

		private Reference TryConstructReference(string projectPath, XElement arg)
		{
			var assemblyPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath), arg.Value));
			if (!File.Exists(assemblyPath))
				return UnableToFindReference(assemblyPath);
			return FoundReference(projectPath, assemblyPath);
		}

		private Reference FoundReference(string projectPath, string assemblyPath)
		{
			return Cache
				.GetOrAdd(assemblyPath, x => GenerateProject(projectPath, x))
				.GetOrAdd(projectPath, x => GenerateReference(projectPath, x));
		}

		private ConcurrentDictionary<string, Reference> GenerateProject(string projectPath, string assemblyPath)
		{
			var result = new ConcurrentDictionary<string, Reference>();
			result.GetOrAdd(projectPath, x => GenerateReference(projectPath, assemblyPath));
			return result;
		}

		private Reference GenerateReference(string projectPath, string assemblyPath)
		{
			try
			{
				var assembly = Assembly.LoadFile(assemblyPath);
				return new Reference
				{
					FullName = assembly.FullName,
					Location = assembly.Location,
					Consumer = projectPath
				};
			}
			catch (FileLoadException flex)
			{
				_warnings.Add(new Warning { Consumer = projectPath, Dependency = assemblyPath, ExceptionType = "FileLoadException" });
				return UnableToLoadReference(assemblyPath, flex);
			}
			catch (BadImageFormatException bifex)
			{
				_warnings.Add(new Warning { Consumer = projectPath, Dependency = assemblyPath, ExceptionType = "BadImageException" });
				return UnableToLoadReference(assemblyPath, bifex);
			}
		}

		private Reference UnableToFindReference(string hintPath)
		{
			_logger.LogWarning($"Unable to locate reference:\r\n\t{hintPath}");
			return default(Reference);
		}

		private Reference UnableToLoadReference(string assemblyPath, FileLoadException flex)
		{
			_logger.LogWarning($"File Load Exception for reference:\r\n\t{assemblyPath}", flex);
			return default(Reference);
		}

		private Reference UnableToLoadReference(string assemblyPath, BadImageFormatException bifex)
		{
			_logger.LogWarning($"Bad Image Format for reference:\r\n\t{assemblyPath}", bifex);
			return default(Reference);
		}
	}
}
