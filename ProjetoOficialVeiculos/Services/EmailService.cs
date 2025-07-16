using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;

namespace ProjetoOficialVeiculos.Services
{
    public class EmailService
    {
        private readonly string _remetente = "rafaeleduardodesouza010@gmail.com";
        private readonly string _senhaApp = "aevs givq lhfm rhum";

        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpo)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Sistema de Abastecimento", _remetente));
            email.To.Add(MailboxAddress.Parse(destinatario));
            email.Subject = assunto;
            email.Body = new TextPart("plain") { Text = corpo };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_remetente, _senhaApp);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

