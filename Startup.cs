using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SiteSesc.Repositories;
using SiteSesc.Services;
using SiteSesc.Configuracoes;


namespace SiteSesc
{
    public class Startup
    {
        private readonly IWebHostEnvironment enviroment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            enviroment = env;

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddDbContext<SiteSescContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Site")), ServiceLifetime.Transient);
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<Ambiente>(Configuration.GetSection("Ambiente"));

            #region ApiPagamentoV2
            ConfigurationApi.ApiPagamentoV2Url = Configuration.GetSection("ApiPagamentoV2").GetValue<string>("Url") ?? string.Empty;
            ConfigurationApi.ApiPagamentoV2ClientId = Configuration.GetSection("ApiPagamentoV2").GetValue<string>("client_id") ?? string.Empty;
            ConfigurationApi.ApiPagamentoV2ClientSecret = Configuration.GetSection("ApiPagamentoV2").GetValue<string>("client_secret") ?? string.Empty;

            services.AddHttpClient<ApiPagamentoV2Service>(client =>
            {
                client.BaseAddress = new Uri(ConfigurationApi.ApiPagamentoV2Url);
                client.DefaultRequestHeaders.Add("ContentType", "application/json");
                client.DefaultRequestHeaders.Add("client_id", ConfigurationApi.ApiPagamentoV2ClientId);
                client.DefaultRequestHeaders.Add("client_secret", ConfigurationApi.ApiPagamentoV2ClientSecret);
            });
            #endregion
            // using Microsoft.AspNetCore.Authentication.Cookies;
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    // using Microsoft.AspNetCore.Http;
                    options.LoginPath = new PathString("/Login");
                    options.LogoutPath = new PathString("/Logout");

                    options.ReturnUrlParameter = "Retorno";

                    options.Cookie.HttpOnly = true;
                    //Verifica ambiente para definir segurança do cookie
                    options.Cookie.SecurePolicy = enviroment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always; //Prod = .Always
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                });

            #region Injeção de dependencia
            //Antes Scoped
            services.AddScoped<EmailService>();
            services.AddScoped<DefaultRepository, DefaultRepository>();
            services.AddScoped<UsuarioRepository, UsuarioRepository>();
            services.AddScoped<ClienteRepository, ClienteRepository>();
            services.AddScoped<ArquivoRepository, ArquivoRepository>();
            services.AddScoped<CidadeRepository, CidadeRepository>();
            services.AddScoped<AreaRepository, AreaRepository>();
            services.AddScoped<ParentescoRepository, ParentescoRepository>();
            services.AddScoped<InscritosRepository>();
            services.AddScoped<StatusRepository, StatusRepository>();
            services.AddScoped<InscritosRepository, InscritosRepository>();
            services.AddScoped<SolicitacaoCadastroRepository, SolicitacaoCadastroRepository>();
            services.AddScoped<MensagemRapidaRepository, MensagemRapidaRepository>();
            services.AddScoped<AtividadeOnLineReposotory, AtividadeOnLineReposotory>();
            services.AddScoped<CobrancaRepository, CobrancaRepository>();
            services.AddScoped<EmpresaRepository, EmpresaRepository>();
            services.AddScoped<LogRepository, LogRepository>();
            services.AddScoped<UnidadeRepository, UnidadeRepository>();
            services.AddSingleton<HostConfiguration, HostConfiguration>();
            services.AddScoped<SafeExecutor>();
            services.AddHttpClient<ApiClient>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(10); // Definindo timeout para 10 minutos
            });
            #endregion

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddCors();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });
            services.AddMemoryCache();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                //app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
                //app.UseHsts();

            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                //app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
                //app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(option => option.AllowAnyOrigin());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Cliente",
                    areaName: "Cliente",
                    pattern: "cliente/{controller=Cliente}/{action=Index}");

                endpoints.MapAreaControllerRoute(
                    name: "Admin",
                    areaName: "Admin",
                    pattern: "admin/{controller=Home}/{action=Dashboard}");

                endpoints.MapControllerRoute("default",
                                 "{controller=Atividade}/{action=DetalhesAtividade}/{cdprograma}/{cdconfig}/{sqocorrenc}/{title}");

                endpoints.MapControllerRoute(
                    name: "detalhes",
                    pattern: "atividade/detalhes/{cdelement}",
                    defaults: new { controller = "Atividade", action = "Details" });

            });
        }
    }
}