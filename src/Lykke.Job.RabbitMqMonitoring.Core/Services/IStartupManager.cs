using System.Threading.Tasks;

namespace Lykke.Job.RabbitMqMonitoring.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}