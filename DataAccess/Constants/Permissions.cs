using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Constants
{
    public static class Permissions
    {
        public const string ClaimType = "fm_permission";

        public static class User
        {
            public const string ManagePermissions = "user.manage_permissions";
        }

        public static readonly string[] AllPermissions = new[] 
        {
            Permissions.User.ManagePermissions
        };
    }
}
