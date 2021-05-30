using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Services
{
    public class JWTService
    {
        private readonly IConfiguration _configuration;

        private readonly SigningCredentials _signingCredentials;

        private readonly JwtSecurityTokenHandler _jwtTokenHandler;
        
        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTPrivateKey"]));
            _signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            _jwtTokenHandler = new JwtSecurityTokenHandler();
        }

        public string GenerateToken(int userId, string userName)
        {
            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), new Claim(ClaimTypes.Name, userName)
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.Now.AddMinutes(5),
                Subject = claims,
                SigningCredentials = _signingCredentials
            };

            var securityToken = _jwtTokenHandler.CreateToken(tokenDescriptor);

            return _jwtTokenHandler.WriteToken(securityToken);
        }
    }
}