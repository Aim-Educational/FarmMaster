﻿using DataAccess;
using EmailSender;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;

namespace FarmMaster.Services.Configuration
{
    public class ConfigureEmailOptions : ConfigWithDatabaseService<EmailSenderConfig>
    {
        readonly string _layoutTemplate;

        public ConfigureEmailOptions(IServiceScopeFactory factory, IWebHostEnvironment env) : base(factory)
        {
            var path = Path.Combine(env.WebRootPath, "email", "layout.html");
            this._layoutTemplate = File.ReadAllText(path);
        }

        protected override void ConfigWithDatabase(EmailSenderConfig options, FarmMasterContext db)
        {
            var settings = db.Settings.FirstOrDefault();
            if (settings == null)
                return;

            options.Layout = new EmailTemplate(this._layoutTemplate);
            options.Smtp.Server = settings.SmtpServer;
            options.Smtp.Port = settings.SmtpPort;
            options.Smtp.Username = settings.SmtpUsername;
            options.Smtp.Password = settings.SmtpPassword; // CURRENTLY STORED UNENCRYPTED
        }
    }
}
