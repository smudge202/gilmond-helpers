using Microsoft.Framework.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gilmond.Helpers.ListReferences.CommandLine
{
	static class LoggerExtensions
	{
		public static void LogReference(this ILogger logger, ReferenceAnalysis reference)
		{
			logger.LogInformation($"{reference.FullName}");
			logger.LogDebug($"{reference.Count} occurences of this reference");
			logger.LogDebug($"\t{reference.ExampleLocation}\r\n");
		}

		public static void OutputAsJson(this IEnumerable<ReferenceAnalysis> references)
		{
			using (var fileStream = File.Open("references.json", FileMode.CreateNew, FileAccess.Write))
			using (var writer = new StreamWriter(fileStream))
				writer.Write(JsonConvert.SerializeObject(references.OrderByDescending(x => x.Count), Formatting.Indented));

			using (var fileStream = File.Open("references-thirdparty.json", FileMode.CreateNew, FileAccess.Write))
			using (var writer = new StreamWriter(fileStream))
				writer.Write(JsonConvert.SerializeObject(references.Where(x => !x.IsProbablyGilmondReference()), Formatting.Indented));
		}

		public static void OutputAsJson(this IList<Warning> warnings)
		{
			using (var fileStream = File.Open("warnings.json", FileMode.CreateNew, FileAccess.Write))
			using (var writer = new StreamWriter(fileStream))
				writer.Write(JsonConvert.SerializeObject(warnings, Formatting.Indented));
		}

		private static bool IsProbablyGilmondReference(this ReferenceAnalysis reference)
		{
			for (var i = 0; i < 3; i++)
				if (char.IsLower(reference.FullName[i]))
					return false;
			return reference.FullName[3] == '_';
		}
    }
}
