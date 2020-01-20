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

### Semantic/Fomantic UI

This is mostly if you're going to be writing any HTML, since this is the main CSS framework behind FarmMaster's frontend.

Honestly, just go to [this](https://fomantic-ui.com/elements/button.html) page, and every so often give something in the huge list of things
on the left-hand side a read through, so you can slowly learn how it's used.

If nothing else, learn its [grid system](https://fomantic-ui.com/collections/grid.html), as that's really useful to know.

While counter productive, large portions of the website actually use custom CSS (there's a dedicated section explaining all of this), so
while learning Fomantic UI is great, it'd probably be best not to spend too much time on it, and instead look towards vanilla CSS and SASS.

### SASS

Again, this'll be part of the dedicated section to CSS and friends, but start learning [here](https://sass-lang.com/guide) and [here](https://sass-lang.com/documentation).

Please note when creating custom SASS that you should use (and [learn](https://css-tricks.com/snippets/css/a-guide-to-flexbox/)) flexbox, which is just
a better way of laying out a page in general.

## CI/CD pipeline

FarmMaster's CI is pretty simplistic.

First of all, read up on what [CI/CD](https://docs.gitlab.com/ee/ci/) is if you're unaware of it. FarmMaster's CI file may [prove interesting](https://github.com/Aim-Educational/FarmMaster/blob/master/.gitlab-ci.yml) as a learning resource as well.

As stated in the CI file, there are currently two jobs: `test`, and `publish_and_package`.

The `test` job is ran on every commit, and simply runs any unittests FarmMaster has (which definitely needs to be improved...).

The `publish_and_package` job is only ran everytime a git tag is created (e.g. `git tag v0.2.0-alpha`). This job will download the latest version
of [AimCLI](https://github.com/Aim-Educational/AimCLITool) (which also makes use of CI/CD), perform the `dotnet publish` command which creates a production
ready package of FarmMaster, and then finally uses `AimCLI` to zip up the distribution of FarmMaster alongside any other metadata, which is then stored
as an 'artifact' on Gitlab's side, allowing this zipped up package to be downloaded.

In otherwords, `publish_and_package` simply builds and packages the latest distribution of FarmMaster.

After that, someone who has access to the server FarmMaster is hosted on (eventually this will become an automated process) will have
to SSH into the server to trigger `AimCLI` to deploy the latest distribution.

While this may all sound like over-kill, this actually streamlines the process to the point of little friction (and once I bother to
automate the final half of the process, there will be no friction - barring bugs!)

## Project Files

Different parts of FarmMaster are split into seperate projects (.csproj), yet are all under the same solution to allow ease-of-use.

### Business

This project contains the EF Core models, DbContext, and EF Core migrations. In other words - the core business objects of FarmMaster.

It has little to non-existant business logic however, as that's handled in the `FarmMaster` project (arguably in error, but whatever at this point).

One file of note is the [BusinessConstants](xref:Business.Model.BusinessConstants) file, which contains a bunch of constant data such as the internal
names of permissions and life events. You'll likely be visting it a lot if you're messing with the aforementioned parts of FarmMaster.

### GroupScript & GroupScriptTests

This contains the parser, AST, and compiler for GroupScript.

I couldn't really find a way to fit it into FarmMaster directly, so thought it'd be more suitable to make it its own project.

There is also another project called `GroupScriptTests` that contains all of the unittests for the project.

### FarmMaster

The main project, this is where the majority of FarmMaster is implemented (using ASP Core, MVC, Razor, GraphQL, etc.).

I'll go over the purpose of *every* folder for this project:

* wwwroot - This is where any files that should be accessible to the web (or internal parts of FarmMaster) need to be placed.

* BackgroundServices - This is where any background tasks for FarmMaster live, such as the service that pushes request metrics to the database.

* Controllers - A standard folder in ASP's MVC scheme. This is where all of the controllers are stored.

* Filters - This is where any custom filters (things like `[FarmAuthorise]`) live.

* GraphQL - This is where everything to do with GraphQL lives.

* Middleware - This is where any custom middleware, such as the authentication middleware, are contained.

* Misc - Anything that needs to exist, but doesn't have any concrete folder to live in gets placed here.

* Models - This is where any ViewModels (either for MVC, razor views, or AJAX requests) are placed. Each file tends to contain multiple classes, grouped under the same area of code/data (one file for Animal view models, another for User view models, etc.)

* Scripts - Any custom, reusuable Javascript libraries FarmMaster creates are actually written in Typescript first, and this is where the Typescript files live.

* Services - This is where any custom services (another ASP concept) live. It is mostly filled up of `IServiceXXXManager` services, which are
services that implement the repository pattern as a way of accessing/modifying the database.

* Styles - This is where all SASS files live. Files that are meant to be imported by other files should be prefixed with an underscore.

* Views - This is where all of the views (M **V** C) are stored. Partial views are prefixed with an underscore. Each controller has their own sub-folder.

That should be enough to get you started. As you get used to the codebase the organisation should hopefully make more sense.
