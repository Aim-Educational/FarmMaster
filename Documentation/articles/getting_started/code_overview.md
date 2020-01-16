# Code Overview

This section mostly details things such as basic overviews of the code's architecture, code styling, etc.

## Code style (C#)

Don't take this as meaning "100% must do this every single time no exceptions", because there are definitely pieces of code
that would look better if they deviate from the styling a little bit. Treat this is a guideline, more than hard laws to follow.

### Styling

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
