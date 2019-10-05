using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Business.Model;
using Microsoft.Extensions.Options;

namespace FarmMaster.Services
{
    /// <summary>
    /// Configuration for being able to create an SmtpClient.
    /// </summary>
    public class IServiceSmtpClientConfig
    {
        /// <summary>
        /// The IP/URL of the host SMTP server.
        /// </summary>
        public string Host;

        /// <summary>
        /// The port of the SMTP server.
        /// </summary>
        public int Port;

        /// <summary>
        /// The credentials used to log into the account being used to send these emails.
        /// </summary>
        public NetworkCredential Credentials;
    }

    /// <summary>
    /// Configuration of the various callbacks that certain parts of AimLogin is required to send.
    /// </summary>
    public class IServiceSmtpDomainConfig
    {
        /// <summary>
        /// NOTE: The verify token for the UserEmail to be verified will be appended onto the end, so the domain should
        /// end with an incomplete query value, .e.g "?token="
        /// </summary>
        public string VerifyEmail;
    }

    /// <summary>
    /// Configure the email templates that IServiceSmtpClient is aware of.
    /// </summary>
    public class IServiceSmtpTemplateConfig
    {
        /// <summary>
        /// Key = The name of the template (e.g. 'password_change').
        /// Value = The path to the razor template (e.g. '/Views/Emails/PassChange.cshtml').
        /// </summary>
        public Dictionary<string, string> EmailTemplates;

        public IServiceSmtpTemplateConfig()
        {
            this.EmailTemplates = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// A barebones wrapper around an SmtpClient, with useful functions (and extentions) to make it easier
    /// to send messages to users.
    /// </summary>
    public interface IServiceSmtpClient
    {
        /// <summary>
        /// Sends the given <paramref name="message"/> to the specified <paramref name="user"/>
        /// </summary>
        /// <param name="user">The user to send the message to.</param>
        /// <param name="message">The message to send.</param>
        Task SendToAsync(User user, MailMessage message);

        Task SendToAsync(IEnumerable<string> emails, MailMessage message);

        /// <summary>
        /// Renders a razor template with the given <paramref name="templateName"/>, and sends the rendered HTML to the
        /// specified <paramref name="user"/>.
        /// </summary>
        /// <remarks>
        /// The <paramref name="templateName"/> is used as a key for the <see cref="IServiceSmtpTemplateConfig.IServiceSmtpTemplateConfig"/> dictionary.
        /// </remarks>
        /// <param name="user">The user to send the message to.</param>
        /// <param name="templateName">The name of the template to render.</param>
        /// <param name="subject">The subject to give the message.</param>
        /// <param name="model">The model to pass to the template.</param>
        Task SendToWithTemplateAsync(User user, string templateName, string subject, object model);

        Task SendToWithTemplateAsync(IEnumerable<string> emails, string templateName, string subject, object model);

        /// <summary>
        /// Determines whether a template exists.
        /// </summary>
        /// <param name="templateName">The name of the template to check.</param>
        /// <returns>True if it exists. False otherwise.</returns>
        bool ContainsTemplate(string templateName);
    }

    public class ServiceSmtpClient : IServiceSmtpClient, IDisposable
    {
        const string FROM_NAME = "FarmMaster System";
        
        readonly IOptions<IServiceSmtpClientConfig> _config;
        readonly IOptions<IServiceSmtpTemplateConfig> _templates;
        readonly IViewRenderService _viewRenderer;

        private SmtpClient _smtpInstance;
        SmtpClient _smtp
        {
            // Lazy load, since our dependencies prevent this from being a singleton, and making this every request is expensive.
            get
            {
                if (this._smtpInstance == null)
                {
                    this._smtpInstance = new SmtpClient(this._config.Value.Host, this._config.Value.Port)
                    {
                        Credentials = this._config.Value.Credentials,
                        EnableSsl = true
                    };
                }

                return this._smtpInstance;
            }
        }

        public ServiceSmtpClient(IOptions<IServiceSmtpClientConfig> config,
                                 IOptions<IServiceSmtpTemplateConfig> templates,
                                 IViewRenderService viewRenderer)
        {
            this._config = config;
            this._templates = templates;
            this._viewRenderer = viewRenderer;
        }

        public async Task SendToAsync(IEnumerable<string> emails, MailMessage message)
        {
            if(emails.Any())
                return;

            foreach (var email in emails)
                this.SetToAndFrom(email, ref message);

            await this._smtp.SendMailAsync(message);
        }

        public Task SendToAsync(User user, MailMessage message)
        {
            return this.SendToAsync(user.Contact.EmailAddresses.Select(e => e.Address), message);
        }

        public async Task SendToWithTemplateAsync(IEnumerable<string> emails, string templateName, string subject, object model)
        {
            if (this._templates.Value == null)
                throw new InvalidOperationException("Please configure the AimSmtpTemplateConfig type.");

            if (!this._templates.Value.EmailTemplates.ContainsKey(templateName))
                throw new IndexOutOfRangeException($"No template called '{templateName}' was found.");

            var message = new MailMessage
            {
                IsBodyHtml = true,
                Subject = subject,
                Priority = MailPriority.Normal,
                BodyEncoding = Encoding.UTF8,
                Body = await this._viewRenderer.RenderToStringAsync(this._templates.Value.EmailTemplates[templateName], model)
            };

            using(message)
                await this.SendToAsync(emails, message);
        }

        public Task SendToWithTemplateAsync(User user, string templateName, string subject, object model)
        {
            return this.SendToWithTemplateAsync(user.Contact.EmailAddresses.Select(e => e.Address), templateName, subject, model);
        }

        private void SetToAndFrom(string email, /*To make it clear this will be mutated*/ref MailMessage message)
        {
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(this._config.Value.Credentials.UserName, FROM_NAME);
        }

        public bool ContainsTemplate(string templateName)
        {
            return this._templates.Value?.EmailTemplates.ContainsKey(templateName) ?? false;
        }

        protected virtual void Dispose(bool disposeAll)
        {
            if(disposeAll && this._smtpInstance != null)
                this._smtpInstance.Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
