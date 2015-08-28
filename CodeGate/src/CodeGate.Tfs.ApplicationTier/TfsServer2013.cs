using Microsoft.Framework.OptionsModel;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.Framework.Logging;

namespace CodeGate.Tfs.ApplicationTier
{
	// reference material: 
	// * https://msdn.microsoft.com/en-us/library/bb130146.aspx
	// * https://msdn.microsoft.com/en-us/library/bb286958.aspx
	sealed class TfsServer2013 : TfsServer
	{
		readonly IOptions<Connection> _config;
		readonly ILogger _logger;
		
		TfsConfigurationServer _configurationServer;

		public TfsServer2013(IOptions<Connection> config, ILogger logger)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));
			if (logger == null)
				throw new ArgumentNullException(nameof(logger));
			_config = config;
			_logger = logger;
		}

		public ReadOnlyCollection<ProjectCollection> GetProjectCollections()
		{
			_configurationServer = TfsConfigurationServerFactory.GetConfigurationServer(_config.Options.Uri);
			var collectionNodes = _configurationServer.CatalogNode
				.QueryChildren(new[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None);
			return collectionNodes.Select(BuildProjectCollection).ToList().AsReadOnly();
		}

		ProjectCollection BuildProjectCollection(CatalogNode collectionNode)
		{
			Guid collectionId;
			var collectionInstanceId = collectionNode.Resource.Properties["InstanceId"];
			if (!Guid.TryParse(collectionInstanceId, out collectionId))
				return FailedProjectCollection(collectionInstanceId);
			var projectCollection = _configurationServer.GetTeamProjectCollection(collectionId);
			var projectNodes = collectionNode
				.QueryChildren(new[] { CatalogResourceTypes.TeamProject }, false, CatalogQueryOptions.None);
			return new ProjectCollection
			{
				Name = projectCollection.Name,
				Projects = projectNodes.Select(BuildProject).ToList().AsReadOnly()
			};
		}

		Project BuildProject(CatalogNode projectNode)
		{
			return new Project { Name = projectNode.Resource.DisplayName };
		}

		ProjectCollection FailedProjectCollection(string instanceId)
		{
			_logger.LogWarning($"Unable to load details for Collection Node with Instance Id: {instanceId}");
			return new ProjectCollection { Name = instanceId, Projects = Enumerable.Empty<Project>().ToList().AsReadOnly() };
		}
	}
}
