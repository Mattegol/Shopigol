using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Unity.Microsoft.DependencyInjection;

namespace Shopigol.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUnityServiceProvider()
                .UseStartup<Startup>()
                .Build();
    }
}
