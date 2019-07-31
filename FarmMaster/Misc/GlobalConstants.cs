﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Misc
{
    public static class GlobalConstants
    {
        public const int    TermsOfServiceVersion       = 1;
        public const int    PrivacyPolicyVersion        = 1;
        public const int    DefaultPageItemCount        = 25;
        public const string AuthCookieName              = "FarmMasterAuth";
        public const string DefaultNumberName           = "Default";
        public const string FormErrorClasses            = "ui basic red pointing prompt label transition hidden";
        public const string RegexEmail                  = @"^[\w\-\.]+\@(?:[\w\-\.]+)+\.\w+$";
        public const string RegexPhone                  = @"^\+?[0-9]*$";
        public const string RegexTimeSpan               = @"^(\d+d)?\s*(\d+m)?\s*(\d+s)?$";
        public const string IdModalContactActionReason  = "modalContactActionReason";
        public const string IdModalAreYouSure           = "modalAreYouSure";
        public const string IdViewModelWithMessageBox   = "boxViewModelWithMessagePartial";
    }
}
