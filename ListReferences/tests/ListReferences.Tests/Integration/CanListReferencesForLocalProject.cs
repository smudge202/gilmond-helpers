using Compose;
using TestAttributes;

namespace Gilmond.Helpers.ListReferences.Tests.Integration
{
	public partial class IntegrationTests
	{
		[Integration]
		public static void CanListReferencesForLocalProject()
		{
			var app = new TestExecutable();
			app.UseServices(services => services
				.ConfigureForLocalProject()
				.AddCoreServices()
				.AddTraceLogger());
			app.OnExecute<ListReferences>(RunTest);
			app.Execute();
		}

		private static void RunTest(ListReferences process)
		{
			var results = process.GetDistinctReferences();
		}
	}
}
