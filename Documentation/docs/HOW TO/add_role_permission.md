# Add a new Role permission

* Open the project in Visual Studio

* Under the 'Business' project, open `Model/EnumRolePermission.cs`

* Locate the `EnumRolePermission.Names` static class

* Add in a new entry similar to the existing entries. Try to keep the naming consistant, for example if you're adding
  in a new permission to **access an admin panel** then you should name the variable `ACCESS_ADMIN_PANEL`. The value
  you give this variable simply has to be unique, naming doesn't matter.

* Open `Model/FarmMasterContext.cs`

* Find the `Data seeding` region, and inside that region will be the `SeedRolePermissions` function.

* The existing seeded values should kind of document itself on how you can seed your new permisison in. **Ensure that** `EnumRolePermissionId` is unique.

* Goto the project's root. Then go into the 'Business' folder, and open a command or powershell prompt in that folder.

* Use the `dotnet ef migrations add` command to create a new migration, e.g. `dotnet ef migrations add AccessAdminPanelPermission`

* You will be prompted to enter a connection string. This is the value used as the `ConnectionString:Migrate` config option for the website.
  The location of this value will differ depending on your environment, but `/FarmMaster/FarmMaster/appsettings.development.json` is usually a good place to put it for local development.

After you follow all of these steps, the permission will be able to be used like all existing permissions.

There is no need to modify any of the pages that let the user create or modify roles, as these pages automatically generate the permission selector
from the database.
