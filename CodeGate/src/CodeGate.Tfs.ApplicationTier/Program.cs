﻿using Compose;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System.IO;
using System.Reflection;

namespace CodeGate.Tfs.ApplicationTier
{
	static class Program
	{
		static int Main(params string[] args)
		{
			var app = new CommandLineApplication();

			var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var config = new ConfigurationBuilder(basePath)
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
