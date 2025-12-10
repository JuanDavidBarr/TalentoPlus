using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TalentoPlus.Services.Interfaces;

namespace TalentoPlus.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly string _fromName;
        private readonly string _fromEmail;

        public EmailService()
        {
            _smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
            _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
            _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? "";
            _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
            _fromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME") ?? "TalentoPlus S.A.S.";
            _fromEmail = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL") ?? "";
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string employeeName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress(employeeName, toEmail));
            message.Subject = "¡Bienvenido a TalentoPlus S.A.S.!";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #0066cc; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .footer {{ text-align: center; padding: 10px; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>¡Bienvenido a TalentoPlus S.A.S.!</h1>
                        </div>
                        <div class='content'>
                            <p>Estimado/a <strong>{employeeName}</strong>,</p>
                            <p>Tu registro en nuestra plataforma ha sido completado exitosamente.</p>
                            <p>Ya puedes autenticarte en la plataforma utilizando tu <strong>número de documento</strong> y <strong>correo electrónico</strong> registrados.</p>
                            <p>Si tienes alguna pregunta, no dudes en contactar al equipo de Recursos Humanos.</p>
                            <br/>
                            <p>¡Gracias por unirte a nuestro equipo!</p>
                            <p>Atentamente,<br/><strong>Equipo de TalentoPlus S.A.S.</strong></p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático, por favor no responda a este mensaje.</p>
                            <p>&copy; {DateTime.Now.Year} TalentoPlus S.A.S. Todos los derechos reservados.</p>
                        </div>
                    </div>
                </body>
                </html>",
                TextBody = $@"
                ¡Bienvenido a TalentoPlus S.A.S.!

                Estimado/a {employeeName},

                Tu registro en nuestra plataforma ha sido completado exitosamente.

                Ya puedes autenticarte en la plataforma utilizando tu número de documento y correo electrónico registrados.

                Si tienes alguna pregunta, no dudes en contactar al equipo de Recursos Humanos.

                ¡Gracias por unirte a nuestro equipo!

                Atentamente,
                Equipo de TalentoPlus S.A.S.
                "
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUser, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}