using DataAccess;
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

            options.Server   = settings.SmtpServer;
            options.Port     = settings.SmtpPort;
            options.Username = settings.SmtpUsername;
            options.Password = settings.SmtpPassword; // CURRENTLY STORED UNENCRYPTED
        }
    }
}
