using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailSender
{
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

    public class EmailSenderSmtpConfig
    {
        public string Server { get; set; }
        public ushort Port { get; set; }
        public string Username { get; set; }
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

    public class EmailResult
    {
        public bool Succeeded { get; internal set; }
        public string Error { get; internal set; }
    }

    public interface ITemplatedEmailSender : IEmailSender
    {
        Task<EmailResult> SendTemplatedEmailAsync(string toAddress, EmailTemplate template, EmailTemplateValues values);

        Task ReloadAsync(EmailSenderConfig config);
    }
}
