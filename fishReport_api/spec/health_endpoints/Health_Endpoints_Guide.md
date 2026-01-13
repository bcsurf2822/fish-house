# Fish House API - Health Endpoints Setup Guide
## Configure Health Checks for Elastic Beanstalk & Database

---

## 🎯 Why You Need Health Endpoints

Elastic Beanstalk's load balancer checks your app health by hitting a specific URL (default: `HTTP:80/`).

**If your app doesn't respond at `/`:**
- Load balancer marks instances as "unhealthy"
- Can't route traffic properly
- May cause 502/503 errors

---

## ✅ Solution 1: Simple Health Endpoint (Recommended)

### Step 1: Create a Health Controller

Create a new file: `Controllers/HealthController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishHouseAPI.Controllers
{
    [ApiController]
    [Route("")]  // Root path for load balancer
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly YourDbContext _dbContext;  // Replace with your actual DbContext name

        public HealthController(
            ILogger<HealthController> logger,
            YourDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Basic health check endpoint at root path
        /// Used by Elastic Beanstalk Load Balancer
        /// </summary>
        [HttpGet("")]
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                service = "Fish House API",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }

        /// <summary>
        /// Database health check endpoint
        /// Tests connection to RDS SQL Server
        /// </summary>
        [HttpGet("health/database")]
        public async Task<IActionResult> DatabaseHealthCheck()
        {
            try
            {
                // Try to connect to database
                var canConnect = await _dbContext.Database.CanConnectAsync();
                
                if (canConnect)
                {
                    // Optional: Get database name to confirm correct DB
                    var connectionString = _dbContext.Database.GetConnectionString();
                    var dbName = _dbContext.Database.GetDbConnection().Database;

                    return Ok(new
                    {
                        status = "healthy",
                        database = "connected",
                        databaseName = dbName,
                        timestamp = DateTime.UtcNow
                    });
                }
                
                _logger.LogWarning("Database health check failed: Cannot connect");
                return StatusCode(503, new
                {
                    status = "unhealthy",
                    database = "disconnected",
                    message = "Cannot connect to database"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed with exception");
                return StatusCode(503, new
                {
                    status = "unhealthy",
                    database = "error",
                    message = ex.Message,
                    type = ex.GetType().Name
                });
            }
        }

        /// <summary>
        /// Detailed health check with all dependencies
        /// </summary>
        [HttpGet("health/detailed")]
        public async Task<IActionResult> DetailedHealthCheck()
        {
            var healthChecks = new Dictionary<string, object>();

            // 1. API Status
            healthChecks["api"] = new
            {
                status = "running",
                version = "1.0.0",  // Update with your version
                timestamp = DateTime.UtcNow
            };

            // 2. Database Status
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();
                healthChecks["database"] = new
                {
                    status = canConnect ? "healthy" : "unhealthy",
                    connected = canConnect
                };
            }
            catch (Exception ex)
            {
                healthChecks["database"] = new
                {
                    status = "error",
                    message = ex.Message
                };
            }

            // 3. Environment Info
            healthChecks["environment"] = new
            {
                aspnetcore_environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                machine_name = Environment.MachineName,
                os_version = Environment.OSVersion.ToString()
            };

            // Determine overall health
            bool isHealthy = healthChecks.Values
                .OfType<dynamic>()
                .All(check => check.status?.ToString() != "unhealthy" && 
                             check.status?.ToString() != "error");

            return isHealthy 
                ? Ok(new { status = "healthy", checks = healthChecks })
                : StatusCode(503, new { status = "unhealthy", checks = healthChecks });
        }
    }
}
```

---

## 🔧 Step 2: Fix HTTPS Redirect Issue

Your logs show: `Failed to determine the https port for redirect`

This happens because Elastic Beanstalk's load balancer uses HTTP to communicate with your app, but your app is trying to force HTTPS.

### Update `Program.cs`:

**Find this line (or similar):**
```csharp
app.UseHttpsRedirection();
```

**Replace with conditional HTTPS:**
```csharp
// Only use HTTPS redirection in Development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
```

**Or better yet, use X-Forwarded-Proto header:**
```csharp
// Configure for load balancer
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// HTTPS redirection will now work correctly behind load balancer
app.UseHttpsRedirection();
```

**Add this package if you use the second option:**
```bash
dotnet add package Microsoft.AspNetCore.HttpOverrides
```

**And add this using statement at the top of Program.cs:**
```csharp
using Microsoft.AspNetCore.HttpOverrides;
```

---

## 🎯 Step 3: Configure Elastic Beanstalk Health Check

### Option A: Use Default (Root Path)
Since you now have a health endpoint at `/`, the default health check will work.

### Option B: Customize Health Check Path

**In AWS Console:**
1. Go to Elastic Beanstalk → Your environment
2. Click **Configuration** in left sidebar
3. Under **Load balancer**, click **Edit**
4. Scroll to **Processes** → Default → **Edit**
5. Change **Health check path** from `/` to `/health`
6. Click **Apply**

