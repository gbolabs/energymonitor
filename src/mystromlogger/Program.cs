// See https://aka.ms/new-console-template for more information

var myStromIp = Environment.GetEnvironmentVariable("MYSTROMLOGGER_IP");
var target = Environment.GetEnvironmentVariable("MYSTROMLOGGER_TARGET");
var objectId = Environment.GetEnvironmentVariable("MYSTROMLOGGER_OBJECTID");

Console.WriteLine($"{nameof(myStromIp)}: {myStromIp}");
Console.WriteLine($"{nameof(target)}: {target}");
Console.WriteLine($"{nameof(objectId)}: {objectId}");

var httpClient = new HttpClient();

var url = $"http://{myStromIp}/report";
var body = httpClient.GetStringAsync(url).GetAwaiter().GetResult();

var targetUrl = $"http://{target}/api/v2/measures/mystrom/upload/{objectId}";

var postresult = httpClient.PostAsync(targetUrl, new StringContent(body)).GetAwaiter().GetResult();