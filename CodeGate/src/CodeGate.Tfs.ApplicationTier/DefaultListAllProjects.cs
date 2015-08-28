using Microsoft.Framework.Logging;
using System;

namespace CodeGate.Tfs.ApplicationTier
{
	sealed class DefaultListAllProjects : ListAllProjects
	{
		readonly TfsServer _tfsServer;
		readonly ILogger _logger;

		public DefaultListAllProjects(TfsServer tfsServer, ILogger logger)
		{
			if (tfsServer == null)
				throw new ArgumentNullException(nameof(tfsServer));
			if (logger == null)
				throw new ArgumentNullException(nameof(logger));
			_tfsServer = tfsServer;
			_logger = logger;
		}

		public void DisplayProjects()
		{
			foreach (var collection in _tfsServer.GetProjectCollections())
				OutputCollectionDetails(collection);				
		}

		void OutputCollectionDetails(ProjectCollection collection)
		{
			_logger.LogInformation(collection.Name);
			foreach (var project in collection.Projects)
				_logger.LogInformation($"\t{project.Name}");
		}
	}
}
