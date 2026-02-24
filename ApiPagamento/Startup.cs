using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PagamentoApi.ApiAutenticacoes;
using PagamentoApi.Clients;
using PagamentoApi.Models;
using PagamentoApi.Repositories;
using PagamentoApi.Services;
using PagamentoApi.Settings;
using PagamentoApi.SignalR;
using PagamentoApi.Swagger;
using PagamentoApi.V2.Settings;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PagamentoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Env = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            /*
            services.AddRestApiService();
            services.AddBaseApiEndPoints();
            services.AddBaseApiAggregators();
            services.AddBbApiAgrregator();
            */
            
            services.AddControllers();
            services.AddSignalR();
            services.AddSingleton<ClienteConectado, ClienteConectado>();

            var urlCors = Env.IsProduction() == true ? "https://www.sescto.com.br" : "http://sistemas.localhost.sescto.com.br:4001";
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins(urlCors)
                        .AllowAnyHeader()
                        .WithMethods("GET", "POST")
                        .AllowCredentials();
                });
            });

            //  Adicionando o controle de versionamento da API
            services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = ApiVersion.Default; //new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;

            });
            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerDefaultValues>();
            });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

            /*
            services.AddSwaggerGen(c =>
            {
               c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pagamento API", Version = "v1" });
            });
            */
            ///////////////////////////////////////////////////////////////////////////////////////////

            var key = Encoding.ASCII.GetBytes(Settings.AuthSettings.Secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            services.AddLogging();

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<UsuarioCliente, UsuarioCliente>();
            services.AddTransient<CobrancaRepository, CobrancaRepository>();
            services.AddTransient<ClientelaRepository, ClientelaRepository>();
            services.AddTransient<CaixaRepository, CaixaRepository>();
            services.AddTransient<CieloRepository, CieloRepository>();
            services.AddTransient<CartaoRepository, CartaoRepository>();
            services.AddTransient<AtividadesRepository, AtividadesRepository>();
            services.AddTransient<LoginRepository, LoginRepository>();
            services.AddTransient<BoletoRepository, BoletoRepository>();
            services.AddTransient<ReportRepository, ReportRepository>();
            services.AddTransient<PixRepository, PixRepository>();
            services.AddTransient<NoticiaRepository, NoticiaRepository>();
            services.AddTransient<TotemRepository, TotemRepository>();
            services.AddTransient<SiteRepository, SiteRepository>();
            services.AddTransient<VendasRepository, VendasRepository>();
            services.AddTransient<SolicitaaoReembolsoRepository, SolicitaaoReembolsoRepository>();

            services.AddSingleton<SemaphoreSlim>(new SemaphoreSlim(1));

            services.AddTransient<IApiAutenticacao, ApiAutenticacao>();

            services.AddIdentityCore<ApplicationUser>().AddDefaultTokenProviders();
            services.AddScoped<IUserStore<ApplicationUser>, InMemoryUserStore>();
            services.AddScoped<TwoFactorService>();

            #region Api Pix
            ConfigurationApi.ApiPixUrl = Configuration.GetSection("ApiPix").GetValue<string>("Url") ?? string.Empty;
            ConfigurationApi.ApiPixClientId = Configuration.GetSection("ApiPix").GetValue<string>("client_id") ?? string.Empty;
            ConfigurationApi.ApiPixClientSecret = Configuration.GetSection("ApiPix").GetValue<string>("client_secret") ?? string.Empty;
            #endregion

            var cieloApiSettings = Configuration.GetSection("CieloApiSettings").Get<CieloApiSettings>(); //Settings stored in app.config (base url, api key to add to header for all requests)
            var bbApiSettings = Configuration.GetSection("BBApiSettings").Get<BBApiSettings>();
            var bbApiSettingsV2 = Configuration.GetSection("BBApiSettingsV2").Get<BBApiSettingsV2>();
            var certificadoSettings = Configuration.GetSection("CertificadoSettings").Get<CertificadoSettings>();
            var bBApiPixV2Settings = Configuration.GetSection("BBApiPixV2Settings").Get<BBApiPixV2Settings>();

            services.AddSingleton(cieloApiSettings);
            services.AddSingleton(bbApiSettings);
            services.AddSingleton(bbApiSettingsV2);
            services.AddSingleton(certificadoSettings);
            services.AddSingleton(bBApiPixV2Settings);
            services.AddSingleton<ApiBancoBrasilService, ApiBancoBrasilService>();

            services.AddHttpClient<CieloClient>();
            services.AddHttpClient<BBClient>();
            services.AddHttpClient<BBClientPixV2>();
            //services.AddHttpClient<ApiBancoBrasilService>();

            //if (Env.IsProduction())
            //{
                services.AddCronJob<AtualizarBoletos>(c =>
                {
                    c.TimeZoneInfo = TimeZoneInfo.Local;
                    c.CronExpression = @"* 3-22/9 * * *";
                });

                services.AddCronJob<AtualizarPix>(c =>
                {
                    c.TimeZoneInfo = TimeZoneInfo.Local;
                    c.CronExpression = @"*/1 * * * *";
                });

                services.AddCronJob<FecharCaixa>(c =>
                {
                    c.TimeZoneInfo = TimeZoneInfo.Local;
                    //c.CronExpression = @"*/5 * * * *";
                    c.CronExpression = @"59 23 * * *";
                });
            //}
        }   
        public void Configure(IApplicationBuilder app)
        {
           
            var versionDescriptionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
            
            // Definindo a cultura para pt-BR
            var defaultCulture = new CultureInfo("pt-BR");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            };
            app.UseRequestLocalization(localizationOptions);
            
           // if (Env.IsDevelopment())
           // {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                // Ativa o Swagger UI
                app.UseSwaggerUI(options =>
                {
                    foreach (var description in versionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            $"API Pagamento - {description.GroupName.ToUpper()}");
                    }
                });
            //}

            // app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors(option => option.AllowAnyOrigin());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<WebHookHub>("/webhookhub");
            });
        }
    }
}