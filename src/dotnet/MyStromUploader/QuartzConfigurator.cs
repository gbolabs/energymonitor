using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;

internal class QuartzConfigurator : IHostedService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly QuartzConfiguratorConfig _config;
    private IScheduler _scheduler;

    public QuartzConfigurator(ISchedulerFactory schedulerFactory, IOptions<QuartzConfiguratorConfig> config)
    {
        // argument checks
        _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
        _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var job = JobBuilder.Create<MyStromUploader>()
            .WithIdentity("MyStromUploader")
            .Build();

        // Create a trigger to run now and then every 5 minutes
        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithCronSchedule(_config.Cron)
            .WithIdentity("MyStromUploaderTrigger")
            .StartAt(DateTimeOffset.Now.AddSeconds(_config.InitialStartDelaySeconds))
            .Build();

        await _scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _scheduler.Shutdown(cancellationToken);
    }
}