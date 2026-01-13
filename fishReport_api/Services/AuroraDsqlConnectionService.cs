using Amazon;
using Amazon.DSQL.Util;
using Amazon.Runtime;
using System.Text;

namespace FishReportApi.Services
{
    /// <summary>
    /// Service for managing Aurora DSQL connections with IAM authentication
    /// </summary>
    public class AuroraDsqlConnectionService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuroraDsqlConnectionService> _logger;
        private string? _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;
        private readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1, 1);

        public AuroraDsqlConnectionService(
            IConfiguration configuration,
            ILogger<AuroraDsqlConnectionService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Gets a connection string with a fresh IAM authentication token
        /// </summary>
        public async Task<string> GetConnectionStringAsync()
        {
            var endpoint = _configuration["AuroraDSQL:ClusterEndpoint"]
                ?? throw new InvalidOperationException("Aurora DSQL cluster endpoint not configured");
            var region = _configuration["AuroraDSQL:Region"] ?? "us-east-1";
            var database = _configuration["AuroraDSQL:Database"] ?? "postgres";
            var username = _configuration["AuroraDSQL:Username"] ?? "admin";

            var token = await GetOrRefreshTokenAsync(endpoint, region);

            var connectionString = new StringBuilder()
                .Append($"Host={endpoint};")
                .Append("Port=5432;")
                .Append($"Database={database};")
                .Append($"Username={username};")
                .Append($"Password={token};")
                .Append("SSL Mode=Require;")
                .Append("Trust Server Certificate=true;")
                .Append("Timeout=30;")
                .Append("Command Timeout=30;")
                .ToString();

            _logger.LogInformation("[AuroraDsqlConnectionService-GetConnectionStringAsync] Generated connection string for {Endpoint}", endpoint);

            return connectionString;
        }

        /// <summary>
        /// Gets a cached token or generates a new one if expired
        /// </summary>
        private async Task<string> GetOrRefreshTokenAsync(string endpoint, string region)
        {
            // Check if we have a valid cached token (refresh 5 minutes before expiry)
            if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
            {
                _logger.LogDebug("[AuroraDsqlConnectionService-GetOrRefreshTokenAsync] Using cached token");
                return _cachedToken;
            }

            await _tokenLock.WaitAsync();
            try
            {
                // Double-check after acquiring lock
                if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
                {
                    return _cachedToken;
                }

                _logger.LogInformation("[AuroraDsqlConnectionService-GetOrRefreshTokenAsync] Generating new IAM authentication token");

                var token = await GenerateAuthTokenAsync(endpoint, region);
                _cachedToken = token;
                _tokenExpiry = DateTime.UtcNow.AddHours(1); // Tokens expire after 1 hour

                _logger.LogInformation("[AuroraDsqlConnectionService-GetOrRefreshTokenAsync] Token generated successfully, expires at {Expiry}", _tokenExpiry);

                return token;
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        /// <summary>
        /// Generates an IAM authentication token for Aurora DSQL
        /// </summary>
        private Task<string> GenerateAuthTokenAsync(string endpoint, string region)
        {
            try
            {
                var regionEndpoint = RegionEndpoint.GetBySystemName(region);
                var credentials = FallbackCredentialsFactory.GetCredentials();

                // Generate authentication token using DSQLAuthTokenGenerator utility
                var token = DSQLAuthTokenGenerator.GenerateDbConnectAdminAuthToken(
                    credentials,
                    regionEndpoint,
                    endpoint);

                _logger.LogDebug("[AuroraDsqlConnectionService-GenerateAuthTokenAsync] Successfully generated auth token");

                return Task.FromResult(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuroraDsqlConnectionService-GenerateAuthTokenAsync] Failed to generate auth token");
                throw new InvalidOperationException("Failed to generate Aurora DSQL authentication token", ex);
            }
        }
    }
}
