using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System;

namespace ingress_function
{
    public static class priv114_em_ingress
    {
        [FunctionName("priv114_em_ingress")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            [CosmosDB(databaseName: "powerMeter_Measures", "RawMeasures")]
            IAsyncCollector<PowerMeasure> output,
            ILogger log)
        {
            if (req is null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (log is null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            try
            {
                var payload = await new StreamReader(req.Body).ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(payload))
                {
                    log.LogError($"No body has been submitted.");
                    return new BadRequestResult();
                }
                else
                {
                    log.LogDebug(payload);
                }

                var data = JsonConvert.DeserializeObject<CosmosDbPowerMeasure>(payload);

                if (data == null)
                {
                    log.LogError(60, $"Unable to decode request body to {nameof(PowerMeasure)}");
                    log.LogDebug(160, message: $"Request body: {payload}");
                    return new BadRequestResult();
                }

                await output.AddAsync(data);
                return new OkObjectResult(data);
            }
            catch (Exception exc)
            {
                log.LogCritical(90, exc, exc.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
