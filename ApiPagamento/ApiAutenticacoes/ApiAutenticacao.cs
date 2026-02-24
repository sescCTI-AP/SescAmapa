using Microsoft.Extensions.Configuration;
using System;

namespace PagamentoApi.ApiAutenticacoes
{
    public class ApiAutenticacao : IApiAutenticacao
    {
        private readonly IConfiguration _configuration;
        public string ServidorApiBB { get; set; }

        public ApiAutenticacao(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsValidoChaveApi(string chaveApi)
        {

            if (string.IsNullOrWhiteSpace(chaveApi))
                return false;
            
            //string? apiKey = _configuration.GetValue<string>(Constants.ApiChaveNome);
            string? apiKey = "7-GyugiLCRi2=bQ/y0gCX/FG0Sa6OtO=EE-aXewE2dnrlzOGBZAe5FiGpp9g4Db2-FDXdSllokvh78FSjJNASlrUjmqSphxLfMxPNOYp6KRd8TRbbTbLmJipqWO5tDUCqQeCh/y6c!LgzCBfcoxq!ro12o5/HrnJChyW5HCFnTXi9y66G/z-su3h7j/kE4Y?8KIhCOMOAn8h5tL15yjvBcQz1!CcqhPhK?PHH08QehlPB!npYpsE8cmrATezlG!l";
            // throw new Exception(chaveApi + " - " + apiKey);
            if (apiKey == null || apiKey != chaveApi)
                return false;
            
            return true;
        }
    }
}
