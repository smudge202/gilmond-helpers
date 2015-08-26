using Compose;
using Microsoft.Framework.DependencyInjection;

namespace Cli
{
    public class Program
    {
        public void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.UseServices(services =>
                services.AddTransient<object>()
            );
        }
    }
}
