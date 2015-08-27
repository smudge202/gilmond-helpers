using System;

namespace CodeGate.Tfs.ApplicationTier
{
	class Connection
	{
		public bool IsSecure { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public string Path { get; set; }
		public Uri Uri
		{
			get
			{
				var uriBuilder = new UriBuilder();
				uriBuilder.Scheme = IsSecure ? "https" : "http";
				uriBuilder.Host = Host;
				uriBuilder.Port = Port;
				uriBuilder.Path = Path;
				return uriBuilder.Uri;
			}
		}
	}
}
