using System;

namespace Lykke.Job.RabbitMqMonitoring.Core.Settings.JobSettings
{
        public class RabbitMqMonitoringSettings
        {
            public DbSettings Db { get; set; }
            public RabbitMqConnection[] RabbitMqConnections { get; set; }
            public TimeSpan CheckRate { get; set; }
            public int MaxMessagesCount { get; set; }
    }
}
