using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender
{
    /// <summary>
    /// The configuration for an <see cref="ITemplatedEmailSender"/>
    /// </summary>
    public class EmailSenderConfig
    {
        /// <summary>
        /// The URL of *THIS* website. This is used when generating links for users to click on.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Used as the main layout template. Value 'body' is reserved for the email body.
        /// </summary>
        public EmailTemplate Layout { get; set; }

        /// <summary>
        /// SMTP settings.
        /// </summary>
        public EmailSenderSmtpConfig Smtp { get; set; }

        public EmailSenderConfig()
        {
            this.Smtp = new EmailSenderSmtpConfig();
        }
    }

    /// <summary>
    /// The SMTP settings.
    /// </summary>
    public class EmailSenderSmtpConfig
    {
        /// <summary>
        /// The SMTP server to connect to.
        /// </summary>
        public string Server { get; set; }
        
        /// <summary>
        /// The port to connect to.
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// The username used to *sign into* the SMTP server.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password used to sign into the SMTP server.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Sometimes the Username may not be an email address, so this field exists for such cases.
        /// 
        /// If this field is null, then Username is used as the email address as well.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// A custom name that the user sees when they recieve an email.
        /// 
        /// If this field is null, then Username is used as the alias.
        /// </summary>
        public string Alias { get; set; }
    }

    /// <summary>
    /// Describes the result of an email operation.
    /// </summary>
    public class EmailResult
    {
        public bool Succeeded { get; internal set; }
        public string Error { get; internal set; }
    }

    /// <summary>
    /// An interface used to send emails.
    /// </summary>
    public interface ITemplatedEmailSender : IEmailSender
    {
        /// <summary>
        /// Sends an email, where its contents are provided by an <see cref="EmailTemplate"/>.
        /// </summary>
        /// <param name="toAddress">The email address to send the email to.</param>
        /// <param name="template">The template containing the email's contents</param>
        /// <param name="values">The values used to resolve the given <paramref name="template"/></param>
        /// <returns>The results of trying to send the email.</returns>
        Task<EmailResult> SendTemplatedEmailAsync(string toAddress, EmailTemplate template, EmailTemplateValues values);

        /// <summary>
        /// Allows the email sender to reload its client settings, based on the updated config provided.
        /// </summary>
        /// <param name="config">The updated config to use.</param>
        /// <returns>Async</returns>
        Task ReloadAsync(EmailSenderConfig config);
    }
}
