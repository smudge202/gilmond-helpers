using Compose;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using System;

namespace CodeGate.Tfs.ApplicationTier
{
	static class ApplicationExtensions
	{
		public static void UseDisplayAllProjects(this Executable<int> app)
		{

			app.OnExecute<ListAllProjects>(listAllProjects =>
			{
				try
				{
					listAllProjects.DisplayProjects();
					return 0;
				}
				catch (Exception ex)
				{
					app.ApplicationServices.GetService<ILogger>()
						.LogCritical("Unhandled Exception", ex);
					return 1;
				}
				finally
				{
					Console.WriteLine();
					Console.WriteLine("Press any key to exit...");
					Console.ReadKey();
				}
			});
		}
	}
}
