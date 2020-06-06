using DataAccess;
using EmailSender;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminModule.Models
{
    public class AdminSettingsViewModel
    {
        public string EmailError { get; set; }
        public EmailSenderConfig Email { get; set; }
    }
}
