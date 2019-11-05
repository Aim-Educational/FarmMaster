using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Model
{
    public static class BusinessConstants
    {
        public static class HoldingRegistrations
        {
            public const string COW = "cow";
            public const string PIG = "pig";
            public const string SHEEP_AND_GOAT = "sheep_goats";
            public const string FISH = "fish";
            public const string POULTRY = "poultry";
        }

        public static class Permissions
        {
            public const string VIEW_CONTACTS           = "view_contacts";
            public const string EDIT_CONTACTS           = "edit_contacts";
            public const string CREATE_CONTACTS         = "create_contacts";
            public const string DELETE_CONTACTS         = "delete_contacts";

            public const string VIEW_ROLES              = "view_roles";
            public const string EDIT_ROLES              = "edit_roles";
            public const string CREATE_ROLES            = "create_roles";
            public const string DELETE_ROLES            = "delete_roles";
            public const string ASSIGN_ROLES            = "assign_roles";

            public const string VIEW_HOLDINGS           = "view_holdings";
            public const string EDIT_HOLDINGS           = "edit_holdings";
            public const string CREATE_HOLDINGS         = "create_holdings";
            public const string DELETE_HOLDINGS         = "delete_holdings";

            public const string VIEW_SPECIES_BREEDS     = "view_species_breeds";
            public const string EDIT_SPECIES_BREEDS     = "edit_species_breeds";
            public const string CREATE_SPECIES_BREEDS   = "create_species_breeds";
            public const string DELETE_SPECIES_BREEDS   = "delete_species_breeds";

            public const string VIEW_LIFE_EVENTS        = "view_life_events";
            public const string EDIT_LIFE_EVENTS        = "edit_life_events";
            public const string CREATE_LIFE_EVENTS      = "create_life_events";
            public const string DELETE_LIFE_EVENTS      = "delete_life_events";

            public const string VIEW_ANIMALS            = "view_animals";
            public const string EDIT_ANIMALS            = "edit_animals";
            public const string CREATE_ANIMALS          = "create_animals";
            public const string DELETE_ANIMALS          = "delete_animals";

            public const string USE_LIFE_EVENT_ENTRY    = "use_life_event_entry";
        }

        public static class BuiltinLifeEvents
        {
            public const string BORN = "Born";
            public const string ARCHIVED = "Archived";
        }

        public static class BuiltinLifeEventFields
        {
            #region BORN
            public const string BORN_DATE = "Date";
            #endregion
        }
    }
}
