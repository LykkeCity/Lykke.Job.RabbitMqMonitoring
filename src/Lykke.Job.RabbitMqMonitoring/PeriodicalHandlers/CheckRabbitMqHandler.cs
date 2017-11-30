using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly RabbitMqConnectionSettings[] _rabbitMqConnectionSettings;
        private readonly int _maxMessagesCount;
        private readonly ILog _log;

        public CheckRabbitMqHandler(
            IRabbitMqManagementService rabbitMqManagementService,
            RabbitMqConnectionSettings[] rabbitMqConnectionSettings, 
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
            var tasks = new List<Task>();

            foreach (var connectionSettings in _rabbitMqConnectionSettings)
            {
                tasks.Add(ProcessConnectionAsync(connectionSettings));
            }

            await Task.WhenAll(tasks);
        }

        private async Task ProcessConnectionAsync(RabbitMqConnectionSettings connectionSettings)
        {
            try
            {
                var queues = await _rabbitMqManagementService.GetQueuesAsync(connectionSettings.Url, connectionSettings.Username, connectionSettings.Password);

                foreach (var queue in queues)
                {
                    await ProcessQueueAsync(connectionSettings, queue);
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(Execute), connectionSettings.Url, ex);
            }
        }

        private async Task ProcessQueueAsync(RabbitMqConnectionSettings connectionSettings, RabbitMqQueue queue)
        {
            var queueSettings = TryGetQueueSettings(connectionSettings, queue);
            var maxMessagesCount = queueSettings?.MaxMessagesCount ??
                                   connectionSettings.MaxMessagesCount ?? 
                                   _maxMessagesCount;

            if (queue.Messages >= maxMessagesCount)
            {
                var title = connectionSettings.Title ?? new Uri(connectionSettings.Url).Host;
                
                await _log.WriteMonitorAsync(
                    title,
                    string.Empty,
                    string.Empty,
                    $"Queue '{queue.Name}' contains {queue.Messages} messages");
            }
        }

        private static RabbitMqQueueSettings TryGetQueueSettings(RabbitMqConnectionSettings connectionSettings, RabbitMqQueue queue)
        {
            if (connectionSettings.Queues != null)
            {
                connectionSettings.Queues.TryGetValue(queue.Name, out var queueSettings);

                return queueSettings;
            }

            return null;
        }
    }
}
