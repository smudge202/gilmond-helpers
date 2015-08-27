using Compose;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;

namespace CodeGate.Tfs.ApplicationTier
{
	static class Program
	{
		static int Main(params string[] args)
		{
			var app = new CommandLineApplication();

			var config = new ConfigurationBuilder()
				.AddJsonFile("config.json")
				.Build();			

			app.UseServices(services =>
				services
					.AddCli()
					.Configure<Connection>(connection => {
						connection.Host = config.Get("tfsServer");
						var isSecure = false;
						if (bool.TryParse(config.Get("tfsIsSecure"), out isSecure))
							connection.IsSecure = isSecure;
					})
			);

			app.UseDisplayAllProjects();
			return app.Execute();
		}
	}
}
