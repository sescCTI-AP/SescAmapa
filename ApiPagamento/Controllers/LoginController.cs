using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PagamentoApi.DTOs;
using PagamentoApi.Models;
using PagamentoApi.Models.BB;
using PagamentoApi.Models.Site;
using PagamentoApi.Repositories;
using PagamentoApi.Services;
using PagamentoApi.Settings;

namespace PagamentoApi.Controllers
{
    
    [Route("v1/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IWebHostEnvironment _server;
        public LoginController(IWebHostEnvironment server)
        {
            _server = server;
        }
        [HttpPost]
        public async Task<ActionResult<dynamic>> Authenticate([FromServices] LoginRepository loginRepository, [FromBody] Authentication login)
        {
            //Como diferenciar login de APP de login de usuarios?
            //Talvez usar roles pra cada tipo de login?
            //Permissoes diferentes para cada tipo de login
            //Se usar mesmo login como diferenciar pra um usuario nao acessar info de outro?
            // talvez passar o usuario(login.app) e senha dos apps para o Banco de Dados, assim cada app tem sua senha

            //select senha do banco
            var appBanco = loginRepository.getApp(login.App);

            if (appBanco == null)
            {
                return NotFound(new { message = "Sistema não encontrado." });
            }

            // Verifica se a senha do APP confere
            if (login.AppSecret == appBanco.Senha)
            {
                // Se tipo login (APP) for usuario, verificar usuario e senha do usuario
                if (login.App == "Usuario")
                {
                    UsuarioSite usuario = verificaLoginUsuario(loginRepository, login);
                    if (usuario == null)
                    {
                        return BadRequest("Usuário ou senha incorretos.");
                    }
                    if (usuario.RefreshToken == null)
                    {
                        usuario.RefreshToken = await loginRepository.GenerateRefreshToken(usuario.Cpf);
                    }
                    login.Nome = usuario.Nome;
                    login.Email = usuario.Email;
                    login.DoisFatoresHabilitado = usuario.DoisFatoresHabilitado;
                    login.RefreshToken = usuario.RefreshToken.Trim();
                }
                // Oculta a senha
                login.AppSecret = "";
                login.Senha = "";

                //Gerar token de acordo com o nivel usuario/app
                // Gera o Token
                var token = TokenService.GenerateToken(login);

                // Retorna os dados
                return Ok(new
                {
                    login.App,
                    login.Cpf,
                    login.Email,
                    login.Nome,
                    login.DoisFatoresHabilitado,
                    login.RefreshToken, 
                    token
                }
                    );
            }
            else
            {
                return NotFound(new { message = "Senha do APP incorreta" });
            }
        }

        [HttpPost("authenticate-2fa")]
        public async Task<ActionResult<dynamic>> Authenticate2FA(
            [FromServices] LoginRepository loginRepository,
            [FromServices] TwoFactorService twoFactorService,
            [FromBody] Authentication login)
        {

            var appBanco = loginRepository.getApp(login.App);
            var loginRetorno = new UsuarioSite();

            if (appBanco == null)
            {
                return NotFound(new ResponseGenericoResult(false, "Erro Interno.", null));
            }

            if (login.AppSecret == appBanco.Senha)
            {

                UsuarioSite usuario = verificaLoginUsuario(loginRepository, login);

                if (usuario == null)
                {
                    return BadRequest(new ResponseGenericoResult(false, "Usuário ou senha incorretos.", null));
                }

                if (!usuario.IsAtivo)
                {
                    var email = usuario.Email;
                    var substringEmailPre = email.Substring(0, 3);
                    var substringEmailPos = email.Split("@")[1];
                    email = $"{substringEmailPre}******{substringEmailPos}";

                    var mensagem = usuario.IdPerfilUsuario == 4 ? "O cadastro do usuário está inativo. Você precisa ativar seu usuario no email enviado para " + email.ToLower() + ".\n Ou procure a Central de Atendimentos" :
                        "O cadastro do usuário está inativo, procure o administrador";

                    return BadRequest(new ResponseGenericoResult(false, "mensagem", null));
                }

                if (usuario.DoisFatoresHabilitado == true)
                {
                    var token2FA = await twoFactorService.GenerateTwoFactorCodeAsync(usuario.Email);
                    token2FA.DoisFatoresHabilitado = usuario.DoisFatoresHabilitado;
                    token2FA.Nome = usuario.Nome;

                    if (login.App == "Usuario")
                    {
                        var server = _server.ContentRootPath;
                        var pathTemplate = "\\templates\\dois_fatores.html";
                        var path = server + pathTemplate;
                        var contentFile = System.IO.File.ReadAllText(path);

                        contentFile = contentFile
                            .Replace("[[nomeCliente]]", usuario.Nome)
                            .Replace("[[codigoVerificacao]]", token2FA.Token);

                        Util.EnviarEmail(new List<string> { usuario.Email }, "Código de verificação", contentFile);
                    }

                    return Ok(new ResponseGenericoResult(true, "Código gerado com sucesso", token2FA));
                }

                if (usuario.RefreshToken == null)
                {
                    usuario.RefreshToken = await loginRepository.GenerateRefreshToken(usuario.Cpf);
                }

                var token = TokenService.GenerateToken(login);

                loginRetorno.Id = usuario.Id;
                loginRetorno.Nome = usuario.Nome;
                loginRetorno.Token = token;
                loginRetorno.RefreshToken = usuario.RefreshToken.Trim();
                loginRetorno.Cpf = usuario.Cpf;
                loginRetorno.IdPerfilUsuario = usuario.IdPerfilUsuario;
                loginRetorno.Email = usuario.Email;
                loginRetorno.DoisFatoresHabilitado = usuario.DoisFatoresHabilitado;
                loginRetorno.IsAtivo = usuario.IsAtivo;

                return Ok(new ResponseGenericoResult(true, "Sucesso na requisição!", loginRetorno));
            }
            else
            {
                return NotFound(new ResponseGenericoResult(false, "Erro Interno.", null));
            }
        }

        [HttpPost("validate-2fa")]
        public async Task<ActionResult<dynamic>> Validate2FA(
            [FromServices] LoginRepository loginRepository,
            [FromServices] TwoFactorService twoFactorService,
            [FromBody] Authentication login)
        {
            var appBanco = loginRepository.getApp(login.App);

            var loginRetorno = new UsuarioSite();

            if (appBanco == null)
            {
                return NotFound(new { message = "Sistema não encontrado." });
            }

            if (login.AppSecret == appBanco.Senha)
            {

                if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.SecurityStamp) || string.IsNullOrEmpty(login.Token))
                {
                    return BadRequest(new ResponseGenericoResult(false, "Faltam informações para validar o token", null));
                }

                var tokenValido = await twoFactorService.ValidateTwoFactorCodeAsync(login.Email, login.Token, login.SecurityStamp);

                if (tokenValido)
                {
                    UsuarioSite usuario = loginRepository.GetUsuarioSiteEmail(login.Email);

                    if (usuario == null)
                    {
                        return BadRequest(new ResponseGenericoResult(false, "usuário não encontrado", null));
                    }

                    if (usuario.RefreshToken == null)
                    {
                        usuario.RefreshToken = await loginRepository.GenerateRefreshToken(usuario.Cpf);
                    }

                    var token = TokenService.GenerateToken(login);

                    loginRetorno.Id = usuario.Id;
                    loginRetorno.Nome = usuario.Nome;
                    loginRetorno.Token = token;
                    loginRetorno.RefreshToken = usuario.RefreshToken.Trim();
                    loginRetorno.Cpf = usuario.Cpf;
                    loginRetorno.IdPerfilUsuario = usuario.IdPerfilUsuario;
                    loginRetorno.Email = usuario.Email;

                    return Ok(new ResponseGenericoResult(true, "Sucesso na requisição!", loginRetorno));
                }
                else
                {
                    return Ok(new ResponseGenericoResult(false, "Token invalido", null));
                }
            }
            else
            {
                return NotFound(new ResponseGenericoResult(false, "Erro Interno.", null));
            }
        }


        private UsuarioSite verificaLoginUsuario(LoginRepository loginRepository, Authentication login)
        {
            //if (login.Cpf == "99999999999")
            //{
            //    return new UsuarioSite
            //    {
            //        Cpf = "99999999999",
            //        Nome = "SESQUITO TOCANTINS",
            //        IdPerfilUsuario = 1,
            //        Senha = login.Senha,
            //        Email = "sesquito@sescto.com.br",
            //    };
            //}

            var usuario = loginRepository.GetUsuarioSite(login.Cpf);

            if (usuario == null)
            {
                return usuario;
            }
            if (login.GrantType == "refresh_token" && usuario.RefreshToken?.Trim() == login.RefreshToken.Trim())
            {
                return usuario;
            }
            else
            {
                var sha = Sha256(login.Senha);
                if ((login.Senha != null && usuario.Senha == Sha256(login.Senha)))
                {
                    return usuario;
                }
            }
            
            return null;

        }



        private string Sha256(string senha)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(senha), 0, Encoding.UTF8.GetByteCount(senha));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}