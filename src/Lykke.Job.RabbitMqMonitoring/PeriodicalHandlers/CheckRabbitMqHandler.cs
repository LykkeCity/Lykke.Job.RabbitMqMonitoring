using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.RabbitMqMonitoring.Core.Services;
using Lykke.Job.RabbitMqMonitoring.Core.Settings.JobSettings;

namespace Lykke.Job.RabbitMqMonitoring.PeriodicalHandlers
{
    public class CheckRabbitMqHandler : TimerPeriod
    {
        private readonly IRabbitMqManagementService _rabbitMqManagementService;
        private readonly RabbitMqConnection[] _rabbitMqConnections;
        private readonly int _maxMessagesCount;
        private readonly ILog _log;

        public CheckRabbitMqHandler(
            IRabbitMqManagementService rabbitMqManagementService,
            RabbitMqConnection[] rabbitMqConnections, 
            TimeSpan checkRate, 
            int maxMessagesCount, 
            ILog log) :
            base(nameof(CheckRabbitMqHandler), (int)checkRate.TotalMilliseconds, log)
        {
            _rabbitMqManagementService = rabbitMqManagementService;
            _rabbitMqConnections = rabbitMqConnections;
            _maxMessagesCount = maxMessagesCount;
            _log = log;
        }

        public override async Task Execute()
        {
            var tasks = new List<Task>();

            foreach (var rabbitMq in _rabbitMqConnections)
            {
                tasks.Add(ProcessConnection(rabbitMq));
            }

            await Task.WhenAll(tasks);
        }

        private async Task ProcessConnection(RabbitMqConnection rabbitMq)
        {
            try
            {
                var queues = await _rabbitMqManagementService.GetQueuesAsync(rabbitMq.Url, rabbitMq.Username, rabbitMq.Password);

                foreach (var queue in queues.Where(item => item.Messages >= _maxMessagesCount))
                {
                    await _log.WriteMonitorAsync(new Uri(rabbitMq.Url).Host, string.Empty, string.Empty, $"Queue '{queue.Name}' contains {queue.Messages} messages");
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(Execute), rabbitMq.Url, ex);
            }
        }
    }
}
