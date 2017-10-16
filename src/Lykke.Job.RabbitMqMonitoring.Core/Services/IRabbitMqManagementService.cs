using System.Threading.Tasks;
using Lykke.Job.RabbitMqMonitoring.Core.Domain;

namespace Lykke.Job.RabbitMqMonitoring.Core.Services
{
    public interface IRabbitMqManagementService
    {
        Task<RabbitMqQueue[]> GetQueues(string url, string username, string password);
    }
}
