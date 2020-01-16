# Code Overview

This section mostly details things such as basic overviews of the code's architecture, code styling, etc.

## Code style

Don't take this as meaning "100% must do this every single time no exceptions", because there are definitely pieces of code
that would look better if they deviate from the styling a little bit. Treat this is a guideline, more than hard laws to follow.

### Styling (C#)

* Classes, enums, structs, public, protected, and internal members should all use PascalCasing.
  * Some exceptions here exist, such as AJAX callbacks making use of underscores due to their naming convention.

* Private variables must be prefixed with an underscore. Private members can be either PascalCase or camelCase (prefer camelCase for variables, and PascalCase for everything else).
  * Private functions can also be prefixed with an underscore, but it's not really used much throughout the code, so it's best to stay consistant with that choice.

* Scoped variables (aka, the ones inside a function body, or parameters) should be camalCase.

* Prefer to use ALL_CAPS_AND_UNDERSCORES for variables representing constant/immutable data. Depends on situation really.

* Use Visual Studio's XML documentation style.

* Curly brackets should be placed on their own lines. In cases of multiple bracket endings, or single-line code that contains brackets, do whatever seems clean to read (aka, leniency).

* Spaces after things like `if ()` or `while()` don't matter, especially since Visual Studio tends to be a bit flip-floppy on how it decides to auto-indent it.

* To the best of your ability, prevent a single line from going off screen due to it being too large. 80 to 120 chars is usually the sweet spot for larger monitors.
  * Documentation and exception messages are excluded from this rule.

* If the body of an `if`, `while`, `foreach`, etc. statement could technically be used without any brackets, only omit the brackets if the body spans only a single line.

```csharp
// Good
if(true)
    SomeSingleLineStatement(param1, param2)

// Bad
if(true)
    SomeMultiLineStatement(
        param1,
        param2
    );

// Good
if(true)
{
    SomeMultiLineStatement(
        param1,
        param2
    );
}
```

* When using LINQ, try to keep each step of the query on a seperate line:

```csharp
// Sometimes it looks fine on one line
var query = animals.Query().FirstOrDefault(a => a.AnimalId == 2);

// Othertimes...
var query = animals.Query().Where(a => a.Gender == Gender.Male).OrderBy(a => a.Name).FirstOrDefault(a => a.AnimalId == 2);

// So do this instead
var query = animals.Query()
                   .Where(a => a.Gender == Gender.Male)
                   .OrderBy(a => a.Name)
                   .FirstOrDefault(a => a.AnimalId == 2);
```

* Be verbose when naming things. Examples from the current code include: `MapLifeEventEntryToAnimal`, `AjaxByIdWithNameValueAsEmailRequest`, and `Animal_ById_Script_ExecuteSingle_AsNameIdImageId`.

## Resources for third party libraries

As we obviously rely on a number of third party libraries, such as EF Core, ASP Core, etc. it'd be wise of me to provide
some key points of interests to research about them.

Please note that FarmMaster, and therefore all of it's libraries, targets .Net Core 2.2

This is especially important when looking for documentation about ASP Core and EF Core, due to the breaking changes between their 2.2 and 3.x versions.

### ASP Core

* [Learn about ASP's MVC (Model View Controller) pattern](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-2.2&tabs=visual-studio)

* [The entire fundamentals section](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-2.2&tabs=windows)
  * If you had to pick one, please learn about how ASP handles Dependency Injection.

* [A more technical section about MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-2.2)

* [Razor (.cshtml) syntax](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-2.2) and [Layout files](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/layout?view=aspnetcore-2.2)

* [Learn about authorization in ASP](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-2.2)
  * FarmMaster has its own version of the `[Authorize]` attribute called `[FarmAuthorise]` which should be used instead, as it integrates better
    with our role system.

* [Model binding. a.k.a how ASP can automatically convert JSON to a class, and vice-versa](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-2.2)

### EF Core

* [Overview of EF Core](https://docs.microsoft.com/en-us/ef/core/)

* [Creating a model](https://docs.microsoft.com/en-us/ef/core/modeling/)

* [Client vs Server evaluation](https://docs.microsoft.com/en-us/ef/core/querying/client-eval)

* [Loading related data (e.g. `.Include`)](https://docs.microsoft.com/en-us/ef/core/querying/related-data)

* [Learn about migrations](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/migrations?view=aspnetcore-2.2)
  * Note that all `dotnet ef` style commands should be performed in the `./Business/` folder.

### GraphQL

* [Overview of GraphQL](https://graphql.org/learn/)
  * Most of that website is pretty useful for learning GraphQL as a concept/language.

* Any instance of FarmMaster will have the `/graphiql` endpoint available, which will let you play around with FarmMaster's GraphQL schema.

* While a bit unweidly, [giving the GraphQL library a look](https://graphql-dotnet.github.io/docs/getting-started/introduction) can't do much harm.
  * There will be a dedicated section for how to modify FarmMaster's GraphQL setup, so don't worry too much if this goes over your head at first.
