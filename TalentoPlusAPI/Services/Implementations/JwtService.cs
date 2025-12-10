using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TalentoPlus.Models;
using TalentoPlus.Services.Interfaces;

namespace TalentoPlus.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationHours;

        public JwtService()
        {
            _secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "DefaultSecretKey12345678901234567890";
            _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "TalentoPlusAPI";
            _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "TalentoPlusClients";
            _expirationHours = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS") ?? "24");
        }

        public string GenerateToken(Employee employee)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, employee.Email),
                new Claim("documentNumber", employee.DocumentNumber),
                new Claim("fullName", $"{employee.FirstName} {employee.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_expirationHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}