﻿using Compose;
using System;

namespace Gilmond.Helpers.ListReferences.CommandLine
{
	sealed class Program
	{
		static void Main(string[] args)
		{
			var app = new CommandLineApplication();
			app.UseServices(services => 
				services
					.AddCoreServices()
					.AddConsoleServices()
					.ConfigureArguments(args)
			);
			app.ListReferences();
			try
			{
				app.Execute();
			}
			finally
			{
				Console.ReadKey();
			}
		}
	}
}
