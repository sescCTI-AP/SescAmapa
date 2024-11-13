using MimeKit;
using SiteSesc.Data;
using System.Net.Mail;

namespace SiteSesc.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IConfiguration configuration)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
        }


        public bool Send(string email, string body, string assunto)
        {
            #region Envia email de confirmação e validação do cadastro
            EnviarEmailSubmit(_emailSettings, new List<string> { email }, assunto, body);
            return true;
            #endregion
        }

        private void EnviarEmailSubmit(EmailSettings emailSettings, List<string> listaEmailsDestinatarios, string assunto, string conteudo, byte[] file = null)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(emailSettings.FromName, emailSettings.FromAddress));
                foreach (var email in listaEmailsDestinatarios)
                {
                    message.To.Add(new MailboxAddress(email, email));
                }
                message.Subject = assunto;
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = conteudo
                };

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(emailSettings.SmtpHost, emailSettings.SmtpPort, false);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro ao enviar o email usando {emailSettings.FromAddress}: \n{ex.Message}");
                throw;
            }
        }
    }
}

