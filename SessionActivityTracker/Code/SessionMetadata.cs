namespace SessionActivityTracker.Code
{
    /// <summary>
    /// Represents metadata for a session, including the last access time and timeout duration.
    /// </summary>
    public record SessionMetadata
    {
        public DateTime LastAccess { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
