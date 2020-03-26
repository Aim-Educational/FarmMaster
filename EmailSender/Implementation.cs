using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace EmailSender
{
    public class TemplatedEmailSender : ITemplatedEmailSender
    {
        EmailSenderConfig _config;
        readonly IHttpContextAccessor _accessor;
        readonly LinkGenerator _generator;
        SmtpClient _client;

        public TemplatedEmailSender(IOptions<EmailSenderConfig> config, IHttpContextAccessor accessor, LinkGenerator generator)
        {
            this._config = config.Value;
            this._accessor = accessor;
            this._generator = generator;
        }

        public Task ReloadAsync(EmailSenderConfig config)
        {
            this._config = config;
            return this.CreateClientAsync(true);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtp        = this._config.Smtp;
            var message     = this.CreateMessage(smtp, email);
            message.Subject = subject;
            message.Body    = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            await this.CreateClientAsync();
            await this._client.SendAsync(message);
        }

        public async Task<EmailResult> SendTemplatedEmailAsync(string toAddress, EmailTemplate template, EmailTemplateValues values)
        {
            var config   = this._config;
            var contents = template.Resolve(values, this._accessor, this._generator);
                contents = config.Layout.Resolve(new EmailTemplateValues { { "body", contents } }, this._accessor, this._generator);

            try
            {
                var message     = this.CreateMessage(config.Smtp, toAddress);
                message.Subject = "TODO";
                message.Body    = new TextPart(TextFormat.Html)
                {
                    Text = contents
                };

                await this.CreateClientAsync();
                await this._client.SendAsync(message);
            }
            catch(Exception ex)
            {
                return new EmailResult { Succeeded = false, Error = ex.Message };
            }

            return new EmailResult { Succeeded = true };
        }

        private MimeMessage CreateMessage(EmailSenderSmtpConfig smtp, string toAddress)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtp.Alias ?? smtp.Username, smtp.Address ?? smtp.Username));
            message.To.Add(new MailboxAddress(toAddress));

            return message;
        }

        private async Task CreateClientAsync(bool closeExisting = false)
        {
            var config = this._config;

            if (this._client != null && closeExisting)
            {
                await this._client.DisconnectAsync(true);
                this._client.Dispose();
                this._client = null;
            }

            if (this._client == null)
                this._client = new SmtpClient();

            if (!this._client.IsConnected)
                await this._client.ConnectAsync(config.Smtp.Server, config.Smtp.Port, true);

            if(!this._client.IsAuthenticated)
                await this._client.AuthenticateAsync(config.Smtp.Username, config.Smtp.Password);
        }
    }

    public static class Extensions 
    {
        public static IServiceCollection AddTemplatedEmailSender(this IServiceCollection services)
        {
            return services.AddSingleton<ITemplatedEmailSender, TemplatedEmailSender>()
                           .AddSingleton<IEmailSender, TemplatedEmailSender>();
        }
    }
}
