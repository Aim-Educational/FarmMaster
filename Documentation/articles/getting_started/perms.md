# Permissions

This section will go over how roles and permissions are structured, and how to add/use them inside of the codebase.

## Model

First, there are permissions. Permissions in the database model are "Enum"-type entities: They are immutable to both user and server, and they
are basically just a database version of a normal `enum`, except they can store more info and can be used more directly inside of EF than normal
enums can.

The function of permissions is to allow users to assign them to roles. Roles are essentially just a bundle of permissions that can
be used to group users together. These permissions can then be read by the server to lock out certain endpoints unless the user
accessing them has a certain permission(s) in their role.

The admin role (referred to as the 'God role' internally) contains no permissions, yet is specially handled to bypass every
permission check.

Now, there are three main entity classes for the role-permission system:

* [Role](xref:Business.Model.Role) - Contains information about a role, such as their name and hierarchy index.

* [EnumRolePermission](xref:Business.Model.EnumRolePermission) - Contains information about a permission.

* [MapRolePermissionToRole](xref:Business.Model.MapRolePermissionToRole) - A mapping entry that maps an `EnumRolePermission` to a `Role`.

I feel this setup is self-explanatory, so I have no need to comment further on the model.

## Creating a new permission

When adding new functionality onto FarmMaster, you will likely need to create new permissions for users to be able to control
who can access the new functionality.

1. The first thing you need to do is to visit the [BusinessConstants](xref:Business.Model.BusinessConstants) file, and locate the
   [BusinessConstants.Permissions](xref:Business.Model.BusinessConstants.Permissions) class.

2. Once you have located this class you need to add in a new `public const string`, similar to the ones that already exist. Please
   try to keep the naming in both the variable name, and its value, consistant with the already existing ones.

3. Next, open the file for [FarmMasterContext](xref:Business.Model.FarmMasterContext). There should be a `#region` called "Data Seeding", and
   within that region is a function called [SeedRolePermissions](xref:Business.Model.FarmMasterContext.SeedRolePermissions).

4. Within this function is an array called `roles`, which contains a bunch of tuple items describing a role. From here you can examine
   the existing entries to figure out how to add your new permission. *Please see the "IMPORTANT" notice below*.

5. Finally, open a command prompt/terminal inside of "./Business/" (Where `Business.csproj` is located), and use the [dotnet ef migrations add](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli#create-a-migration)
   command to create a new EF migration for your permission.

> [!IMPORTANT]
> **Always add new permissions to the bottom of the array.**
>
> The ids for the resulting `EnumRolePermission` entries are calculated based on the order of the `roles` array, so never. Ever. change
> the order of any item in the array.
>
> Please double check your migration to ensure that there are no `UpdateColumn` function calls, as that is a likely indicator that you've messed up the order somewhere.

After following the above steps, your new permission is good to go.

> [!NOTE]
> FarmMaster's front end will automatically add a checkbox for your new permission onto the "Role Edit" page.
>
> The category that it gets put under depends on the first word of your permission's name. e.g. "Add Animal" is listed under "Add".

## Using permissions

There are two main ways to use permissions right now: the [FarmAuthorise](xref:FarmMaster.Filters.FarmAuthorise) filter, or the
[IServiceRoleManager](xref:FarmMaster.Services.IServiceRoleManager) service.

### The [FarmAuthorise] filter

If you're not aware of what a "filter" is in ASP-land, please read about it [here](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-3.1).
FarmAuthorise is an [authorisation filter](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-3.1#authorization-filters).

The `FarmAuthorise` filter can be passed two arrays, one called `PermsOR` and another called `PermsAND` - both can be used together.

Any permissions (permissions are passed via their internal names, a.k.a the values in `BusinessConstants.Permissions`) passed to `PermsOR` means
"this user can only access this endpoint if they have at least one of this permissions".

Any permissions passed to `PermsAND` means "this user can only access this endpoint if they have every one of these permissions".

This is very similar the ASP's built-in `[Authorise]` filter, but `[FarmAuthorise]` integrates better with our role system.

If you need to lock an endpoint behind the user simply being logged in, and you don't care about their permissions, then just
don't pass anything to the filter's constructor.

Here's an example of its usage, where the user needs the `CREATE_ANIMALS` permission to access the endpoint:

```csharp
[FarmAuthorise(PermsAND: new[] { BusinessConstants.Permissions.CREATE_ANIMALS })]
public IActionResult Create()
{
    return View("CreateEdit", new AnimalCreateEditViewModel{ IsCreate = true });
}
```

### The IServiceRoleManager service

There are times where code that doesn't have access to filters, or code that needs finer introspection of a user's permissions, need a way
to access information about the user's role and permissions.

Virtually every part of ASP Core (and FarmMaster) supports ASP's [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1), which can be used to access the `IServiceRoleManager` service.

Having a look at its [documentation](xref:FarmMaster.Services.IServiceRoleManager) should give you a general idea of what you can do with it, such
as determining if a specific role has a certain permission.

The [Role](xref:Business.Model.Role) class itself has an extention method called [CanModify](xref:Business.Model.RoleExtentions.CanModify) which can be used to determine
if users of role `A` can modify the permissions of role `B`.

## Deleting permissions

Deleting a permission is the exact opposite process of creating one:

1. Delete the entry in `FarmMasterContext's` data seeding region.

2. Delete the entry in `BussinessConstants.Permissions`.

3. Create a new migration to commit the change.

> [!IMPORTANT]
> **Only delete permissions if there are no permissions under it inside of the data seeding array**
>
> Again, this is because it'll mess up the ids.
>
> Set both values in the tuple to `null`, and the data seeder will skip over it while incrementing the id.
> Sure it might still exist in the database, but it won't be used anywhere.
.
> [!NOTE]
> Permissions use cascading delete, so deleting a permission from the database will also delete all mapping entries for that permission.
