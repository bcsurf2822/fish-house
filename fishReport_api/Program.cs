using System.Text;
using FishReportApi.Data;
using FishReportApi.Repositories;
using FishReportApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//DB CONTEXT
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FishDBContext>(options =>
    options.UseSqlServer(connectionString));

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
        policy.WithOrigins("fishnet-in-the-cloud.netlify.app")
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

//CORS
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowLocal5173");
}
else
{
    app.UseCors("AllowClient");
}


// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseCors("AllowAll"); // Always allow all during dev/debugging
// // app.UseCors("AllowLocal5173"); // Or use a more specific one if needed

// // Swagger always ON (for now)
// app.UseSwagger();
// app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();