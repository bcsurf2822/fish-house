# Fish House API

A RESTful API built with ASP.NET Core 9.0 for managing fish species and fish market inventories. This API provides endpoints for tracking fish species information, fish markets, and the many-to-many relationships between them through a market inventory system.

## Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Database](#database)
- [Authentication](#authentication)
- [Configuration](#configuration)
- [Deployment](#deployment)
- [Development Notes](#development-notes)

## Features

- CRUD operations for fish species and fish markets
- Many-to-many relationship management (market inventory)
- JWT authentication for protected endpoints
- AutoMapper for DTO transformations
- Swagger/OpenAPI documentation (development mode)
- CORS support for frontend integration
- Repository pattern with generic base repository
- Entity Framework Core with SQL Server (production) and SQLite (development)

## Technology Stack

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 9.0.4** - ORM for database operations
- **SQL Server** - Production database (Azure SQL)
- **SQLite** - Development database
- **AutoMapper 12.0.1** - Object-to-object mapping
- **JWT Bearer Authentication** - Secure API endpoints
- **Swagger/Swashbuckle** - API documentation
- **Newtonsoft.Json** - JSON serialization and PATCH support

## Project Structure

```
fishReport_api/
├── Controllers/              # API endpoint controllers
│   ├── FishController.cs           # Fish species endpoints
│   ├── FishMarketController.cs     # Fish market endpoints
│   └── LoginController.cs          # Authentication endpoint
├── Models/                   # Entity/Domain models
│   ├── Species.cs                  # Fish species entity
│   ├── FishMarket.cs               # Fish market entity
│   ├── FishMarketInventory.cs      # Junction table entity
│   └── Login.cs                    # User model
├── DTOs/                     # Data Transfer Objects
│   ├── GeneralDTO.cs               # SpeciesDTO, FishMarketDTO
│   ├── InventoryDTO.cs             # Inventory-specific DTOs
│   ├── LoginDTO.cs                 # UserDTO, LoginDTO
│   └── MarketControlDTO.cs         # Market operation DTOs
├── Data/                     # Database context
│   └── FishDBContext.cs            # EF Core DbContext
├── Repositories/             # Data access layer
│   ├── Interfaces/
│   │   ├── ICommonRepository.cs    # Generic repository interface
│   │   ├── IFishRepository.cs      # Fish-specific operations
│   │   └── IFishMarketRepository.cs # Market-specific operations
│   ├── CommonRepository.cs         # Generic repository implementation
│   ├── FishRepository.cs           # Fish repository
│   └── FishMarketRepository.cs     # Market repository
├── Mappings/                 # AutoMapper profiles
│   └── AutoMapperProfile.cs        # DTO mappings
├── Migrations/               # EF Core database migrations
├── Properties/               # Launch settings
│   └── launchSettings.json
├── Program.cs                # Application entry point
├── appsettings.json          # Production configuration
├── appsettings.Development.json # Development configuration
└── FishHouseApi.csproj       # Project file
```

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- [Azure SQL Database](https://azure.microsoft.com/services/sql-database/) (for production) OR SQLite (for development)
- Code editor (Visual Studio, VS Code, Rider, etc.)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd fishReport_api
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update connection strings**

   Edit `appsettings.Development.json` to use SQLite for local development:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=FishMarket.db"
     }
   }
   ```

4. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Build the project**
   ```bash
   dotnet build
   ```

## Running the Application

### Development Mode

```bash
# Standard way (may require macOS security approval)
dotnet run

# Alternative way (direct DLL execution, bypasses macOS Gatekeeper)
dotnet bin/Debug/net9.0/FishHouseApi.dll
```

The API will start on:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001 (if configured)
- **Swagger UI:** http://localhost:5000/swagger (development only)

### Production Mode

```bash
# Set environment variable
export ASPNETCORE_ENVIRONMENT=Production

# Run the application
dotnet run
```

### macOS Security Note

If you encounter a macOS security warning about the app not being verified:
1. Go to **System Settings** > **Privacy & Security**
2. Scroll to the **Security** section
3. Click **"Allow Anyway"** for `FishHouseApi`
4. Run the app again

**Alternative:** Run the DLL directly:
```bash
dotnet bin/Debug/net9.0/FishHouseApi.dll
```

## API Endpoints

### Fish Species Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/fish/getall` | No | Get all fish species |
| GET | `/api/fish/fishinventory` | No | Get all species with market info |
| GET | `/api/fish/getbyid/{id}` | No | Get specific fish by ID |
| POST | `/api/fish/create` | No | Create new fish species |
| PATCH | `/api/fish/updatepartial/{id}` | Yes | Partial update (JSON Patch) |
| DELETE | `/api/fish/{id}` | Yes | Delete fish species |

### Fish Market Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/fishmarket/getall` | No | Get all markets with species |
| GET | `/api/fishmarket/marketid/{id}` | No | Get market by ID |
| GET | `/api/fishmarket/inventory` | No | Get market inventory summary |
| POST | `/api/fishmarket/createnew` | Yes | Create new market |
| POST | `/api/fishmarket/addtoinventory/{marketId}/{speciesId}` | Yes | Add species to market |
| PUT | `/api/fishmarket/update/{id}` | No | Update market |
| PATCH | `/api/fishmarket/updatepartial/{id}` | Yes | Partial update market |
| DELETE | `/api/fishmarket/deletefrominventory/{marketId}/{speciesId}` | No | Remove species from market |
| DELETE | `/api/fishmarket/delete/{id}` | Yes | Delete market |

### Authentication Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/authentication/login` | No | Login and receive JWT token |

**Login Credentials:**
```json
{
  "username": "marketAdmin",
  "password": "marketAdmin"
}
```

## Database

### Entity Models

**Species**
- Id (int) - Primary key
- Name (string, required, max 100)
- Habitat (string, required, max 100)
- Length (int, 0-500 cm)
- Population (int?, nullable)
- Lifespan (int)
- Price (decimal, $0.01-$200.00)

**FishMarket**
- Id (int) - Primary key
- MarketName (string, required, max 100)
- Location (string?, nullable, max 200)

**FishMarketInventory** (Junction Table)
- FishMarketId (int) - Composite key
- SpeciesId (int) - Composite key

### Database Configuration

The application uses different databases based on the environment:

- **Development:** SQLite (local file database)
- **Production:** Azure SQL Server

This is configured in `Program.cs`:
```csharp
if (builder.Environment.IsDevelopment())
{
    options.UseSqlite(connectionString);
}
else
{
    options.UseSqlServer(connectionString);
}
```

### Migrations

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Update database to latest migration
dotnet ef database update

# Rollback to a previous migration
dotnet ef database update PreviousMigrationName

# Remove last migration (if not applied)
dotnet ef migrations remove
```

## Authentication

The API uses JWT (JSON Web Token) Bearer authentication for protected endpoints.

### How to Authenticate

1. **Login to get token:**
   ```bash
   curl -X POST http://localhost:5000/api/authentication/login \
     -H "Content-Type: application/json" \
     -d '{"username":"marketAdmin","password":"marketAdmin"}'
   ```

2. **Use token in requests:**
   ```bash
   curl -X DELETE http://localhost:5000/api/fish/1 \
     -H "Authorization: Bearer YOUR_TOKEN_HERE"
   ```

### Token Details

- **Algorithm:** HMAC-SHA256
- **Expiry:** 60 minutes
- **Claims:** Subject (username), JTI (unique ID)
- **Issuer:** FishReportApi
- **Audience:** FishMarketPolice

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_SQL_Server_Connection_String"
  },
  "JwtSettings": {
    "Key": "Your_Secret_Key_Here",
    "Issuer": "FishReportApi",
    "Audience": "FishMarketPolice",
    "ExpiryMinutes": 60
  }
}
```

### CORS Configuration

The application supports multiple CORS policies:

- **AllowLocal5173:** Development frontend (http://localhost:5173)
- **AllowClient:** Production frontend (fishnet-in-the-cloud.netlify.app)
- **AllowFisheriesService:** External service integration
- **AllowPublic:** Public endpoints

## Deployment

### Azure App Service Deployment

1. **Prerequisites:**
   - Azure subscription
   - Azure SQL Database set up

2. **Create App Service:**
   - Runtime: .NET 9
   - Operating System: Linux (recommended) or Windows
   - Pricing Tier: Free F1 or higher

3. **Configure Application Settings in Azure:**
   - Connection string: `DefaultConnection`
   - JWT settings: `JwtSettings__Key`, `JwtSettings__Issuer`, `JwtSettings__Audience`

4. **Deploy using Azure CLI:**
   ```bash
   # Login to Azure
   az login

   # Publish the app
   dotnet publish -c Release -o ./publish

   # Deploy to Azure
   az webapp up --name your-app-name --resource-group your-resource-group
   ```

5. **Deploy using Visual Studio:**
   - Right-click project > Publish
   - Select Azure > Azure App Service (Linux)
   - Follow the wizard

### Environment Variables

For production, set these environment variables in Azure App Service:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection` - Your Azure SQL connection string
- `JwtSettings__Key` - Your JWT secret key
- `JwtSettings__Issuer` - Your issuer name
- `JwtSettings__Audience` - Your audience name

## Development Notes

### Logging Convention

Use this logging format throughout the codebase:
```csharp
Console.WriteLine("[FILENAME-FUNCTION] description and log of what is happening");
```

Example:
```csharp
Console.WriteLine("[FishRepository-GetAllAsync] Fetching all fish species from database");
```

### Repository Pattern

The project uses the Repository pattern with a generic base:

- `ICommonRepository<T>` - Generic CRUD interface
- `GenericRepository<T>` - Base implementation
- `IFishRepository` / `FishRepository` - Fish-specific operations
- `IFishMarketRepository` / `FishMarketRepository` - Market-specific operations

### AutoMapper

DTOs are automatically mapped using AutoMapper profiles in `Mappings/AutoMapperProfile.cs`. The mapping handles:

- Entity to DTO transformations
- Flattening of navigation properties
- Collection mappings for inventory relationships

### Testing the API

Use Swagger UI in development:
- Navigate to http://localhost:5000/swagger
- Test endpoints interactively
- View request/response schemas

Or use the included `FishHouseApi.http` file with REST Client extensions.

### Common Commands

```bash
# Clean build artifacts
dotnet clean

# Build the project
dotnet build

# Run tests (when added)
dotnet test

# Publish for deployment
dotnet publish -c Release -o ./publish

# Watch for file changes and auto-rebuild
dotnet watch run
```

### Known Limitations

- Authentication uses hardcoded credentials (not production-ready)
- No user registration system
- No role-based authorization beyond authentication
- No pagination on list endpoints
- No query filtering/sorting capabilities
- Database credentials in appsettings.json (should use Azure Key Vault in production)

### Future Enhancements

- User registration and management system
- Role-based authorization (Admin, User roles)
- Password hashing (bcrypt, PBKDF2)
- Pagination, filtering, and sorting
- Caching layer (Redis)
- Structured logging (Serilog)
- Health checks and monitoring
- API versioning
- Request validation middleware
- Global exception handling
- Comprehensive unit and integration tests

## Dependencies

### NuGet Packages

- **Microsoft.EntityFrameworkCore.SqlServer** (9.0.4) - SQL Server provider
- **Microsoft.EntityFrameworkCore.Sqlite** (9.0.3) - SQLite provider
- **Microsoft.EntityFrameworkCore.Design** (9.0.3) - CLI tools
- **Microsoft.EntityFrameworkCore.Tools** (9.0.3) - Package Manager Console
- **Microsoft.AspNetCore.Authentication.JwtBearer** (9.0.3) - JWT auth
- **Microsoft.AspNetCore.OpenApi** (9.0.3) - OpenAPI support
- **Microsoft.AspNetCore.Mvc.NewtonsoftJson** (9.0.3) - JSON Patch
- **AutoMapper.Extensions.Microsoft.DependencyInjection** (12.0.1) - DI integration
- **Swashbuckle.AspNetCore** (8.0.0) - Swagger UI
- **Microsoft.IdentityModel.JsonWebTokens** (8.7.0) - JWT tokens

## Troubleshooting

### Database Connection Issues

If you can't connect to the database:
1. Verify connection string in appsettings.json
2. Check if Azure SQL firewall allows your IP
3. Test connection using SQL Server Management Studio or Azure Data Studio
4. For development, use SQLite by setting `ASPNETCORE_ENVIRONMENT=Development`

### CORS Errors

If frontend can't access the API:
1. Check CORS policy in Program.cs matches your frontend URL
2. Verify the correct policy is being used for your environment
3. Add your Azure App Service URL to CORS policies before deployment

### JWT Authentication Errors

If you get 401 Unauthorized:
1. Ensure you've logged in and received a token
2. Include token in Authorization header: `Bearer <token>`
3. Check if token has expired (60 minutes)
4. Verify JWT settings match between appsettings and token generation

### Migration Errors

If migrations fail:
1. Ensure connection string is correct
2. Check database permissions
3. Try: `dotnet ef database drop` then `dotnet ef database update`
4. Delete Migrations folder and recreate: `dotnet ef migrations add Initial`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is part of the Fish House project collection.

---

**Last Updated:** October 2025
**API Version:** 1.0
**.NET Version:** 9.0

For more detailed documentation, refer to `CLAUDE.md` in the project root.
