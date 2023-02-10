using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class MyStromUploader : IHostedService
{
    private readonly MyStromSettings _settings;
    private readonly ILogger<MyStromUploader> _logger;
    private readonly CloudIngressConfig _cloudIngressConfig;

    public MyStromUploader(IOptions<MyStromSettings> settings,
        IOptions<CloudIngressConfig> cloudIngressConfig,
        ILogger<MyStromUploader> logger)
    {
        _settings = settings.Value;
        _cloudIngressConfig = cloudIngressConfig.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Get the report from the MyStrom switch over the REST API
        using var httpClient = new HttpClient();
        var request = $"http://{_settings.MyStromSwitches[0].IpAddress}/report";
        var response = await httpClient.GetAsync(request);
        _logger.LogInformation($"Response: {await response.Content.ReadAsStringAsync()}");
        _logger.LogInformation($"Status code: {response.StatusCode}");

        var report = await response.Content.ReadFromJsonAsync<MyStromReport>();

        // Add the API key to the header
        httpClient.DefaultRequestHeaders.Add("X-Api-Key", _cloudIngressConfig.ApiKey);

        // Upload the report to the cloud
        var uploadRequest = _cloudIngressConfig.BaseUrl + _settings.MyStromSwitches[0].ApiPath;
        // json content
        var content = JsonContent.Create(new
        {
            sampling = DateTime.Now,
            power = report.power,
            Ws = report.Ws
        });
        var uploadResponse = await httpClient.PostAsync(uploadRequest, content);
        _logger.LogInformation($"Upload response: {await uploadResponse.Content.ReadAsStringAsync()}");
        _logger.LogInformation($"Upload status code: {uploadResponse.StatusCode}");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        return;
    }
}

internal class MyStromReport
{
    public decimal power { get; set; }
    public decimal Ws { get; set; }
}