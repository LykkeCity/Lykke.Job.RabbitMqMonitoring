using System;
using System.Collections.Generic;

namespace Lykke.Job.RabbitMqMonitoring.Settings.JobSettings
{
        public class RabbitMqMonitoringSettings
        {
            public DbSettings Db { get; set; }
            public IReadOnlyCollection<RabbitMqConnectionSettings> RabbitMqConnections { get; set; }
            public TimeSpan CheckRate { get; set; }
            public int MaxMessagesCount { get; set; }
    }
}
