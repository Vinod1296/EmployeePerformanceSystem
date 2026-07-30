using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeePerformance.Application.Configuration;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmployeePerformance.Infrastructure.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
            ValidateSettings();
        }

        public string GenerateToken(User user)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes);

            var claims = new List<Claim>
            {
                new("UserId", user.UserId.ToString()),
                new("EmployeeId", user.EmployeeId.ToString()),
                new("Username", user.Username),
                new(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(_jwtSettings.Key))
            {
                throw new InvalidOperationException("JWT key is missing.");
            }

            if (string.IsNullOrWhiteSpace(_jwtSettings.Issuer))
            {
                throw new InvalidOperationException("JWT issuer is missing.");
            }

            if (string.IsNullOrWhiteSpace(_jwtSettings.Audience))
            {
                throw new InvalidOperationException("JWT audience is missing.");
            }
        }
    }
}
