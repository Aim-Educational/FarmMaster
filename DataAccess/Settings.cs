using System.ComponentModel.DataAnnotations;

namespace DataAccess
{
    public class Settings
    {
        [Key]
        public int SettingsKey { get; set; }

        public string SmtpServer { get; set; }
        public ushort SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }
}
