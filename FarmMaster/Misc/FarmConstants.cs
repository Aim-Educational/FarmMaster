using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Misc
{
    public static class FarmConstants
    {
        public static class Versions
        {
            public const int TermsOfService = 1;
            public const int PrivacyPolicy  = 1;
        }

        public static class CookieNames
        {
            public const string AuthCookie = "FarmMasterAuth";
        }

        public static class Regexes
        {
            public const string Email    = @"^[\w\-\.]+\@(?:[\w\-\.]+)+\.\w+$";
            public const string Phone    = @"^\+?[0-9]*$";
            public const string TimeSpan = @"^(\d+d)?\s*(\d+m)?\s*(\d+s)?$";
        }

        public static class EmailTemplateNames
        {
            public const string EmailVerify             = "verify_email";
            public const string ContactInfoAudit        = "user_info_audit_alert";
            public const string AnonymisationRequest    = "anon_request";
        }
    }

    // THESE WILL BE MOVED/REMOVED SOON
    public static class GlobalConstants
    {
        public const int    DefaultPageItemCount        = 25;
        public const string FormErrorClasses            = "ui basic red pointing prompt label transition hidden";
        public const string IdModalContactActionReason  = "modalContactActionReason";
        public const string IdModalAreYouSure           = "modalAreYouSure";
        public const string IdViewModelWithMessageBox   = "boxViewModelWithMessagePartial";
    }
}
