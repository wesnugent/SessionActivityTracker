namespace SessionActivityTracker
{
    public class SessionActivityTrackingOptions
    {
        public TimeSpan IdleTime { get; set; } = TimeSpan.FromMinutes(20);
    }
}
