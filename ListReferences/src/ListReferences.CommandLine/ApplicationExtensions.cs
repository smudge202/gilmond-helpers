using Compose;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
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

					var warnings = app.ApplicationServices.GetService<IList<Warning>>();
					foreach (var reference in references.Where(x => !x.References.All(y => y.Location == x.References.First().Location)))
						foreach(var inconsistentLocation in reference.References
							.GroupBy(x => x.Location, (location, refs) => new { Location = location, References = refs }))
							warnings.Add(new Warning
							{
								Consumer = $"{inconsistentLocation.References.Count()} references to {inconsistentLocation.Location}",
								Dependency = reference.FullName,
								ExceptionType = "InconsistentDependencyLocations"
							});

					references.OutputAsJson();

					warnings.OutputAsJson();

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
