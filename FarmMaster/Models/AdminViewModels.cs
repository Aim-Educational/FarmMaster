using FarmMaster.Areas.Identity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public class AdminSettingsViewModel
    {
        public EmailSenderConfig Email { get; set; }
    }
}
