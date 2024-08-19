internal class QuartzConfiguratorConfig
{
    public int Recurrence { get; set; }
    public string JobName { get; set; }
    public string Cron { get; set; }
    public int InitialStartDelaySeconds { get; set; }
}