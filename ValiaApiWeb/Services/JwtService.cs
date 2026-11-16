
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.Security;

namespace ValiaApi.Security
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GeneratedToken GenerateToken(User user)
        {
            string secret = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "A chave JWT não foi configurada.");

            byte[] keyBytes = Convert.FromBase64String(secret);

            if (keyBytes.Length < 32)
                throw new InvalidOperationException(
                    "A chave JWT deve possuir pelo menos 32 bytes.");

            var signingKey = new SymmetricSecurityKey(keyBytes);

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            int expirationMinutes = _configuration
                .GetValue<int>("Jwt:ExpirationMinutes");

            if (expirationMinutes <= 0)
                throw new InvalidOperationException(
                    "O tempo de expiração do JWT deve ser positivo.");

            DateTimeOffset expiresAt = DateTimeOffset.UtcNow
                .AddMinutes(expirationMinutes);

            var claims = new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.UserId.ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials);

            string tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return new GeneratedToken(tokenString, expiresAt);
        }
    }
}


