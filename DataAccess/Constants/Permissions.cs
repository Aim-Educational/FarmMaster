using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Constants
{
    // NOTE: Always use the terms "read" and "write" inside of the values, see how Startup.cs creates policies to see why.
    public static class Permissions
    {
        public const string ClaimType = "fm_permission";

        public static class Other
        {
            public const string DebugUI   = "other.debug.ui";   // User can access debugging pages and special UI elements.
            public const string GraphQLUI = "other.graphql.ui"; // User can access the graphql ui.
            public const string Settings  = "other.settings";   // User can access the settings page.
        }

        public static class User
        {
            public const string Delete           = "user.delete"; // Can delete users without the user's consent.
            public const string Read             = "user.read"; // Can read users in general, but not protected data.
            public const string ReadPermissions  = "user.permissions.read";
            public const string WritePermissions = "user.permissions.write";
            public const string ManageUI         = "user.manage.ui"; // User can access the user manage pages (but not necessarily do anything).
        }

        public static class Contact
        {
            public const string ReadNotes   = "contact.notes.read";
            public const string WriteNotes  = "contact.notes.write";
            public const string ManageUI    = "contact.manage.ui";
            public const string Read        = "contact.read";
            public const string Write       = "contact.write";
            public const string Delete      = "contact.delete";
        }

        public static class Species
        {
            public const string ReadNotes   = "species.notes.read";
            public const string WriteNotes  = "species.notes.write";
            public const string ManageUI    = "species.manage.ui";
            public const string Read        = "species.read";
            public const string Write       = "species.write";
            public const string Delete      = "species.delete";
        }

        public static class Breed
        {
            public const string ReadNotes   = "breed.notes.read";
            public const string WriteNotes  = "breed.notes.write";
            public const string ManageUI    = "breed.manage.ui";
            public const string Read        = "breed.read";
            public const string Write       = "breed.write";
            public const string Delete      = "breed.delete";
        }

        public static readonly string[] AllPermissions = new[] 
        {
            Contact.ReadNotes,
            Contact.WriteNotes,
            Contact.ManageUI,
            Contact.Read,
            Contact.Write,
            Contact.Delete,

            Species.ReadNotes,
            Species.WriteNotes,
            Species.ManageUI,
            Species.Read,
            Species.Write,
            Species.Delete,

            Breed.ReadNotes,
            Breed.WriteNotes,
            Breed.ManageUI,
            Breed.Read,
            Breed.Write,
            Breed.Delete,

            User.ReadPermissions,
            User.WritePermissions,
            User.Read,
            User.ManageUI,
            User.Delete,

            Other.GraphQLUI,
            Other.DebugUI,
            Other.Settings,
        };
    }
}
