using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.RegularExpressions;
using PagamentoApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System;

namespace PagamentoApi.Services
{
    public class Util
    {
        public static byte[] RedimensionarImagem(byte[] imagemBytes, int tamanhoMaximo)
        {
            using (var ms = new MemoryStream(imagemBytes))
            using (var imagem = Image.FromStream(ms))
            {
                int largura = imagem.Width;
                int altura = imagem.Height;
                int qualidade = 100; // Começa com a qualidade máxima
                byte[] imagemRedimensionada;

                var codecInfo = GetEncoderInfo("image/jpeg");
                var parametros = new EncoderParameters(1);

                do
                {
                    using (var novaImagem = new Bitmap(imagem, new Size(largura, altura)))
                    {
                        using (var msRedimensionada = new MemoryStream())
                        {
                            parametros.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualidade);
                            novaImagem.Save(msRedimensionada, codecInfo, parametros);
                            imagemRedimensionada = msRedimensionada.ToArray();
                        }
                    }

                    // Se a imagem ainda estiver acima do tamanho limite
                    if (imagemRedimensionada.Length > tamanhoMaximo)
                    {
                        if (qualidade > 10)
                        {
                            qualidade -= 5; // Reduz a qualidade primeiro (passo menor para evitar perda brusca)
                        }
                        else
                        {
                            largura = (int)(largura * 0.95);
                            altura = (int)(altura * 0.95); // Só reduz a resolução se a qualidade já estiver baixa
                        }
                    }

                } while (imagemRedimensionada.Length > tamanhoMaximo && largura > 50 && altura > 50); // Mantemos um tamanho mínimo de 50x50 px para evitar distorções

                return imagemRedimensionada;
            }
            
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (var encoder in encoders)
            {
                if (encoder.MimeType == mimeType)
                {
                    return encoder;
                }
            }
            return null;
        }

        public static async Task<RetornoDadosEmpresa> GetDadosEmpresa(string cnpj)
        {
            var cnpjSemFormatacao = Regex.Replace(cnpj, @"[^0-9]+", "");
            var client = new HttpClient();
            var result = await client.GetAsync($"https://www.receitaws.com.br/v1/cnpj/{cnpjSemFormatacao}");
            if (result.IsSuccessStatusCode)
            {
                var resultado = result.Content.ReadAsStringAsync().Result;
                var empresaWebService = JsonConvert.DeserializeObject<RetornoDadosEmpresa>(result.Content.ReadAsStringAsync().Result);
                return empresaWebService;
            }
            return null;
        }

        public static void EnviarEmail(List<string> listaEmailsDestinatarios, string assunto, string conteudo, byte[] file = null)
        {
            try
            {
                var smtpAddress = "srvsmtp01.sescto.com.br";
                var portNumber = 25;
                var nomeRemetente = "Sesc Tocantins";
                var emailRemetente = "contato@sescto.com.br";
                var senhaEmailRemetente = "";

                var mensagemEmail = new MailMessage
                {
                    From = new MailAddress(emailRemetente, nomeRemetente, Encoding.UTF8)
                };

                foreach (var destinatario in listaEmailsDestinatarios)
                {
                    mensagemEmail.To.Add(destinatario);
                }

                mensagemEmail.Subject = assunto;
                mensagemEmail.Body = conteudo;
                mensagemEmail.IsBodyHtml = true;
                mensagemEmail.BodyEncoding = Encoding.UTF8;
                mensagemEmail.Priority = MailPriority.High;
                if (file != null)
                {
                    var data = new Attachment(new MemoryStream(file), "sesc-tocantins.pdf", MediaTypeNames.Application.Pdf);
                    mensagemEmail.Attachments.Add(data);
                }

                using (var smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    //smtp.UseDefaultCredentials = true;
                    //smtp.Credentials = new NetworkCredential(emailRemetente, senhaEmailRemetente);
                    smtp.EnableSsl = false;
                    smtp.Send(mensagemEmail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception($"Ocorreu um erro ao enviar o email: \n{ex.Message}");
            }
        }
    }
}
