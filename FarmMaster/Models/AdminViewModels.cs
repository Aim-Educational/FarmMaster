using DataAccess;
using EmailSender;
using FarmMaster.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class AdminSettingsViewModel
    {
        public string EmailError { get; set; }
        public EmailSenderConfig Email { get; set; }
    }
}
