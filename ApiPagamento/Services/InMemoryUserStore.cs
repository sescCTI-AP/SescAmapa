using Microsoft.AspNetCore.Identity;
using PagamentoApi.Models;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

namespace PagamentoApi.Services
{

    public class InMemoryUserStore : IUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser>, IUserSecurityStampStore<ApplicationUser>
    {
        private Dictionary<string, ApplicationUser> _users = new Dictionary<string, ApplicationUser>();

        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (!_users.ContainsKey(user.Id))
            {
                // Gera o SecurityStamp se ele estiver nulo ou vazio
                if (string.IsNullOrEmpty(user.SecurityStamp))
                {
                    user.SecurityStamp = Guid.NewGuid().ToString();
                }

                _users.Add(user.Id, user);
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            // Remove o usuário do dicionário
            _users.Remove(user.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            // Busca o usuário pelo ID
            _users.TryGetValue(userId, out var user);
            return Task.FromResult(user);
        }

        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            // Busca o usuário pelo nome de usuário normalizado
            foreach (var user in _users.Values)
            {
                if (user.UserName.ToUpper() == normalizedUserName)
                {
                    return Task.FromResult(user);
                }
            }
            return Task.FromResult<ApplicationUser>(null);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            // Atualiza o usuário no dicionário
            _users[user.Id] = user;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName?.ToUpper());
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true); // Para este exemplo, sempre retorna true
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            // Busca o usuário pelo email normalizado
            foreach (var user in _users.Values)
            {
                if (user.Email.ToUpper() == normalizedEmail)
                {
                    return Task.FromResult(user);
                }
            }
            return Task.FromResult<ApplicationUser>(null);
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email?.ToUpper());
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true); // Para este exemplo, o 2FA está sempre habilitado
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            // Retorna o Security Stamp do usuário
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
        {
            // Define o Security Stamp do usuário
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Nada a liberar, já que estamos usando um armazenamento in-memory
        }
    }
}


