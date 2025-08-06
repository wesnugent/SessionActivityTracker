# SessionActivityTracker

**SessionActivityTracker** is a lightweight ASP.NET Core middleware and controller 
combo that tracks user activity enabling you to monitor and manage session timeouts effectively.

## Features
- Middleware to track activity timestamps across successful requests.
- API Controller to retrieve the last activity information.
- Configurable idle timeout duration to match your application.
- Works with distributed caching (e.g., Redis, SQL Server, In-Memory).
- Ideal for ASP.NET Core MVC apps that want to show session timeout warnings.

## Getting Started
### 1. Install the NuGet Package
```bash
dotnet add package SessionActivityTracker
```

### 2. Configure Services in `Startup.cs` or `Program.cs`:
```csharp
builder.Services.AddDistributedMemoryCache(); // Or Redis/SQL distributed cache
builder.Services.AddSession();

builder.Services.AddSessionActivityTracking(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(20); // Set your desired timeout
});
```

### 3. Add Middleware
```csharp
app.UseSession();
app.UseSessionActivityTracking();
```

### 4. Prevent Cookie Sliding Expiration on Polling Endpoint
To ensure session timeout detection works accurately, you will need to prevent cookie sliding 
expiration on the polling endpoint.
```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    
    options.SlidingExpiration = true;

    // ... other settings like LoginPath, ExpireTimeSpan, etc.
    
    options.Events.OnCheckSlidingExpiration = context =>
    {
        // Don't refresh cookie expiration when polling for session status
        if (context.Request.Path.StartsWithSegments(SessionActivityTrackingConstants.RemainingTimeEndpoint))
        {
            context.ShouldRenew = false;
        } else
        {
            context.ShouldRenew = true;
        }
        
        return Task.CompletedTask;
    };
});
```

## API Endpoints
### Get Remaining Session Time
```http
GET /api/session/remaining-time
```
#### Response
```json
{
	"RemainingSeconds": 180, // Seconds remaining before session timeout
	"IsExpired": false // Indicates if the session has expired
}
```
### Refresh Idle Timeout
```http
POST /api/session/refresh-time
```
#### Response
```json
{
	"RemainingSeconds": 1200, // Seconds remaining before session timeout
	"IsExpired": false // Indicates if the session has expired
}
```

## License
MIT

## Feedback
If you find any issues or have suggestions, please open an issue or submit a pull request 
on GitHub.
