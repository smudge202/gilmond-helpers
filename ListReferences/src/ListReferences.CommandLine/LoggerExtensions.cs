using Microsoft.Framework.Logging;

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
	}
}
