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

            services.AddLogging(builder => { builder.AddConsole(); });
        })
        .ConfigureServices((hostContext, services) =>
        {
            // see Quartz.Extensions.DependencyInjection documentation about how to configure different configuration aspects
            services.AddQuartz(q =>
            {
                // Register the job, loading the schedule from configuration
                q.AddJob<MyStromUploader.MyStromUploader>(j => j
                    .WithIdentity("MyStromUploader")
                    .Build());

                // Create a trigger for the job
                q.AddTrigger(t => t
                    .ForJob("MyStromUploader")
                    .WithIdentity("MyStromUploaderTrigger")
                    .StartNow()
                    .WithCronSchedule(hostContext.Configuration["QuartzConfiguratorConfig:Cron"])
                );
            });

            // Quartz.Extensions.Hosting hosting
            services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });
        });
}