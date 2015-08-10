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
					var references = process
						.GetReferences()
						.GroupBy(x => x.FullName, (name, refs) => new ReferenceAnalysis { FullName = name, References = refs });

					foreach (var reference in references.OrderBy(x => x.FullName))
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
