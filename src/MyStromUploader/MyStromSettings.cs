internal class MyStromSettings
{
    public List<MyStromSwitch> MyStromSwitches { get; set; } = new List<MyStromSwitch>();
    public CloudIngressConfig CloudIngressConfig { get; set; } = new CloudIngressConfig();
}

internal class CloudIngressConfig
{
    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }
}

internal class MyStromSwitch
{
    public string Name { get; set; }
    public string ID { get; set; }
    public string IpAddress { get; set; }
    public string ApiPath { get; set; }
}