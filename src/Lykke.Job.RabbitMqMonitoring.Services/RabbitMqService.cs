using System.Threading.Tasks;
using Flurl.Http;
using Lykke.Job.RabbitMqMonitoring.Core.Domain;
using Lykke.Job.RabbitMqMonitoring.Core.Services;

namespace Lykke.Job.RabbitMqMonitoring.Services
{
    public class RabbitMqManagementService : IRabbitMqManagementService
    {
        public async Task<RabbitMqQueue[]> GetQueues(string url, string username, string password)
        {
            return await $"{url}/api/queues"
                .WithBasicAuth(username, password)
                .GetJsonAsync<RabbitMqQueue[]>();
        }
    }
}
