namespace EvrenDev.Infrastructure.Notifications;

public class SignalRSettings
{
    public bool UseBackplane { get; set; }

    public class Backplane
    {
        public string? Provider { get; set; }
        public string? StringConnection { get; set; }
    }
}
