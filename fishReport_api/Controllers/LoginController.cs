using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FishReportApi.DTOs;
using FishReportApi.Models;

namespace FishReportApi.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            // Dummy user validation (since you have a hardcoded user)
            if (loginDto.Username == "marketAdmin" && loginDto.Password == "marketAdmin")
            {
                var token = GenerateToken(loginDto.Username);
                return Ok(new { Token = token, loginDto.Username });
            }
            return Unauthorized("Please provide valid Market Police Credentials");
        }

        private string GenerateToken(string username)
        {
            var authSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: authSettings["Issuer"],
                audience: authSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}