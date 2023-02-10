using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var hostBuilder = new HostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddEnvironmentVariables(prefix: "DOTNET_");
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment.EnvironmentName;
                config.AddJsonFile("appsettings.json");
                config.AddJsonFile($"appsettings.{env}.json", optional: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOptions();
                services.Configure<MyStromSettings>(hostContext.Configuration.GetSection(nameof(MyStromSettings)));
                services.Configure<CloudIngressConfig>(
                    hostContext.Configuration.GetSection(nameof(CloudIngressConfig)));
                services.AddHostedService<MyStromUploader>();
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    //TODO: Add Application Insights
                    // builder.AddApplicationInsights(hostContext.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
                });
            });
        await hostBuilder.Build().RunAsync();
    }
}