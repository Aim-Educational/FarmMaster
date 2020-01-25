# IServiceEntityManager and the repository pattern

This section covers how FarmMaster uses the [repository pattern](https://deviq.com/repository-pattern/) to handle the abstraction
of accessing data from the database.

## What is the repository pattern

In short, it is an abstraction that combines data access with business logic.

So instead of the website being littered with code that has direct knowledge and access to the [DbContext](xref:Business.Model.FarmMasterContext)
we instead create a new `repository` interface for *every entity* that handles any functionality the website needs.

## IServiceEntityManager

The [IServiceEntityManager](xref:FarmMaster.Services.IServiceEntityManager) interface is the basis of this pattern. In FarmMaster a `repository`
is called an `entity manager` since the naming makes more sense this way.

`IServiceEntityManager` describes a few functions that any inheriting service that intends to act as an entity manager needs to implement:

* GetIdFor - Deprecated, throw an exception in any new service you make.

* Query - Instead of creating a seperate function for each different query, we instead give the user code a bit more control by providing this function which returns
  an `IQueryable`, so the user code can leverage LINQ to create fluent queries on the underlying data store (which is almost always the database).

* QueryAllIncluded - Deprecated, throw an exception in any new service you make.

* Update - Sometimes the user code needs to make a small edit or tweak to an entity's data, and it'd be overkill to make a new function for the specific modification.
  So, this function allows the user to pass an entity to the service, which will then save any changes made to the entity.

Furthermore, some old code you see may make use of two [extention functions](xref:FarmMaster.Services.IServiceEntityDataExtentions) called `FromId` and
`FromIdAllIncluded`. I wrote these when I was still early in my learning of EF Core, and their major downside is that *they pull the entire data set for every entity
of a certain type from the database* due to it forcing a client-side evaluation super early on. **Never use them**, they will be removed at some point.

### Dependency injection

Because FarmMaster uses entity managers as [services](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1),
it means that they can be accessed via ASP's dependency injection capabilities.

So if you're making a new MVC action, or a new service/middleware/whatever that can use dependency injection, then getting access to these entity managers is
as easy as accessing any other service.

### Making a new entity manager

Making a new entity manager is simpler than it may seem - most of the complexity comes from the actual implementation code of the manager.

1. Create a new file inside of `./FarmMaster/Services` with the naming scheme of `IServiceXXXManager.cs` where "XXX" is the name of the entity you want to manage
   (e.g. [IServiceUserManager](xref:FarmMaster.Services.IServiceUserManager) for users).

2. Inside this file, create a new interface (call it the same thing as the file).

3. Make this interface inherit from [IServiceEntityManager](xref:FarmMaster.Services.IServiceEntityManager), passing the type of the entity you want to manage as
   the type parameter: `interface IServiceUserManager : IServiceEntityManager<User>`

4. You may wish to define any other functions this interface may need, such as a [Create()](xref:FarmMaster.Services.IServiceUserManager#Create) function.

5. Inside the same file, create a new class. You can call it whatever, but the general scheme is to use the same name as the interface but without the "I" at the start.

6. This new class will be the default implementation of your entity manager, so naturally you need to make sure it also inherits your newly made interface.

7. In Visual Studio you should have a red squiggly line underneath where you inherit your interface, hover over it and press "Implement interface" from the dropdown.
   For other IDEs/text editors you'll have to do things manually.

8. For the deprecated functions mentioned earlier, just make sure they're throwing exceptions.

9. For the none deprecated functions, you may wish to examine the code from the existing entity managers to see how to implement them.

10. Note that you'll likely need to use dependency injection to gain access to `FarmMasterContext`, again, see the existing services to learn how to do that.

11. Once you're happy with your default implementation, open the startup file (`./FarmMaster/Startup.cs`); locate the `ConfigureServices` function; locate the comment
    saying `// Data Managers`; add a line in that section for your newly made service, you should be able to figure out based on the existing lines of code.

After the steps above are done, you can now access your newly made service via dependency injection.

> [!NOTE]
> Instead of creating your own functions for deleting an entity, please see below about a standard interface.

## IServiceEntityManagerFullDeletion and IServiceEntityManagerAnonymousDeletion

There are essentially two types of deletion that FarmMaster cares about: Completely removing the data from the database (full deletion), and
simply hiding the data while possibly also dummying out some of the values (anonymous deletion).

FarmMaster provides two interfaces, similar to `IServiceEntityManager`, that provide a standard way of defining whether your entity manager
performs a full delete, or an anonymous delete.

So please inherit [IServiceEntityManagerFullDeletion](xref:FarmMaster.Services.IServiceEntityManagerFullDeletion) for full deletion, and
[IServiceEntityManagerAnonymousDeletion](xref:FarmMaster.Services.IServiceEntityManagerAnonymousDeletion) for anonymous deletion.

## GDPR support & IServiceGdprData

Some entity managers will manage entities that contain references to a [User](xref:Business.Model.User) and/or [Contact](xref:Business.Model.Contact).

These managers will need to provide support for GDPR information and deletion requests.

A few things to note beforehand though:

* Contacts can be external entities or people that have been manually registered into the system. They are not users.

* Users are people who have used the sign up page to gain access to the system.

* **Users are also given their own Contact as part of the creation process**

* The above means that you must also handle a user's `Contact`, as well as the `User` itself within your GDPR functions. **The user's contact isn't automatically
  passed to the contact versions of the GDPR functions, do that yourself.**

* Deletion requests for users and contacts are *always* anonymous deletions. If you keep any mapping entries between a user and something else, you are safe to
  fully delete them. If you have an entity that simply refers to a user directly (a.k.a as an Owner of something) then you can simply leave it be, as the name
  of the user will be anonymised by another service.

* Your service only needs to remove/anonymise data about a contact or user that's *relevent to the entity you're managing*. For example, the manager for group script
  isn't responsible for removing a contact's emails, but the manager for contacts *is* responsible for that.

Now, adding GDPR support into your entity manager is very simple (again, complexity is from the implementation code): all you have to do
is inherit and implement the [IServiceGdprData](xref:FarmMaster.Services.IServiceGdprData) interface.

For the [AnonymiseContactData](xref:FarmMaster.Services.IServiceGdprData#AnonymiseContactData) and
[AnonymiseUserData](xref:FarmMaster.Services.IServiceGdprData#AnonymiseUserData) functions, simply do the anonymisation/deletion process described above. Look
at services such as `IServiceContactManager` and `IServiceUserManager` if you need references for existing GDPR code.

For the [GetContactGdprData](xref:FarmMaster.Services.IServiceGdprData#GetContactGdprData) and
[GetUserGdprData](xref:FarmMaster.Services.IServiceGdprData#GetUserGdprData) you will be passed a `JObject` as the second parameter, which you need to populate
with any data relevent to the given contact/user. Again, see other services to see how this code is implemented.

## Entity Managers that manage multiple entities

There are cases where an entity manager may inherit from `IServiceEntityManager` multiple times (for example, [IServiceSpeciesBreedManager](xref:FarmMaster.Services.IServiceSpeciesBreedManager)).

This is fine and all since sometimes entities are so tightly related that a single manager for both makes sense, but there's an issue in accessing
the functions that `IServiceEntityManager` exposes: calls to them are ambiguous, so it creates a compiler error.

Fortunately FarmMaster provides an [identity function](xref:FarmMaster.Services.IServiceUserManager#For) that can be used to lower a manager's
type down into a specific `IServiceEntityManager`.

For example, with `IServiceSpeciesBreedManager` if we wanted to access the `IServiceEntityManager<Breed>` variant of its functions, one could do:
`serviceSpeciesBreed.For<Breed>().Query()...`

If you wanted species then: `serviceSpeciesBreed.For<Species>().Query()`, etc.
