using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Constants
{
    // NOTE: Always use the terms "read" and "write" inside of the values, see how Startup.cs creates policies to see why.
    public static class Permissions
    {
        public const string ClaimType = "fm_permission";

        public static class User
        {
            public const string ReadPermissions  = "user.read_permissions";
            public const string WritePermissions = "user.write_permissions";
        }

        public static readonly string[] AllPermissions = new[] 
        {
            User.ReadPermissions,
            User.WritePermissions
        };
    }
}
