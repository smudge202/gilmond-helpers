using FluentAssertions;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TestAttributes;

namespace CodeGate.Tfs.ApplicationTier.Tests
{
	public class Tfs2013ListAllProjectsTests
	{
		static TfsServer DefaultTfsServer
		{
			get
			{
				return new Mock<TfsServer>().Object;
			}
		}

		static ILogger DefaultLogger
		{
			get
			{
				return new Mock<ILogger>().Object;
			}
		}

		static ReadOnlyCollection<ProjectCollection> DefaultProjectCollections
		{
			get
			{
				return RandomNumberOf(() => DefaultProjectCollection);
			}
		}

		static ProjectCollection DefaultProjectCollection
		{
			get
			{
				return new ProjectCollection
				{
					Name = RandomString,
					Projects = RandomNumberOf(() => DefaultProject).ToList().AsReadOnly()
				};
			}
		}

		static Project DefaultProject
		{
			get
			{
				return new Project
				{
					Name = RandomString
				};
			}
		}

		static string RandomString
		{
			get
			{
				return Guid.NewGuid().ToString();
			}
		}

		static Random RandomInstance = new Random();
		static ReadOnlyCollection<T> RandomNumberOf<T>(Func<T> selector)
		{
			return Enumerable.Range(0, RandomInstance.Next(1, 10))
				.Select(x => selector())
				.ToList()
				.AsReadOnly();
		}

		public class Constructor
		{
			[Unit]
			public static void WhenTfsServerIsNotProvidedThenThrowsException()
			{
				Action act = () => new DefaultListAllProjects(null, DefaultLogger);
				act.ShouldThrow<ArgumentNullException>();
			}

			[Unit]
			public static void WhenLoggerIsNotProvidedThenThrowsException()
			{
				Action act = () => new DefaultListAllProjects(DefaultTfsServer, null);
				act.ShouldThrow<ArgumentNullException>();
			}
		}

		public class GetProjects
		{
			static ListAllProjects CreateTarget(
				TfsServer tfsServer = null,
				ILogger logger = null)
			{
				return new DefaultListAllProjects(
					tfsServer ?? DefaultTfsServer,
					logger ?? DefaultLogger);
			}

			[Unit]
			public static void WhenCalledThenRequestsCollectionsFromTfsServer()
			{
				var tfsServer = new Mock<TfsServer>();
				tfsServer.Setup(m => m.GetProjectCollections()).Returns(DefaultProjectCollections);

				CreateTarget(tfsServer.Object).DisplayProjects();

				tfsServer.Verify(m => m.GetProjectCollections(), Times.Once);
			}

			[Unit]
			public static void WhenCollectionsReturnedFromTfsServerThenOutputsNamesToLogger()
			{
				var collections = DefaultProjectCollections;
				var tfsServer = new Mock<TfsServer>();
				tfsServer.Setup(m => m.GetProjectCollections()).Returns(collections);
				var logger = new Mock<ILogger>();

				CreateTarget(tfsServer.Object, logger.Object).DisplayProjects();

				foreach (var collection in collections)
					logger.Verify(m => m.Log(
							LogLevel.Information,
							It.IsAny<int>(),
							It.Is<object>(state => state is string && state.ToString().Contains(collection.Name)),
							It.IsAny<Exception>(),
							It.IsAny<Func<object, Exception, string>>()),
						Times.Once);
			}

			[Unit]
			public static void WhenReturnedCollectionContainsProjectsThenOutputsNamesToLogger()
			{
				var collections = DefaultProjectCollections;
				var tfsServer = new Mock<TfsServer>();
				tfsServer.Setup(m => m.GetProjectCollections()).Returns(collections);
				var logger = new Mock<ILogger>();

				CreateTarget(tfsServer.Object, logger.Object).DisplayProjects();

				foreach (var project in collections.SelectMany(x => x.Projects))
					logger.Verify(m => m.Log(
							LogLevel.Information,
							It.IsAny<int>(),
							It.Is<object>(state => state is string && state.ToString().Contains(project.Name)),
							It.IsAny<Exception>(),
							It.IsAny<Func<object, Exception, string>>()),
						Times.Once);
			}
		}
	}
}
