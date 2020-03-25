using DataAccess;
using EmailSender;
using FarmMaster.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Services.Configuration
{
    public class ConfigureEmailOptions : ConfigWithDatabaseService<EmailSenderConfig>
    {
        public ConfigureEmailOptions(IServiceScopeFactory factory) : base(factory)
        {}

        protected override void ConfigWithDatabase(EmailSenderConfig options, FarmMasterContext db)
        {
            var settings = db.Settings.FirstOrDefault();
            if (settings == null)
                return;

            options.Url = "localhost"; // TODO: Don't hardcode
            options.Layout = new EmailTemplate("{{ body }}"); // TODO: Load from file

            options.Smtp.Server   = settings.SmtpServer;
            options.Smtp.Port     = settings.SmtpPort;
            options.Smtp.Username = settings.SmtpUsername;
            options.Smtp.Password = settings.SmtpPassword; // CURRENTLY STORED UNENCRYPTED
        }
    }
}
