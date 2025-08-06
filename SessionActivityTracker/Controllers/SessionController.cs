using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SessionActivityTracker.Code;

namespace SessionActivityTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly SessionTimeTracker _tracker;
        private readonly SessionActivityTrackingOptions _options;
        public SessionController(SessionTimeTracker tracker, IOptions<SessionActivityTrackingOptions> options)
        {
            _tracker = tracker;
            _options = options.Value;
        }

        /// <summary>
        /// Retrieves the remaining session time for the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("remaining-time")]
        public async Task<IActionResult> GetRemainingTime()
        {
            string? userKey = HttpContext.Session.Id;
            if (string.IsNullOrEmpty(userKey)) return BadRequest("No active session");

            SessionMetadata? metadata = await _tracker.GetSessionMetadataAsync(userKey);
            if (metadata == null) return BadRequest("Session metadata not found");

            TimeSpan timeElapsed = DateTime.UtcNow - metadata.LastAccess;
            TimeSpan remainingTime = TimeSpan.FromSeconds(metadata.TimeoutSeconds) - timeElapsed;

            return Ok(new
            {
                RemainingSeconds = Math.Max(0, (int)remainingTime.TotalSeconds),
                IsExpired = remainingTime <= TimeSpan.Zero
            });
        }

        /// <summary>
        /// Refreshes the session time for the current user by updating the last access time.
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh-time")]
        public async Task<IActionResult> RefreshTime()
        {
            string? userKey = HttpContext.Session.Id;
            if (string.IsNullOrEmpty(userKey)) return BadRequest("No active session");

            await _tracker.UpdateLastAccessAsync(userKey, _options.IdleTime);

            return await GetRemainingTime();
        }
    }
}
