namespace MyFreeFormForm.Services
{

    public class QueueProcessorMonitor : IQueueProcessorMonitor
    {
        public bool IsHealthy { get; set; } = true;
    }
}
