using DataAccess;
using FarmMaster.Services;
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

    public class AdminUsersViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}
