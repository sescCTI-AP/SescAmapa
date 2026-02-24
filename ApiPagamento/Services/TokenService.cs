using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PagamentoApi.Settings;
using PagamentoApi.Models;

namespace PagamentoApi.Services
{
    public class TokenService
    {
        public static string GenerateToken(Authentication authApp)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AuthSettings.Secret);
            Claim[] claims;
            if (authApp.App == "Usuario")
            {
                claims = new Claim[]{
                    new Claim(ClaimTypes.Name, authApp.Cpf),
                    new Claim(ClaimTypes.GivenName, authApp.Nome),
                    new Claim(ClaimTypes.Role, "usuario")
                };
            }
            else
            {
                claims = new Claim[]{
                    new Claim(ClaimTypes.Name, authApp.App),
                    new Claim(ClaimTypes.Role, "app")
                };
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                //Expires = DateTime.UtcNow.AddMinutes(2),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}