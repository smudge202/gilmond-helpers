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
		private static ConcurrentDictionary<string, Reference> Cache 
			= new ConcurrentDictionary<string, Reference>();

		private readonly ILogger _logger;

		public XmlLinqProjectFileReader(ILogger logger)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			_logger = logger;
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

		private Reference TryConstructReference(string path, XElement arg)
		{
			var assemblyPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), arg.Value));
			if (!File.Exists(assemblyPath))
				return UnableToFindReference(assemblyPath);
			return FoundReference(assemblyPath);
		}

		private Reference FoundReference(string assemblyPath)
		{
			return Cache.GetOrAdd(assemblyPath, GenerateReference);
		}

		private Reference GenerateReference(string assemblyPath)
		{
			try
			{
				var assembly = Assembly.LoadFile(assemblyPath);
				return new Reference
				{
					FullName = assembly.FullName,
					Location = assembly.Location
				};
			}
			catch (FileLoadException flex)
			{
				return UnableToLoadReference(assemblyPath, flex);
			}
			catch (BadImageFormatException bifex)
			{
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
