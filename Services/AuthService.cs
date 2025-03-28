using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MiniGame.Models;

namespace MiniGame.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        // public string GenerateToken(string username, RoleEnum role, string email, string password)
        // {
        //     var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        //     var claims = new[] { new Claim(ClaimTypes.Name, username) };
        //     var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        //     var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: credentials);
        //     return new JwtSecurityTokenHandler().WriteToken(token);
        // }
    }
}
