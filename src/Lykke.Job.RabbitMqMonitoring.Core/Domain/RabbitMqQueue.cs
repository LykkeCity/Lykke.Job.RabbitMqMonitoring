namespace Lykke.Job.RabbitMqMonitoring.Core.Domain
{
    public class RabbitMqQueue
    {
        public int Messages { get; set; }
        public string Name { get; set; }
        public int Memory { get; set; }
    }
}
