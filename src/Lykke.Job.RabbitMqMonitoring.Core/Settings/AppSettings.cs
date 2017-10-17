using Lykke.Job.RabbitMqMonitoring.Core.Settings.JobSettings;
using Lykke.Job.RabbitMqMonitoring.Core.Settings.SlackNotifications;

namespace Lykke.Job.RabbitMqMonitoring.Core.Settings
{
    public class AppSettings
    {
        public RabbitMqMonitoringSettings RabbitMqMonitoringJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
