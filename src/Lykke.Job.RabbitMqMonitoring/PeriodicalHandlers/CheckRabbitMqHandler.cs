using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.RabbitMqMonitoring.Core.Domain;
using Lykke.Job.RabbitMqMonitoring.Core.Services;
using Lykke.Job.RabbitMqMonitoring.Settings.JobSettings;

namespace Lykke.Job.RabbitMqMonitoring.PeriodicalHandlers
{
    public class CheckRabbitMqHandler : TimerPeriod
    {
        private readonly IRabbitMqManagementService _rabbitMqManagementService;
        private readonly IReadOnlyCollection<RabbitMqConnectionSettings> _rabbitMqConnectionSettings;
        private readonly int _maxMessagesCount;
        private readonly ILog _log;

        public CheckRabbitMqHandler(
            IRabbitMqManagementService rabbitMqManagementService,
            IReadOnlyCollection<RabbitMqConnectionSettings> rabbitMqConnectionSettings,
            TimeSpan checkRate,
            int maxMessagesCount,
            ILog log) :
            base(nameof(CheckRabbitMqHandler), (int)checkRate.TotalMilliseconds, log)
        {
            _rabbitMqManagementService = rabbitMqManagementService;
            _rabbitMqConnectionSettings = rabbitMqConnectionSettings;
            _maxMessagesCount = maxMessagesCount;
            _log = log;
        }

        public override async Task Execute()
        {
            var tasks = _rabbitMqConnectionSettings.Select(ProcessConnectionAsync);

            await Task.WhenAll(tasks);
        }

        private async Task ProcessConnectionAsync(RabbitMqConnectionSettings connectionSettings)
        {
            try
            {
                var queues = await _rabbitMqManagementService.GetQueuesAsync(connectionSettings.Url, connectionSettings.Username, connectionSettings.Password);

                foreach (var queue in queues)
                {
                    ProcessQueue(connectionSettings, queue);
                }
            }
            catch (Exception ex)
            {
                _log.WriteError(nameof(Execute), connectionSettings.Url, ex);
            }
        }

        private void ProcessQueue(RabbitMqConnectionSettings connectionSettings, RabbitMqQueue queue)
        {
            var queueSettings = TryGetQueueSettings(connectionSettings, queue);
            var maxMessagesCount = queueSettings?.MaxMessagesCount ??
                                   connectionSettings.MaxMessagesCount ??
                                   _maxMessagesCount;

            if (queue.Messages >= maxMessagesCount)
            {
                var title = connectionSettings.Title ?? new Uri(connectionSettings.Url).Host;

                _log.WriteMonitor(
                    title,
                    string.Empty,
                    $"Queue '{queue.Name}' contains {queue.Messages} messages");
            }
        }

        private static RabbitMqQueueSettings TryGetQueueSettings(RabbitMqConnectionSettings connectionSettings, RabbitMqQueue queue)
        {
            if (connectionSettings.Queues != null)
            {
                connectionSettings.Queues.TryGetValue(queue.Name, out var queueSettings);

                if (queueSettings == null)
                {
                    queueSettings = CheckByRegExp(connectionSettings, queue.Name);
                }

                return queueSettings;
            }

            return null;
        }

        private static RabbitMqQueueSettings CheckByRegExp(RabbitMqConnectionSettings connectionSettings, string queueName)
        {
            var queueNames = connectionSettings.Queues.Keys.ToList();

            foreach (var queue in queueNames)
            {
                if (Regex.IsMatch(queueName, queue))
                    return connectionSettings.Queues[queue];
            }

            return null;
        }
    }
}
