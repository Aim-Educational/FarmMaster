using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FarmMaster.Services
{
    public class EmailSenderConfig
    {
        public string Server { get; set; }
        public ushort Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class EmailSender : IEmailSender
    {
        readonly IOptionsSnapshot<EmailSenderConfig> _config;

        public EmailSender(IOptionsSnapshot<EmailSenderConfig> config)
        {
            this._config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var config = this._config.Value;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(config.Username));
            message.To.Add(new MailboxAddress(email));
            message.Subject = subject;

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using(var client = new SmtpClient())
            {
                await client.ConnectAsync(config.Server, config.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(config.Username, config.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
