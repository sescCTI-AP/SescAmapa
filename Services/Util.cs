using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using QRCoder;
using SiteSesc.Helpers;
using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.Enums;
using SiteSesc.Models.ViewModel;
using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SiteSesc.Services
{
    public static class Util
    {
        public static string LimparCpf(string cpf) => cpf.Replace("-", "").Replace(".", "");
        public static string PrimeiroSegundoNome(string nomeCompleto)
        {
            var nomes = nomeCompleto.Split(' ');
            if (nomes.Length >= 2)
            {
                return nomes[0] + " " + nomes[1];
            }
            else if (nomes.Length == 1)
            {
                return nomes[0];
            }
            else
            {
                return string.Empty;
            }
        }

        public static int CalculaIdade(DateTime dtnascimen)
        {
            DateTime dataAtual = DateTime.Now;
            int anos = new DateTime(DateTime.Now.Subtract(dtnascimen).Ticks).Year - 1;
            return anos;
        }

        public static List<DateTime> GetDatasInIntervalo(DateTime dataInicio, DateTime dataFim)
        {
            return Enumerable.Range(0, 1 + dataFim.Subtract(dataInicio).Days).Select(offset => dataInicio.AddDays(offset)).ToList();
        }
        public static async Task<string> DownloadImageAsBase64(string url)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            string dns = config.GetSection("HostConfig")["Url"];
            url = dns + url;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Baixar a imagem como array de bytes
                    byte[] imageBytes = await client.GetByteArrayAsync(url);

                    // Converter o array de bytes para Base64
                    string base64Image = Convert.ToBase64String(imageBytes);

                    return base64Image;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao baixar a imagem: {ex.Message}");
                    return null;
                }
            }
        }

        public static string MascararEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                throw new ArgumentException("Email inválido.");
            }

            var atIndex = email.IndexOf('@');
            var localPart = email.Substring(0, atIndex);
            var domainPart = email.Substring(atIndex);

            if (localPart.Length > 2)
            {
                var maskedLocalPart = localPart.Substring(0, 2) + new string('*', localPart.Length - 2);
                return maskedLocalPart + domainPart;
            }

            return localPart + domainPart;
        }

        public static async Task<EmpresaWebService> GetDadosEmpresaByCnpj(string cnpj)

        {
            if (string.IsNullOrEmpty(cnpj))
            {
                return null;
            }
            var cnpjSemFormatacao = Regex.Replace(cnpj, @"[^0-9]+", "");
            if(cnpjSemFormatacao.Length < 14)
            {
                return null;
            }
            var client = new HttpClient();
            var result = await client.GetAsync($"https://www.receitaws.com.br/v1/cnpj/{cnpjSemFormatacao}");
            if (result.IsSuccessStatusCode)
            {
                var empresaWebService = JsonConvert.DeserializeObject<EmpresaWebService>(result.Content.ReadAsStringAsync().Result);
                if (empresaWebService.status.Equals("OK"))
                {
                    return empresaWebService;
                }
                return null;
            }

            return null;
        }

        private static bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        public static Turma MakeTurma(int cdprograma, int cdconfig, int sqocorrenc)
        {
            return new Turma
            {
                CDPROGRAMA = cdprograma,
                CDCONFIG = cdconfig,
                SQOCORRENC = sqocorrenc
            };
        }

        public static bool ValidaCpf(string cpf)
        {
            var valor = cpf.Replace(".", "");

            valor = valor.Replace("-", "");

            if (valor.Length != 11)

                return false;

            var igual = true;

            for (var i = 1; i < 11 && igual; i++)

                if (valor[i] != valor[0])

                    igual = false;


            if (igual || valor == "12345678909")

                return false;


            int[] numeros = new int[11];


            for (int i = 0; i < 11; i++)

                numeros[i] = int.Parse(

                    valor[i].ToString());

            int soma = 0;

            for (int i = 0; i < 9; i++)

                soma += (10 - i) * numeros[i];


            int resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {

                if (numeros[9] != 0)

                    return false;
            }

            else if (numeros[9] != 11 - resultado)

                return false;

            soma = 0;

            for (int i = 0; i < 10; i++)

                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else

            if (numeros[10] != 11 - resultado)
                return false;

            return true;
        }

        public static List<int> CategoriasPleno()
        {
            var lista = new List<int>();
            lista.Add(1);
            lista.Add(3);
            lista.Add(4);
            lista.Add(5);
            lista.Add(8);
            lista.Add(17);
            lista.Add(18);
            lista.Add(23);
            lista.Add(24);
            lista.Add(27);
            lista.Add(32);
            lista.Add(38);
            lista.Add(45);
            return lista;
        }

        public static string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            // Normaliza a string para decompor os caracteres acentuados
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            // Remove os caracteres não-espaciáveis e sinais diacríticos
            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Converte para minúsculas e remove caracteres especiais
            string result = stringBuilder.ToString().ToLowerInvariant();
            result = Regex.Replace(result, @"[^a-z0-9]", string.Empty);

            return result;
        }

        public static int GetTipoPix(string idClasse)
        {
            switch (idClasse)
            {
                case "OCRID":
                    return (int)TipoPix.Cobranca;
                case "CART":
                    return (int)TipoPix.Recarga;
                default:
                    return (int)TipoPix.Avulso;
            }
        }

        public static Turma CdelementToTurma(string cdelement)
        {
            var cdprograma = cdelement.Substring(0, 8);
            var cdconfig = cdelement.Substring(8, 8);
            var sqocorrenc = cdelement.Substring(16, 8);
            return new Turma
            {
                CDPROGRAMA = Convert.ToInt32(cdprograma),
                CDCONFIG = Convert.ToInt32(cdconfig),
                SQOCORRENC = Convert.ToInt32(sqocorrenc)
            };
        }

        public static bool PoucasVagas(int vagas, int ocp)
        {
            var calculo = (ocp * 100) / 100;
            return calculo > 80 && calculo < 100 ? true : false;
        }

        public static string GetDuasPalavras(string texto)
        {
            if (!string.IsNullOrEmpty(texto))
            {
                texto = texto.Trim();
                TextInfo textInfo = new CultureInfo("pt-BR", false).TextInfo;
                texto = textInfo.ToTitleCase(texto.ToLower());
                string[] words = texto.Split(' ');

                if (words.Length >= 2)
                {
                    return words[0] + " " + words[1];
                }
                return words[0];
            }
            return "";
        }

        public class DescricaoProtocoloCancelamento
        {
            public static string[] GetListaDescricao()
            {
                return new[]
                {
                "Solicitação incial",
                "Em atendimento",
                "Em contato com o cliente",
                "Cliente recuperado",
                "Solicitação cancelada",
                "Matricula cancelada"
            };
            }
        }

        public static int GetIdSubModalidade(string dssubmodal)
        {
            int firstDotIndex = dssubmodal.IndexOf('.');
            if (firstDotIndex != -1)
            {
                int secondDotIndex = dssubmodal.IndexOf('.', firstDotIndex + 1);
                if (secondDotIndex != -1)
                {
                    string valueBeforeSecondDot = dssubmodal.Substring(0, secondDotIndex);
                    return Convert.ToInt32(valueBeforeSecondDot.Replace(".", ""));
                }
            }
            return 0;
        }


        public static string GerarThumbnailBase64(string caminho)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            string pastaArquivos = config.GetSection("HostConfig")["PastaArquivosSite"];

            if (config.GetSection("Development")["mode"] == "producao")
            {
                string pastaPai = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                pastaArquivos = pastaArquivos.Replace("/", "\\");
                caminho = caminho.Replace("/", "\\");

                string caminhoCompleto = $"{pastaPai}{pastaArquivos}{caminho}";

                using (var stream = File.OpenRead(caminhoCompleto))
                {
                    using (SKBitmap imagem = SKBitmap.Decode(stream))
                    {
                        return MakeWebPThumbnail(imagem);
                    }
                }
            }
            else
            {
                string url = config.GetSection("HostConfig")["Url"];
                string urlCompleta = $"{url}{pastaArquivos}{caminho}";
                var request = WebRequest.Create(urlCompleta);

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    using (SKBitmap imagem = SKBitmap.Decode(stream))
                    {
                        return MakeWebPThumbnail(imagem);
                    }
                }
            }
        }

        public static string MakeWebPThumbnail(SKBitmap imagem)
        {
            int largura = 150; // largura da miniatura
            int altura = (int)(largura * ((double)imagem.Height / imagem.Width));

            using (SKBitmap resizedImage = imagem.Resize(new SKImageInfo(largura, altura), SKFilterQuality.High))
            {
                using (SKImage skImage = SKImage.FromBitmap(resizedImage))
                {
                    using (SKData data = skImage.Encode(SKEncodedImageFormat.Webp, 75)) // 75 é a qualidade
                    {
                        // Converte os dados WebP em Base64
                        return "data:image/webp;base64," + Convert.ToBase64String(data.ToArray());
                    }
                }
            }
        }

        public static string GerarQrCode(string cdbarras)
        {
            if (cdbarras != null)
            {
                var img = Util.GeneratedQRCode(cdbarras);
                if (img != null)
                {
                    var imagem64 = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(img));
                    return imagem64;
                }
            }
            return null;
        }

        public static byte[] GeneratedQRCode(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public static string MakeImage(Image imagem)
        {

            int largura = 150; // largura da miniatura
            int altura = (int)(largura * ((double)imagem.Height / imagem.Width));
            Bitmap thumbnail = new Bitmap(largura, altura);

            using (Graphics graphic = Graphics.FromImage(thumbnail))
            {
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphic.DrawImage(imagem, 0, 0, largura, altura);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                thumbnail.Save(ms, ImageFormat.Png);
                byte[] byteImage = ms.ToArray();
                string base64String = Convert.ToBase64String(byteImage);
                return "data:image/png;base64," + base64String;
            }
        }

        public static int GetQuantidadeDiasUteisEntreDatas(DateTime dataInicio)
        {
            TimeSpan diferenca = DateTime.Now.Date.Subtract(dataInicio.Date);
            return diferenca.Days;
        }

        public static bool VerificaDataSuperiorOuIgual2Dias(DateTime data)
        {
            DateTime dataHoje = DateTime.Today;
            DateTime dataMais2Dias = dataHoje.AddDays(2);
            return data >= dataMais2Dias;
        }

        public static string IdentificarBandeira(string numeroCartao)
        {
            var padroesBandeiras = new Dictionary<string, string>
            {
                { "^4[0-9]{12}(?:[0-9]{3})?$", "visa" },
                { "^5[1-5][0-9]{14}$", "master" },
                { "^3[47][0-9]{13}$", "amex" },
                { "^6(?:011|5[0-9]{2})[0-9]{12}$", "discover" },
                { "^3(?:0[0-5]|[68][0-9])[0-9]{11}$", "diners" },
                { "^(?:2131|1800|35[0-9]{3})[0-9]{11}$", "jcb" },
                { "^(506[0-9]{4}|6505[0-9]{2}|6504[0-9]{2}|6506[0-9]{2})[0-9]{10,12}$", "elo" },
                { "^(606282|3841)[0-9]{10,13}$", "hiper" }
            };

            foreach (var padrao in padroesBandeiras)
            {
                if (Regex.IsMatch(numeroCartao, padrao.Key))
                {
                    return padrao.Value;
                }
            }

            return "master";
        }

        //public static async Task<string> SubstituirVariaveis<T>(string template, T objeto)
        //{
        //    PropertyInfo[] propriedades = typeof(T).GetProperties();

        //    foreach (var propriedade in propriedades)
        //    {
        //        var atributo = propriedade.GetCustomAttribute<TemplateVariableAttribute>();
        //        if (atributo != null)
        //        {
        //            string nomeVariavel = $"[[ {atributo.NomeVariavel} ]]";
        //            string valorCampo = propriedade.GetValue(objeto)?.ToString() ?? "";
        //            template = template.Replace(nomeVariavel, valorCampo);
        //        }
        //    }

        //    return template;
        //}


        public static async Task<string> SubstituirVariaveis<T>(string template, T objeto)
        {
            PropertyInfo[] propriedades = typeof(T).GetProperties();

            foreach (var propriedade in propriedades)
            {
                var atributo = propriedade.GetCustomAttribute<TemplateVariableAttribute>();
                if (atributo != null)
                {
                    string nomeVariavel = $"[[ {atributo.NomeVariavel} ]]";
                    string valorCampo = ObterValorPropriedade(propriedade, objeto);
                    template = template.Replace(nomeVariavel, valorCampo);
                }
            }

            return template;
        }

        private static string ObterValorPropriedade<T>(PropertyInfo propriedade, T objeto)
        {
            if (propriedade.PropertyType.IsClass && propriedade.PropertyType != typeof(string))
            {
                foreach (var prop in propriedade.PropertyType.GetProperties())
                {
                    var atributo = prop.GetCustomAttribute<TemplateVariableAttribute>();
                    if (atributo != null)
                    {
                        return prop.GetValue(propriedade.GetValue(objeto)).ToString();
                    }
                }
            }

            return propriedade.GetValue(objeto)?.ToString() ?? "";
        }
        public static List<int> ProgramasEducacao()
        {
            var programas = new List<int>();

            for (var i = 3220005; i <= 3280726; i++)
            {
                programas.Add(i);
            }

            programas.Add(3280799);
            programas.Add(3280717);
            programas.Add(3280719);
            programas.Add(3710287);
            programas.Add(3710287);
            programas.Add(3280800);
            programas.Add(3710160);
            programas.Add(3710161);
            programas.Add(3710162);
            programas.Add(3710163);
            programas.Add(3710164);
            programas.Add(3710209);
            programas.Add(3710210);
            programas.Add(3710211);
            programas.Add(3710270);
            programas.Add(3710229);
            programas.Add(3710230);
            programas.Add(3710231);
            programas.Add(3710232);
            programas.Add(3710233);
            programas.Add(3710234);
            programas.Add(3710235);
            programas.Add(3710236);
            programas.Add(3710237);
            programas.Add(3280861);
            programas.Add(3280862);
            programas.Add(3280863);
            programas.Add(3280864);
            programas.Add(3710157);
            programas.Add(3710158);
            programas.Add(3710159);
            programas.Add(3280703);
            programas.Add(3280855);
            programas.Add(3280856);
            programas.Add(3280858);
            programas.Add(3280859);
            programas.Add(3160004);
            programas.Add(3160003);
            programas.Add(3710309);

            for (var i = 3710289; i <= 3710298; i++)
            {
                programas.Add(i);
            }
            return programas.ToList();
        }
    }
}
