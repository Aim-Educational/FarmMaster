using EmailSender;

namespace AdminModule.Models
{
    public class AdminSettingsViewModel
    {
        public string EmailError { get; set; }
        public EmailSenderConfig Email { get; set; }
    }
}
