using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Repositories;
using SiteSesc.Services;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using SiteSesc.Models.ViewModel;
using SiteSesc.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SiteSesc.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly SiteSescContext _context;
        private IWebHostEnvironment _server;
        private readonly IConfiguration configuration;
        public readonly HostConfiguration hostConfiguration;
        private UsuarioRepository _usuarioRepository;
        private IMemoryCache _memoryCache;
        private readonly EmailService _emailService;
        private readonly ClienteRepository _clienteRepository;
        private EmpresaRepository _empresaRepository;



        public UsuarioController(SiteSescContext context, 
            IWebHostEnvironment server, 
            IConfiguration configuration, 
            UsuarioRepository usuarioRepository, 
            IMemoryCache memoryCache, 
            EmailService emailService,
            ClienteRepository clienteRepository)
        {
            this.configuration = configuration;
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _context = context;
            _server = server;
            _usuarioRepository = usuarioRepository;
            _memoryCache = memoryCache;
            _emailService = emailService;
            _clienteRepository = clienteRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("login")]
        public async Task<ActionResult> Login(string actionRequest, string control, int? programa, int? config, int? ocorr, string cdelement =  null)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (((ClaimsIdentity)User.Identity).FindFirst("Id") != null)
                {
                    var cliente = await _usuarioRepository.GetUsuario(new Guid(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value));
                    if (cliente != null)
                        return RedirectToAction("Index", "Cliente", new { area = "Cliente" });
                }
            }
            ViewBag.UrlRedirect = cdelement;
            return View();
        }


        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UsuarioViewModel usuario, string cdelement = null)
        {
            if (ModelState.IsValid)
            {
                var autenticar = await _usuarioRepository.Autenticar(usuario);
                if (autenticar == null)
                {
                    ViewBag.Validacao = "Usuário ou senha incorretos";
                    return View(usuario);
                }

                var usuarioLogado = _usuarioRepository.verificaLogin(usuario);
                if(usuarioLogado == null)
                {
                    ViewBag.Validacao = "Usuário ou senha incorretos";
                    return View();
                }

                var email = usuarioLogado.Email;
                
                
                var substringEmailPre = email.Substring(0, 3);
                var substringEmailPos = email.Split("@")[1];
                email = $"{substringEmailPre}******{substringEmailPos}";

                if (!usuarioLogado.IsAtivo)
                {
                    ViewBag.Validacao = usuarioLogado.IdPerfilUsuario == 4 ? "O cadastro do usuário está inativo. Você precisa ativar seu usuario no email enviado para " + email.ToLower() + ".\n Ou procure a Central de Atendimentos" :
                        "O cadastro do usuário está inativo, procure o administrador";
                    return View(usuario);
                }

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, usuarioLogado.Nome.Trim()));
                identity.AddClaim(new Claim("CPF", usuarioLogado.Cpf));
                identity.AddClaim(new Claim("Id", usuarioLogado.Id.ToString()));
                identity.AddClaim(new Claim("Perfil", usuarioLogado.IdPerfilUsuario.ToString()));
                identity.AddClaim(new Claim("Usuario", "Cliente"));
                identity.AddClaim(new Claim(ClaimTypes.GivenName, usuarioLogado.Guid.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Sid, usuarioLogado.Guid.ToString()));

                var clienteCentral = await _clienteRepository.ObterClientePorCpf(usuarioLogado.Cpf);

                ViewBag.isCliente = "Cliente";

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


                if (clienteCentral == null)                
                    return RedirectToAction("EscolhaCategoria", "SolicitacaoCadastroCliente", new { area = "Cliente" });
                if (!string.IsNullOrEmpty(cdelement))
                {
                    return RedirectToAction("Details","Atividade", new {cdelement = cdelement });
                }
                return RedirectToAction("Index", "Cliente", new { area = "Cliente" });

                //DESCOMENTAR POSTERIOMENTE
            }
            ViewBag.Validacao = "Preencha todos os dados";
            return View(usuario);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [Route("novo-usuario")]
        public IActionResult Create([FromServices] UsuarioRepository usuarioRespository)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("novo-usuario")]
        public async Task<IActionResult> Create(UsuarioCreate userCreate)
        {
            var usuario = new Usuario();
            if (!ModelState.IsValid)
            {
                return View(userCreate);
            }
            try
            {
                usuario = UsuarioCreate.ToUsuario(userCreate);
                usuario.SetSenha(usuario.Senha);
                usuario.SetCpf(usuario.Cpf);
                usuario.Username = usuario.Cpf; //mudamos para pegar usuario o CPF tezste
            }
            catch (ArgumentException ex)
            {
                ViewBag.Validation = ex.Message;
                return View(userCreate);
            }

            try
            {
                var verificaUsuario = _usuarioRepository.VerificaUsuarioExistente(usuario.Cpf, usuario.Username, usuario.Email);
                if (verificaUsuario)
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            await _context.AddAsync(usuario);
                            await _context.SaveChangesAsync();
                            transaction.Commit();

                            Dictionary<string, string> listaParametrosEmail = new Dictionary<string, string>();
                            listaParametrosEmail.Add("[[nomeCliente]]", usuario.Nome);
                            listaParametrosEmail.Add("[[urlSite]]", hostConfiguration.Url);
                            listaParametrosEmail.Add("[[guid]]", usuario.Guid.ToString());

                            var bodyEmail = BuildTemplateEmail.Make(_server, "\\templates\\templates-email\\confirmacao_cadastro.html", listaParametrosEmail);
                            var enviaEmail = _emailService.Send(usuario.Email, bodyEmail, "Confirmação de cadastro");

                            ViewBag.EnviaEmail = enviaEmail;
                            return View("Login");
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            ViewBag.Validation = e.Message;
                            return View(userCreate);
                        }
                    }
                }
                return View(userCreate);
            }
            catch (ArgumentException ex)
            {
                ViewBag.Validation = ex.Message;
                return View(userCreate);
            }
        }

        [Route("usuario/ativar-usuario/{guid}")]
        public async Task<IActionResult> AtivarUsuario(string guid)
        {
            ViewBag.Guid = guid;
            return View();
        }

        [HttpPost]
        [Route("usuario/ativar")]
        public async Task<IActionResult> Ativar(Guid usuarioGuid)
        {
            try
            {
                var usuario = await _usuarioRepository.Ativar(usuarioGuid);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("recuperar-senha/{cpf}")]
        public async Task<IActionResult> RecuperarSenha(string cpf)
        {
            try
            {
                cpf = cpf.Replace(".", "").Replace("-", "");
                if (Util.ValidaCpf(cpf))
                {
                    var usuario = await _usuarioRepository.GetUsuarioCpf(cpf);
                    if (usuario != null)
                    {
                        // Envia Link de recuperação de senha
                        Dictionary<string, string> listaParametrosEmail = new Dictionary<string, string>();
                        listaParametrosEmail.Add("[[nomeCliente]]", usuario.Nome);
                        listaParametrosEmail.Add("[[urlSite]]", hostConfiguration.Url);
                        listaParametrosEmail.Add("[[guid]]", usuario.Guid.ToString());

                        var bodyEmail = BuildTemplateEmail.Make(_server, "\\templates\\templates-email\\recuperacao_senha.html", listaParametrosEmail);
                        var enviaEmail = _emailService.Send(usuario.Email, bodyEmail, "Recuperação de senha");
                        if (enviaEmail)
                        {
                            var email = Util.MascararEmail(usuario.Email);
                            return Ok("Foi enviado um email para " + email + ". Clique no link enviado para resetar a senha.");
                        }
                        else
                        {
                            return BadRequest("Erro ao enviar o e-mail. Verifique as configurações do servidor de e-mail.");
                        }
                    }
                    else
                    {
                        return BadRequest("Usuário não encontrado.");
                    }
                }
                return BadRequest("CPF inválido.");
            }
            catch (MailKit.Security.AuthenticationException)
            {
                return BadRequest("Erro de autenticação ao tentar enviar o e-mail. Verifique as credenciais do servidor de e-mail.");
            }
            catch (Exception e)
            {
                return BadRequest("Ocorreu um erro inesperado. Tente novamente mais tarde.");
            }
        }


        [HttpGet("alterar-senha/{guid}")]
        public async Task<IActionResult> AlterarSenha(string guid)
        {
            ViewBag.Guid = guid;
            return View();
        }
        
        [HttpPost("alterar-senha")]
        public async Task<IActionResult> ConfirmaSenha(UsuarioResetSenha user)
        {
            if (!string.IsNullOrEmpty(user.Senha))
            {
                if (user.Senha != user.ConfirmarSenha)
                {
                    ViewBag.Validation = "As senhas não conferem";
                    return RedirectToAction("AlterarSenha", "Home", new { guid = user.Guid });
                }

                var usuario = await _usuarioRepository.GetUsuario(Guid.Parse(user.Guid.ToString()));

                // Verifica se o usuário está inativo
                if (!usuario.IsAtivo)
                {
                    // Ativa o usuário
                    usuario.IsAtivo = true;
                    await _usuarioRepository.Ativar(usuario.Guid);
                }

                usuario.Senha = Seguranca.Sha256(user.Senha);
                var atualizaUsuario = await _usuarioRepository.UpdateUsuario(usuario);

                if (atualizaUsuario)
                {
                    return RedirectToAction("Login", "Usuario");
                }
                else
                {
                    return RedirectToAction("AlterarSenha", "Home", new { guid = user.Guid });
                }
            }
            else
            {
                return RedirectToAction("AlterarSenha", "Home", new { guid = user.Guid });
            }
        }

    }
}
