using System;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MiniGame.Models;
using MiniGame.Services;

namespace MiniGame.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public JwtHelper(IConfiguration configuration, UserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        // Генерація JWT токену
        public string GenerateJwtToken(string username, string email, string password, RoleEnum role, int id)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim(ClaimTypes.Hash, password),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Витягування інформації з токену
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
                Console.WriteLine("Token is valid");
                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null; // Токен недійсний або не може бути валідуваний
            }
        }

        // Витягування користувача (наприклад, імені) з токену
        public JWTPayloadUserData GetUserInfoFromToken(string token)
        {
            var principal = GetPrincipalFromToken(token);
            if (principal == null) return null;

            var username = principal.FindFirst(ClaimTypes.Name)?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var password = principal.FindFirst(ClaimTypes.Hash)?.Value;
            var id = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // User user = await _userService.GetUserByIdAsync(id);
            // return { username: username, role: role, email: email, password: password, id: id}

            return new JWTPayloadUserData
            {
                username = username,
                role = role,
                email = email,
                passwordHash = password,
                id = id
            };
            // return user;
        }

    }
    public class JWTPayloadUserData
    {
        public string username;
        public string role;
        public string email;
        public int id;
        public string passwordHash;
    }
}
