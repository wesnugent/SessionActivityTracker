using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SessionActivityTracker.Code;
using SessionActivityTracker.Middleware;

namespace SessionActivityTracker.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSessionActivityTracking(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SessionTrackingMiddleware>();
        }

        public static IServiceCollection AddSessionActivityTracking(this IServiceCollection services, Action<SessionActivityTrackingOptions>? configure = null)
        {
            services.Configure(configure ?? (_ => { }));
            services.AddSingleton<SessionTimeTracker>();

            return services;
        }
    }
}
