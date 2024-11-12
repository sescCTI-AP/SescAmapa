using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SiteSesc.Models.ApiPagamento;

using System.Security.Claims;
using SiteSesc.Models;
using SiteSesc.Models.ViewModel;
using Microsoft.IdentityModel.Tokens;

namespace SiteSesc.Data
{
    public class ApiClient
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment enviroment;

        public HttpClient Cliente { get; private set; }
        public readonly ApiEmbed apiEmbed;

        private readonly string _urlBaseApi;
        public string token { get; set; }
        public ApiClient(HttpClient client, IConfiguration configuration, IWebHostEnvironment env)
        {
            enviroment = env;
            this.configuration = configuration;
            apiEmbed = configuration.GetSection("ApiCliente").Get<ApiEmbed>();
            _urlBaseApi = apiEmbed.Base;
            var handler = new HttpClientHandler();

            if (env.IsDevelopment())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            }

            Cliente = new HttpClient(handler);
            Cliente.BaseAddress = new Uri(apiEmbed!.Base);
            Constructor();
            
        }

        public async void Constructor()
        {
            await ObtemLoginApp();
        }

        public async Task ObtemLoginApp()
        {
            var dados = new
            {
                App = "Site",
                AppSecret = "PWw1YgyXWnLbczhWlVtPXvoiG3POXV81bC5jrvTBY3slzbz4V4RoeBVhXp8wCHxF"
            };
            StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(dados), Encoding.UTF8, "application/json");

            try
            {
                var url = $"{_urlBaseApi}/v1/login";
                var result = await Cliente.PostAsync(url, jsonContent);
                if (result.IsSuccessStatusCode)
                {
                    var temp = JsonConvert.DeserializeObject<LoginApp>(await result.Content.ReadAsStringAsync());
                    token = temp.Token;
                    Cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public async Task ObtemLoginSite(UsuarioViewModel usuario)
        {

            if (!string.IsNullOrEmpty(usuario.Username) && !string.IsNullOrEmpty(usuario.Senha))
            {
                StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");

                var url = $"{_urlBaseApi}/v1/login";
                var result = await Cliente.PostAsync(url, jsonContent);
                if (result.IsSuccessStatusCode)
                {
                    var temp = JsonConvert.DeserializeObject<LoginApp>(await result.Content.ReadAsStringAsync());
                    token = temp.Token;
                    Cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            Console.WriteLine("Token login: " + token);
        }
    }
}