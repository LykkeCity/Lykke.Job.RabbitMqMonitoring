using System.Collections.Generic;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.RabbitMqMonitoring.Settings.JobSettings
{
    public class RabbitMqConnectionSettings
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [Optional]
        public string Title { get; set; }
        [Optional]
        public int? MaxMessagesCount { get; set; }
        [Optional]
        public IReadOnlyDictionary<string, RabbitMqQueueSettings> Queues { get; set; }

    }
}
