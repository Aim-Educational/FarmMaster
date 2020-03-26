using EmailSender;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FarmMaster.Constants
{
    public class Emails
    {
        public const string ConfirmPassword = "confirm_email.html";
    }

    public static class EmailExtensions
    {
        static EmailTemplate _confirmPassword;
        static EmailTemplateValues _values; // So we don't recreate it every single time.

        public static void LoadTemplates(IWebHostEnvironment env)
        {
            Func<string, string> templatePath = file => Path.Combine(env.WebRootPath, "email", file);

            _confirmPassword = EmailTemplate.FromFile(templatePath(Emails.ConfirmPassword));
            _values = new EmailTemplateValues();
        }

        public static Task<EmailResult> SendConfirmPasswordAsync(this ITemplatedEmailSender email, string toAddress, string callbackUrl)
        {
            _values.Clear();
            _values["url"] = HtmlEncoder.Default.Encode(callbackUrl);

            return email.SendTemplatedEmailAsync(
                toAddress,
                "Please confirm your email",
                _confirmPassword,
                _values
            );
        }
    }
}
