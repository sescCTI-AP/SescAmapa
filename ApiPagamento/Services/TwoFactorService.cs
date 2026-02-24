using Microsoft.AspNetCore.Identity;
using PagamentoApi.Models;
using System.Threading.Tasks;
using System;

namespace PagamentoApi.Services
{
    public class TwoFactorService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TwoFactorService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Verify2FARequest> GenerateTwoFactorCodeAsync(string email)
        {
            // Cria uma instância temporária de ApplicationUser
            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                UserName = email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // Gera o token de 2FA usando o EmailProvider
            var token = await _userManager.GenerateTwoFactorTokenAsync(applicationUser, TokenOptions.DefaultEmailProvider);

            // Retorna o token gerado
            var verify2FARequest = new Verify2FARequest
            {
                Email = email,
                Token = token,
                SecurityStamp = applicationUser.SecurityStamp
            };

            return verify2FARequest;
        }


        public async Task<bool> ValidateTwoFactorCodeAsync(string email, string token, string securityStamp)
        {
            // Cria uma instância temporária de ApplicationUser para validar o token
            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                UserName = email,
                SecurityStamp = securityStamp

            };

            // Valida o token 2FA
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(applicationUser, TokenOptions.DefaultEmailProvider, token);
            return isValid;
        }
    }

}
