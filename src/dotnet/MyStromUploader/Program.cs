using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var hostBuilder = CreateHostBuilder(args).Build();
        await hostBuilder.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
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
            services.Configure<QuartzConfiguratorConfig>(
                hostContext.Configuration.GetSection(nameof(QuartzConfiguratorConfig)));
            services.Configure<MyStromSettings>(hostContext.Configuration.GetSection(nameof(MyStromSettings)));
            services.Configure<CloudIngressConfig>(
                hostContext.Configuration.GetSection(nameof(CloudIngressConfig)));

            services.AddHostedService<QuartzConfigurator>();
            services.AddQuartz(q => { q.UseMicrosoftDependencyInjectionJobFactory(); });
            services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });

            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
        });
}