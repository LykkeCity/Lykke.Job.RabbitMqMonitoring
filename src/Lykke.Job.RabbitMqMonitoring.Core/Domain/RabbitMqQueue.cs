namespace Lykke.Job.RabbitMqMonitoring.Core.Domain
{
    public class RabbitMqQueue
    {
        public long Messages { get; set; }
        public string Name { get; set; }
        public long Memory { get; set; }
    }
}
