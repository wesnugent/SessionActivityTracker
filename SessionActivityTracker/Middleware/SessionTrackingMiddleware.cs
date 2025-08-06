using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SessionActivityTracker.Code;

namespace SessionActivityTracker.Middleware
{
    public class SessionTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SessionTimeTracker _tracker;
        private readonly SessionActivityTrackingOptions _options;

        public SessionTrackingMiddleware(RequestDelegate next, SessionTimeTracker tracker, IOptions<SessionActivityTrackingOptions> options)
        {
            _next = next;
            _tracker = tracker;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Only track on successful requests not includiong the last access endpoint
            if (context.Request.Path.StartsWithSegments(SessionActivityTrackingConstants.RemainingTimeEndpoint))
            {
                return;
            }
            if (context.Response.StatusCode < 400)
            {
                string? userKey = context.Session.Id;
                if (!string.IsNullOrEmpty(userKey))
                {
                    await _tracker.UpdateLastAccessAsync(userKey, _options.IdleTime);
                }
            }
        }
    }
}
