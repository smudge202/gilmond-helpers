using Compose;

namespace CodeGate.Tfs.ApplicationTier
{
	static class ApplicationExtensions
	{
		public static void UseDisplayAllProjects(this Executable<int> app)
		{

			app.OnExecute<ListAllProjects>(listAllProjects =>
			{
				return 0;
			});
		}
	}
}