---

## 📝 Step 4: Alternative - Use .NET Health Checks Middleware

.NET 8 has built-in health check support:

### In `Program.cs`, add health checks:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✨ Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<YourDbContext>("database");

var app = builder.Build();

// Configure middleware
app.UseRouting();
app.UseAuthorization();

// ✨ Map Health Check Endpoints
app.MapHealthChecks("/health");           // Simple health check
app.MapHealthChecks("/health/ready");     // Readiness check
app.MapHealthChecks("/health/live");      // Liveness check

app.MapControllers();
app.Run();
```

**Install the package:**
```bash
dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
```

---

## 🚀 Step 5: Test Your Health Endpoints

After redeploying, test these URLs in your browser:

```
http://fishhouse-api-prod.us-east-1.elasticbeanstalk.com/
http://fishhouse-api-prod.us-east-1.elasticbeanstalk.com/health
http://fishhouse-api-prod.us-east-1.elasticbeanstalk.com/health/database
http://fishhouse-api-prod.us-east-1.elasticbeanstalk.com/health/detailed
```

**Expected Responses:**

✅ **Healthy:**
```json
{
  "status": "healthy",
  "service": "Fish House API",
  "timestamp": "2025-10-30T14:30:00Z",
  "environment": "Production"
}
```

❌ **Database Issue:**
```json
{
  "status": "unhealthy",
  "database": "disconnected",
  "message": "Cannot connect to database"
}
```

---

## 🔍 Step 6: Verify Database Connection

If the `/health/database` endpoint fails, check:

### 1. Connection String Format
In `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=fish-house-app-database.c6vsqsge26t9.us-east-1.rds.amazonaws.com,1433;Database=FishHouseDB;User Id=sqladmin;Password=YourPassword;TrustServerCertificate=True;Encrypt=True;"
  }
}
```

### 2. Security Group Rules
Verify your API security group (`fish-house-app-api-sg`) can reach the database:

- API Security Group: `sg-0ae000b04b5a73bc9`
- Database Security Group: `sg-08ea120e1da2c2a93`
- Database SG should allow port 1433 from API SG ✅ (already configured)

### 3. Test from EC2 Instance (Advanced)
If database connection fails, SSH into the EC2 instance and test:

```powershell
# On Windows EC2 instance
Test-NetConnection -ComputerName fish-house-app-database.c6vsqsge26t9.us-east-1.rds.amazonaws.com -Port 1433
```

---

## 📦 Complete Deployment Checklist

1. **Add HealthController.cs** ✅
2. **Update Program.cs** to fix HTTPS redirect ✅
3. **Test locally** with `dotnet run` ✅
4. **Publish** your application:
   ```bash
   dotnet publish -c Release -o site
   cd site
   zip -r ../site.zip .
   cd ..
   zip fishhouse-api-v3.zip site.zip aws-windows-deployment-manifest.json
   ```
5. **Deploy** to Elastic Beanstalk
6. **Test health endpoints** in browser
7. **Verify load balancer** health in AWS Console

---

## 🎓 Interview Talking Point

**"How did you implement health checks?"**

"I implemented a multi-tiered health check system in my Fish House API. 

First, I created a dedicated HealthController with multiple endpoints:
- A root endpoint at `/` for the load balancer's basic health check
- `/health/database` to verify RDS connectivity
- `/health/detailed` for comprehensive system status

I also addressed an HTTPS redirect issue where the load balancer communicates over HTTP but the app was forcing HTTPS. I resolved this by implementing X-Forwarded headers middleware to properly detect the protocol behind the load balancer.

The health checks helped me quickly identify when the API was deployed successfully but couldn't connect to the RDS database, which led me to verify the security group rules and connection string configuration."

---

## 🐛 Common Issues & Solutions

### Issue 1: 502 Bad Gateway
**Cause:** App crashed or not starting  
**Solution:** Check application logs for exceptions

### Issue 2: Load Balancer Shows "OutOfService"
**Cause:** Health check failing  
**Solution:** Verify `/` or configured health path returns 200 OK

### Issue 3: Database Health Check Fails
**Cause:** Connection string or security group issue  
**Solution:** Verify RDS endpoint, credentials, and security group rules

### Issue 4: HTTPS Redirect Loop
**Cause:** Load balancer → HTTP → App → HTTPS redirect → Loop  
**Solution:** Use ForwardedHeaders middleware or disable HTTPS redirect

---

## 📚 Additional Resources

**Microsoft Docs:**
- [Health checks in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [Work with a reverse proxy](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer)

**AWS Docs:**
- [Elastic Load Balancing health checks](https://docs.aws.amazon.com/elasticloadbalancing/latest/application/target-group-health-checks.html)

---

**Created:** October 30, 2025  
**Purpose:** Health Endpoint Configuration  
**Status:** Ready to Implement 🏥
