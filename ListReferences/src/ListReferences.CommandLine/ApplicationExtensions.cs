using Compose;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;

namespace Gilmond.Helpers.ListReferences.CommandLine
{
	static class ApplicationExtensions
	{
		public static void ListReferences(this CommandLineApplication app)
		{
			app.OnExecute<ListReferences>(process =>
			{
				
				return 0;
			});
		}
	}
}
