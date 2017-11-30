using Lykke.Job.RabbitMqMonitoring.Settings.JobSettings;
using Lykke.Job.RabbitMqMonitoring.Settings.SlackNotifications;

namespace Lykke.Job.RabbitMqMonitoring.Settings
{
    public class AppSettings
    {
        public RabbitMqMonitoringSettings RabbitMqMonitoringJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
