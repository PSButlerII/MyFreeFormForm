using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyFreeFormForm.Services
{
    public class QueueProcessorHealthCheck : IHealthCheck
    {
       
        private readonly IQueueProcessorMonitor _monitor;

        public QueueProcessorHealthCheck(IQueueProcessorMonitor monitor)
        {
            _monitor = monitor;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (_monitor.IsHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("QueueProcessor is healthy."));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("QueueProcessor is not healthy."));
            }
        }
    }

}
