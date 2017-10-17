using System;
using System.Threading.Tasks;
using Common.Log;
using Flurl.Http;
using Lykke.Job.RabbitMqMonitoring.Core.Domain;
using Lykke.Job.RabbitMqMonitoring.Core.Services;

namespace Lykke.Job.RabbitMqMonitoring.Services
{
    public class RabbitMqManagementService : IRabbitMqManagementService
    {
        private readonly ILog _log;

        public RabbitMqManagementService(ILog log)
        {
            _log = log;
        }

        public async Task<RabbitMqQueue[]> GetQueuesAsync(string url, string username, string password)
        {
            try
            {
                return await $"{url}/api/queues"
                    .WithBasicAuth(username, password)
                    .GetJsonAsync<RabbitMqQueue[]>();
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(RabbitMqManagementService), nameof(GetQueuesAsync), url, ex);
            }

            return Array.Empty<RabbitMqQueue>();
        }
    }
}
