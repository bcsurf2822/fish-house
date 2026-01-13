using System.Text;
using FishReportApi.Data;
using FishReportApi.Repositories;
using FishReportApi.Repositories.Interfaces;
using FishReportApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//AURORA DSQL CONNECTION SERVICE
builder.Services.AddSingleton<AuroraDsqlConnectionService>();

//DB CONTEXT (Aurora DSQL with IAM Authentication)
// Note: Connection string is generated on first use with fresh IAM token
builder.Services.AddDbContext<FishDBContext>((serviceProvider, options) =>
{
    var connectionService = serviceProvider.GetRequiredService<AuroraDsqlConnectionService>();
    // GetConnectionStringAsync is called synchronously here - token is generated on-demand
    var connectionString = connectionService.GetConnectionStringAsync().GetAwaiter().GetResult();
    options.UseNpgsql(connectionString);
});

//JWT AUTHENTICATION
var authSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(authSettings.GetValue<string>("Key") ?? throw new InvalidOperationException("JWT Key not found."));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => // Fix is here
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = authSettings["Issuer"],
        ValidAudience = authSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Unauthorized: You must be logged in to perform that operation.\"}");
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Forbidden: You do not have permission to access this resource.\"}");
        }
    };
});

//REPOSITORIES
builder.Services.AddScoped(typeof(ICommonRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IFishRepository, FishRepository>();
builder.Services.AddScoped<IFishMarketRepository, FishMarketRepository>();


//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal5173", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
    });
    options.AddPolicy("AllowAll", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
});
    options.AddPolicy("AllowFisheriesService", policy =>
{
    policy.WithOrigins("https://", "https://")
          .AllowAnyMethod()
          .AllowAnyHeader();
});
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins(
                "https://fish-house-demo.netlify.app",
                "https://fishnet-in-the-cloud.netlify.app",
                "http://localhost:5173"
            )
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
    options.AddPolicy("AllowPublic", policy =>
{
    policy.WithOrigins("https://", "https://")
          .AllowAnyMethod()
          .AllowAnyHeader();
});
});

// Services
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(Program));


//BUILDS APP
var app = builder.Build();

//DATABASE MIGRATION DISABLED FOR AURORA DSQL
// Aurora DSQL doesn't support EF migrations (no auto-increment)
// Schema is managed manually via SQL scripts
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     try
//     {
//         var context = services.GetRequiredService<FishDBContext>();
//         var logger = services.GetRequiredService<ILogger<Program>>();
//         logger.LogInformation("[Program-Startup] Applying database migrations...");
//         context.Database.Migrate();
//         logger.LogInformation("[Program-Startup] Database migrations applied successfully");
//     }
//     catch (Exception ex)
//     {
//         var logger = services.GetRequiredService<ILogger<Program>>();
//         logger.LogError(ex, "[Program-Startup] An error occurred while migrating the database");
//         throw;
//     }
// }

//CORS - Use AllowClient for Netlify deployment
app.UseCors("AllowClient");


// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS Redirection - Only use in Development when we have HTTPS configured
// Elastic Beanstalk load balancer handles HTTPS termination
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();