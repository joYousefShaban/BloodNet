using BloodNet.Models;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.Diagnostics;
using System.Text.Encodings.Web;

namespace BloodNet.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendRegisterEmail(EmailDTO request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = "BloodNet - Email Confirmation";
            var messageBody = $"<h1>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(request.CallbackUrl)}'>clicking here</a>.</h1>";
            Debug.WriteLine(messageBody);
            email.Body = new TextPart(TextFormat.Html) { Text = messageBody };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}