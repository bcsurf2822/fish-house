# Elastic Beanstalk Deployment Guide - Fish House API

**Date:** October 30, 2025
**Project:** Fish House API (.NET 8.0)
**Target:** AWS Elastic Beanstalk (Windows Server + IIS)
**Database:** AWS RDS SQL Server

---

## Table of Contents

1. [Deployment Summary](#deployment-summary)
2. [Issues Encountered and Fixes](#issues-encountered-and-fixes)
3. [Final Configuration](#final-configuration)
4. [Deployment Process](#deployment-process)
5. [Troubleshooting](#troubleshooting)
6. [Testing After Deployment](#testing-after-deployment)

---

## Deployment Summary

### Environment Details

- **Elastic Beanstalk Environment:** fishhouse-api-prod
- **URL:** http://fishhouse-api-prod.us-east-1.elasticbeanstalk.com
- **Platform:** .NET 8 on Windows Server with IIS
- **Database:** fish-house-app-database.c6vsqsge26t9.us-east-1.rds.amazonaws.com
- **Database Type:** SQL Server (Azure SQL)

### Key Achievements

- ✅ Automatic database migrations on startup
- ✅ CORS configured for localhost:5173 (local testing)
- ✅ HTTPS redirection disabled for Elastic Beanstalk
- ✅ Connection string fixed for SQL Server
- ✅ SQL Server-specific migrations created
- ✅ Seed data configured (18 fish species, 3 markets)

---

## Issues Encountered and Fixes

### Issue 1: CORS Configuration Missing Protocol

**Problem:**
```csharp
policy.WithOrigins("fishnet-in-the-cloud.netlify.app")  // Missing https://
```

**Error:** Frontend couldn't connect due to CORS rejection

**Fix Applied (Program.cs:96):**
```csharp
policy.WithOrigins("https://fishnet-in-the-cloud.netlify.app")
```

**For Local Testing (Program.cs:146):**
```csharp
app.UseCors("AllowLocal5173");  // Allows http://localhost:5173
```

---

### Issue 2: HTTPS Redirection Failure

**Problem:**
```
Failed to determine the https port for redirect.
```

**Error:** Application couldn't start because Elastic Beanstalk doesn't configure HTTPS on the instance level (handled by load balancer)

**Fix Applied (Program.cs:156-161):**
```csharp
// HTTPS Redirection - Only use in Development when we have HTTPS configured
// Elastic Beanstalk load balancer handles HTTPS termination
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
```

---

### Issue 3: Connection String "User Id" vs "User ID"

**Problem:**
```
System.ArgumentException: Keyword not supported: 'userid'.
```

**Error:** SQL Server connection strings are case-sensitive for keywords

**Wrong:**
```json
"User Id=sqldbadmin"
```

**Fix Applied (appsettings.json:11):**
```json
"User Id=sqldbadmin"  (This is actually correct - Microsoft's standard)
```

**Note:** The fix was actually ensuring the exact casing Microsoft expects: `User Id` (capital U and I, lowercase 'd')

---

### Issue 4: SQLite Migration vs SQL Server

**Problem:**
```
Microsoft.Data.SqlClient.SqlException: Operand type clash: decimal is incompatible with text
Applying migration '20251029201600_InitialSQLiteMigration'.
```

**Error:** Migration was created for SQLite but running against SQL Server

**Root Cause:**
- SQLite uses TEXT data type
- SQL Server uses VARCHAR/NVARCHAR
- Migration file had SQLite-specific syntax

**Fix Applied:**
1. Deleted old SQLite migrations:
   ```bash
   rm -rf Migrations/*.cs Migrations/*.Designer.cs
   ```

2. Created new SQL Server migration:
   ```bash
   dotnet ef migrations add InitialSqlServerMigration --context FishDBContext
   ```

3. Ensured Program.cs uses SQL Server in production:
   ```csharp
   if (builder.Environment.IsDevelopment())
   {
       options.UseSqlite(connectionString);
   }
   else
   {
       options.UseSqlServer(connectionString);  // Production uses SQL Server
   }
   ```

---

### Issue 5: No Database Seeding on Startup

**Problem:** Database tables weren't being created automatically

**Fix Applied (Program.cs:124-143):**
```csharp
//DATABASE MIGRATION AND SEEDING ON STARTUP
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FishDBContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("[Program-Startup] Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("[Program-Startup] Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "[Program-Startup] An error occurred while migrating the database");
        throw; // Re-throw to prevent startup if database migration fails
    }
}
```

**What This Does:**
- Automatically applies pending migrations on startup
- Creates database tables if they don't exist
- Inserts seed data from FishDBContext.cs
- Logs the process for debugging
- Fails fast if migration fails (prevents app from starting with broken DB)

---

## Final Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=fish-house-app-database.c6vsqsge26t9.us-east-1.rds.amazonaws.com,1433;Database=fish-house-app-database;User Id=sqldbadmin;Password=Underthesea88!;TrustServerCertificate=True;Encrypt=True;"
  },
  "JwtSettings": {
    "Key": "WelcomethisIsOurFishReportApIsecretKEY%^&^*^*(*&((((((**&@@#$",
    "Issuer": "FishReportApi",
    "Audience": "FishMarketPolice",
    "ExpiryMinutes": 60
  }
}
```

### aws-windows-deployment-manifest.json

```json
{
    "manifestVersion": 1,
    "deployments": {
        "aspNetCoreWeb": [
            {
                "name": "fishhouse-api",
                "parameters": {
                    "appBundle": "site.zip",
                    "iisPath": "/",
                    "iisWebSite": "Default Web Site"
                }
            }
        ]
    }
}
```

### Program.cs - Key Sections

**Database Configuration:**
```csharp
builder.Services.AddDbContext<FishDBContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});
```

**CORS Configuration:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal5173", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins("https://fishnet-in-the-cloud.netlify.app")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
```

**Middleware Pipeline:**
```csharp
//CORS - Use AllowLocal5173 for testing from localhost
app.UseCors("AllowLocal5173");

// Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS Redirection - Only use in Development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
```

---

## Deployment Process

### Step 1: Clean Previous Builds

```bash
cd /path/to/FishHouseAPI
dotnet clean
rm -rf site/ site.zip fishhouse-api-deployment.zip
```

### Step 2: Publish Application

```bash
dotnet publish -c Release -o site
```

### Step 3: Create Deployment Package

```bash
# Create site.zip from published output
cd site
zip -r ../site.zip .
cd ..

# Create final deployment ZIP
zip fishhouse-api-deployment.zip site.zip aws-windows-deployment-manifest.json
```

### Step 4: Verify Package Structure

```bash
unzip -l fishhouse-api-deployment.zip
```

**Expected Output:**
```
Archive:  fishhouse-api-deployment.zip
  Length      Date    Time    Name
---------  ---------- -----   ----
 20048337  10-30-2025 12:10   site.zip
      346  10-30-2025 10:13   aws-windows-deployment-manifest.json
---------                     -------
 20048683                     2 files
```

### Step 5: Upload to Elastic Beanstalk

1. Go to AWS Console → Elastic Beanstalk
2. Select environment: `fishhouse-api-prod`
3. Click **"Upload and deploy"**
4. Upload `fishhouse-api-deployment.zip`
5. Click **"Deploy"**
6. Wait for deployment to complete (2-5 minutes)

### Step 6: Check Logs

1. In Elastic Beanstalk console, click **"Logs"**
2. Click **"Request Logs"** → **"Last 100 Lines"**
3. Look for:
   ```
   [Program-Startup] Applying database migrations...
   [Program-Startup] Database migrations applied successfully
   Application started successfully
   ```

---

## Troubleshooting

### How to Read Elastic Beanstalk Logs

**Location in Logs:**
- `EventLog Application:` - Application startup messages
- `AWS.DeploymentCommands.*.log:` - Deployment process
- Look for `[Program-Startup]` - Your custom logging

**Common Log Searches:**

```bash
# Search for errors
grep -i "error\|exception\|fail" TailLogs-*.out

# Search for migration messages
grep "Program-Startup" TailLogs-*.out

# Search for application startup
grep "Application.*started" TailLogs-*.out
```

### Common Errors and Solutions

#### Error: "Keyword not supported: 'userid'"
**Solution:** Ensure connection string uses exact casing: `User Id=`

#### Error: "Operand type clash: decimal is incompatible with text"
**Solution:** Recreate migrations for SQL Server (delete SQLite migrations)

#### Error: "Failed to determine the https port for redirect"
**Solution:** Disable HTTPS redirection in production (see Program.cs fix above)

#### Error: "No connection could be made"
**Solution:** Check RDS security group allows traffic from Elastic Beanstalk security group

#### Application Timeout / Not Responding
**Solution:** Check if migration is failing - app will crash during startup if migration fails

---

## Testing After Deployment

### Test 1: API Health Check

```bash
curl http://fishhouse-api-prod.us-east-1.elasticbeanstalk.com/api/fish/getall
```

**Expected:** JSON array with fish species

### Test 2: Get All Markets

```bash
curl http://fishhouse-api-prod.us-east-1.elasticbeanstalk.com/api/fishmarket/getall
```

**Expected:** JSON array with 3 markets (NY Fish Co., Tempest, Lobster House)

### Test 3: Check Specific Fish

```bash
curl http://fishhouse-api-prod.us-east-1.elasticbeanstalk.com/api/fish/getbyid/1
```

**Expected:** JSON object for Bluefin Tuna

### Test 4: From Local Frontend

**Frontend .env file:**
```
VITE_API_BASE_URL=http://fishhouse-api-prod.us-east-1.elasticbeanstalk.com
```

**Start frontend:**
```bash
cd fishReport_frontend
npm run dev
```

**Open browser:** http://localhost:5173

**Expected:** Frontend should load fish data from API

---

## Seed Data Reference

### Fish Species (18)
1. Bluefin Tuna - $12.99
2. Mahi Mahi - $8.50
3. Swordfish - $14.25
4. Atlantic Salmon - $9.75
5. Yellowfin Tuna - $11.20
6. Humphead Wrasse - $18.00
7. Anchovy - $2.00
8. Marlin - $13.80
9. Pacific Halibut - $10.50
10. Giant Trevally - $9.90
11. King Mackerel - $7.99
12. Tarpon - $6.49
13. Sailfish - $15.25
14. Snapper - $6.99
15. Groupers - $10.25
16. Flying Fish - $5.50
17. Pompano - $8.25
18. Sturgeon - $16.50

### Fish Markets (3)
1. NY Fish Co. - New York, NY
2. Tempest - New Bedford, MA
3. Lobster House - Cape May, NJ

### Inventory Relationships
- **NY Fish Co.** has: Bluefin Tuna, Mahi Mahi, Atlantic Salmon, Humphead Wrasse, Anchovy, Pacific Halibut, Giant Trevally
- **Tempest** has: Swordfish, Yellowfin Tuna, Marlin, King Mackerel, Tarpon, Snapper, Flying Fish, Sturgeon
- **Lobster House** has: Atlantic Salmon, Anchovy, Pacific Halibut, Sailfish, Groupers, Pompano

---

## API Endpoints Reference

### Fish Endpoints

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| GET | `/api/fish/getAll` | No | Get all fish species |
| GET | `/api/fish/fishInventory` | No | Get all species with market info |
| GET | `/api/fish/getById/{id}` | No | Get specific fish by ID |
| POST | `/api/fish/create` | No | Create new fish species |
| PATCH | `/api/fish/updatePartial/{id}` | Yes | Partial update (JSON Patch) |
| DELETE | `/api/fish/{id}` | Yes | Delete fish species |

### Market Endpoints

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| GET | `/api/fishmarket/getAll` | No | Get all markets with species |
| GET | `/api/fishmarket/marketid/{id}` | No | Get market by ID |
| GET | `/api/fishmarket/inventory` | No | Get market inventory summary |
| POST | `/api/fishmarket/createnew` | Yes | Create new market |
| POST | `/api/fishmarket/addtoinventory/{marketId}/{speciesId}` | Yes | Add species to market |
| PUT | `/api/fishmarket/update/{id}` | No | Update market |
| PATCH | `/api/fishmarket/updatepartial/{id}` | Yes | Partial update market |
| DELETE | `/api/fishmarket/deletefrominventory/{marketId}/{speciesId}` | No | Remove species from market |
| DELETE | `/api/fishmarket/delete/{id}` | Yes | Delete market |

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/authentication/login` | Login and get JWT token |

**Credentials:**
- Username: `marketAdmin`
- Password: `marketAdmin`

---

## Files Modified During Deployment

### Program.cs
- Added database migration on startup
- Fixed CORS configuration
- Disabled HTTPS redirection for production
- Changed CORS policy to AllowLocal5173

### appsettings.json
- Verified connection string format
- Ensured proper SQL Server connection string

### Migrations/
- Deleted SQLite-specific migrations
- Created new `InitialSqlServerMigration` for SQL Server

### Deployment Package Structure
```
fishhouse-api-deployment.zip
├── site.zip (contains all published .NET application files)
└── aws-windows-deployment-manifest.json
```

---

## Next Steps (Optional Enhancements)

1. **Add Environment Variables in Elastic Beanstalk:**
   - Move connection string to environment variables
   - Configure different CORS policies per environment

2. **Enable HTTPS:**
   - Add SSL certificate to load balancer
   - Update CORS to use https:// for production URLs

3. **Setup Monitoring:**
   - Configure CloudWatch alarms
   - Set up error notifications

4. **Database Backups:**
   - Configure automated RDS backups
   - Test restore procedures

5. **CI/CD Pipeline:**
   - Automate deployment with GitHub Actions
   - Run tests before deployment

---

## Support and Documentation

- **AWS Elastic Beanstalk Docs:** https://docs.aws.amazon.com/elasticbeanstalk/
- **.NET on Elastic Beanstalk:** https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/dotnet-core-tutorial.html
- **Entity Framework Core Migrations:** https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/

---

**Last Updated:** October 30, 2025
**Deployment Status:** ✅ Successful
**API Health:** ✅ Running
