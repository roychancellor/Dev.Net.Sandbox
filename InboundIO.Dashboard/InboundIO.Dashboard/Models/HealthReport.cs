namespace InboundIO.Dashboard.Models
{
    public enum HealthStatus
    {
        Healthy,
        Degraded,
        Unhealthy,
        UNREACHABLE,
    }

    public class HealthReport
    {
        public bool IsHealthy { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public Dictionary<string, string> DependenciesHealth { get; set; } = [];
    }

}
