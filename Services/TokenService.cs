using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Dapper.SqlMapper;

namespace SiteSesc.Services
{
    public class TokenService
    {
        private const string SecretKey = "5VxG2Rt2R6dbfC7Q)1QdGsMm0pRA+xCA"; // Deve ser a mesma em ambos os sistemas
        private const int TokenExpirationInMinutes = 30; // Tempo de expiração do token

        public string GenerateToken(string nome, string userId, string email, string credencial)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, nome),
                new Claim("cpf", userId),
                new Claim(ClaimTypes.Email, email),
                new Claim("credencial", credencial)
            }),
                Expires = DateTime.UtcNow.AddMinutes(TokenExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
