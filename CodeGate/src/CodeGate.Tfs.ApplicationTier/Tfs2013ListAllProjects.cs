using Microsoft.Framework.Logging;
using System;

namespace CodeGate.Tfs.ApplicationTier
{
	sealed class Tfs2013ListAllProjects : ListAllProjects
	{
		readonly TfsServer _tfsServer;
		readonly ILogger _logger;

		public Tfs2013ListAllProjects(TfsServer tfsServer, ILogger logger)
		{
			if (tfsServer == null)
				throw new ArgumentNullException(nameof(tfsServer));
			if (logger == null)
				throw new ArgumentNullException(nameof(logger));
			_tfsServer = tfsServer;
			_logger = logger;
		}

		// reference material: 
		// * https://msdn.microsoft.com/en-us/library/bb130146.aspx
		// * https://msdn.microsoft.com/en-us/library/bb286958.aspx
		public void DisplayProjects()
		{
			foreach (var collection in _tfsServer.GetProjectCollections())
				_logger.LogInformation(collection.Name);
		}
	}
}
