using Compose;

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
