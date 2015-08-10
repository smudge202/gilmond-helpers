using Microsoft.Framework.Logging;

namespace Gilmond.Helpers.ListReferences.CommandLine
{
	static class LoggerExtensions
	{
		public static void LogReference(this ILogger logger, Reference reference)
		{
			logger.LogInformation($"{reference.FullName}");
			logger.LogDebug($"\t{reference.Location}\r\n");
		}
	}
}
