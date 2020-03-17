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
            public const string Read             = "user.read"; // Can read users in general, but not protected data.
            public const string ReadPermissions  = "user.permissions.read";
            public const string WritePermissions = "user.permissions.write";
        }

        public static readonly string[] AllPermissions = new[] 
        {
            User.ReadPermissions,
            User.WritePermissions,
            User.Read
        };
    }
}
