using Microsoft.Framework.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Gilmond.Helpers.ListReferences.CommandLine
{
	static class LoggerExtensions
	{
		public static void LogReference(this ILogger logger, ReferenceAnalysis reference)
		{
			logger.LogInformation($"{reference.FullName}");
			logger.LogDebug($"{reference.Count} occurences of this reference");
			logger.LogDebug($"\t{reference.Location}\r\n");
		}

		public static void OutputAsJson(this IEnumerable<ReferenceAnalysis> references)
		{
			using (var fileStream = File.OpenWrite("references.json"))
			using (var writer = new StreamWriter(fileStream))
				writer.Write(JsonConvert.SerializeObject(references, Formatting.Indented));
		}
	}
}
