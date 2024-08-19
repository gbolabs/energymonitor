using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

internal class MyStromUploader : IJob
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

    private async Task<List<MyStromReport>> GetMyStromReports(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));

        // Get the report from the MyStrom switch over the REST API
        var reports = new List<MyStromReport>();
        foreach (var stromSwitch in _settings.MyStromSwitches)
        {
            var request = $"http://{stromSwitch.IpAddress}/report";

            try
            {
                var response = await httpClient.GetAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                var statusCode = response.StatusCode;
                _logger.LogInformation("Response: {Response}", responseContent);
                _logger.LogInformation("Status code: {responseContent}", responseContent);

                var r = await response.Content.ReadFromJsonAsync<MyStromReport>();
                if (r != null)
                    reports.Add(r);
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to get report from MyStrom switch: {e}", e);
            }
        }

        return reports;
    }

    /// <summary>
    /// Method invoked by the timer
    /// </summary>
    private async Task Upload()
    {
        var httpClient = new HttpClient();
        var reports = await GetMyStromReports(httpClient);

        if (!reports.Any(r => r.Power > 0))
        {
            // No power consumption, no need to upload
            _logger.LogInformation("No power production, no need to upload");
            return;
        }

        var power = reports.Sum(r => r.Power);
        var ws = reports.Sum(r => r.Ws);

        var report = new MyStromReport
        {
            Power = power,
            Ws = ws
        };

        _logger.LogInformation("Power: {power}", power);
        _logger.LogInformation("Ws: {ws}", ws);

        // Add the API key to the header
        httpClient.DefaultRequestHeaders.Add("X-Api-Key", _cloudIngressConfig.ApiKey);

        // Upload the report to the cloud
        var uploadRequest = _cloudIngressConfig.BaseUrl + _settings.MyStromSwitches[0].ApiPath;
        // json content
        var content = JsonContent.Create(new
        {
            sampling = DateTime.Now,
            power = report.Power,
            report.Ws
        });
        var uploadResponse = await httpClient.PostAsync(uploadRequest, content);
        var responseContent = uploadResponse.Content.ReadAsStringAsync();
        var uploadStatusCode = uploadResponse.StatusCode;
        _logger.LogInformation("Upload response: {uploadContent}", responseContent);
        _logger.LogInformation("Upload status code: {uploadStatusCode}", uploadStatusCode);
    }

    public async Task Execute(IJobExecutionContext context) => await Upload();
}

internal class MyStromReport
{
    public decimal Power { get; set; }
    public decimal Ws { get; set; }
}
