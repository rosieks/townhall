using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Townhall
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddUserSecrets("B243CFE0-D4DE-4B21-929D-69E059D3575F");
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((context,services) =>
                {
                    services.AddOptions();
                    services.Configure<Configuration>(context.Configuration);
                    services.AddSingleton<IHostedService, ReadD2CMessagesService>();
                    services.AddSingleton<EventsProcessor>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }
    }
}
