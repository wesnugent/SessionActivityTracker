using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SessionActivityTracker.Code
{
    public class SessionTimeTracker
    {
        private readonly IDistributedCache _cache;
        public SessionTimeTracker(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Updates the last access time for a user session in the distributed cache.
        /// </summary>
        /// <param name="userKey"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task UpdateLastAccessAsync(string userKey, TimeSpan timeout)
        {
            string key = $"session_meta:{userKey}";
            SessionMetadata metadata = new SessionMetadata
            {
                LastAccess = DateTime.UtcNow,
                TimeoutSeconds = (int)timeout.TotalSeconds
            };

            string json = JsonSerializer.Serialize(metadata);
            await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                // Expire slightly longer than session time
                SlidingExpiration = timeout.Add(TimeSpan.FromMinutes(1)),
            });
        }

        /// <summary>
        /// Retrieves the session metadata for a user from the distributed cache.
        /// </summary>
        /// <param name="userKey"></param>
        /// <returns></returns>
        public async Task<SessionMetadata?> GetSessionMetadataAsync(string userKey)
        {
            string key = $"session_meta:{userKey}";
            string? json = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(json)) return null;

            return JsonSerializer.Deserialize<SessionMetadata>(json);
        }
    }
}
