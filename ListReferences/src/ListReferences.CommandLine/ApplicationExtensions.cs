using Compose;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;
using System.Linq;

namespace Gilmond.Helpers.ListReferences.CommandLine
{
	static class ApplicationExtensions
	{
		public static void ListReferences(this CommandLineApplication app)
		{
			app.OnExecute<ListReferences>(process =>
			{
				var logger = app.ApplicationServices.GetService<ILogger>();
				try
				{
					foreach (var reference in process.GetDistinctReferences().OrderBy(x => x.FullName))
						logger.LogReference(reference);
					return 0;
				}
				catch (Exception ex)
				{
					logger.LogCritical("Unhandled Exception", ex);
					return 1;
				}
			});
		}
	}
}
