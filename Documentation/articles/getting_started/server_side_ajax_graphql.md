# Server-side AJAX and GraphQL

While the previous described the client-side portion of AJAX and GraphQL, this section will go over the server-side portion.

## AJAX

As a quick reminder from the previous section, there are two types of AJAX functions - those that return a value, and those that don't.

When creating an AJAX function you will have to pick between using either [FarmAjaxReturnsMessage](xref:FarmMaster.Filters.FarmAjaxReturnsMessage) and
[FarmAjaxReturnsMessageAndValue](xref:FarmMaster.Filters.FarmAjaxReturnsMessageAndValue).

These special filters provider certain behaviours to the action that they're attached to:

* It will automatically verify that the model state is valid, and return an error message if there's an issue.

* Any uncaught exceptions will be caught and returned as an error message to the client.

* It requires that the action's model either be or inherit from [AjaxRequestModel](xref:FarmMaster.Models.AjaxRequestModel).

* It can automatically retrieve the user using the action via reading the `SessionToken` from the action's model.

* If you add a [User](xref:Business.Model.User) as a parameter, then the filter will automatically set its value to the user using the action.

* It can ensure that the user using the AJAX action has certain permissions, similar to [FarmAuthorise](xref:FarmMaster.Filters.FarmAuthorise).

Additionally, actions that return a value must wrap the value inside of the [AjaxValueResult](xref:FarmMaster.Filters.AjaxValueResult) class,
while actions that don't return a value must return a `new EmptyResult()`. It kind of goes without saying, but any values returned by AJAX will
be automatically converted to JSON, so the data you return must be structured the same way you want to access it in javascript.

These filters remove *substantial* amounts of boilerplate code that used to exist prior to their creation, so while they do a lot, and might get in the way
sometimes, appreciate their value in the general case.

### Creating a new AJAX action

The first thing you want to do is open the file for [FarmMaster.Controllers.AjaxController](xref:FarmMaster.Controllers.AjaxController) as that is where
**all** AJAX actions are placed.

Next, please observe the way the AJAX actions are named, as well as the `#region`s spread throughout the file.

For our example action we'll make it so that it takes an ID for an animal, and then returns the name, tag, and id of that animal. Normally you'd want to use
GraphQL for this, but this is just an example action.

I'll show you the code then go over it:

```c#
#region Animal
[HttpPost]
[FarmAjaxReturnsMessageAndValue(BusinessConstants.Permissions.VIEW_ANIMALS)]
public IActionResult Animal_ById_AsNameTagId(
    [FromBody] AjaxByIdRequest model,
    User _,
    [FromServices] IServiceAnimalManager animals
)
{
    var animal = animals.Query().FirstOrDefault(a => a.AnimalId == model.Id);
    if (animal == null)
        throw new IndexOutOfRangeException($"No animal with ID #{model.Id}");

    return new AjaxValueResult(new
        {
            name = animal.Name,
            tag = animal.Tag,
            id = animal.AnimalId
        }
    );
}
#endregion
```

* First off, notice that we placed the function inside of an `Animal` region (if a region doesn't exist, just create a new one) as that's where it fits best.

* The action is marked as `[HttpPost]` as AJAX requests are typically done with HTTP POST.

* We return a value for this action, so we use `[FarmAjaxReturnsMessageAndValue]` while also specifying that the user needs the `VIEW_ANIMALS` permission.

* The action is called `Animal_ById_AsNameTagId`. Placing `ById` after a data type is a common convention to specify that an id needs to be passed. Using `AsXXX` (`AsNameTagId`)
  is a common convention for describing what data is returned. And of course we prefix it will `Animal` since that's what the action works on.

* Our model is marked `[FromBody]` as that tells ASP where to look for the model's data in the HTTP request (the body).

* Our model is of the type `AjaxByIdRequest`. There are *many* pre-made AJAX request models, but this specific one simply contains an `Id` field. It's better to reuse
  the pre-made ones than it is to create a new one. Create a new class (in `./FarmMaster/Models/AjaxModels.cs`) for actions that need specialised data inputs.

* We specify a `User` parameter just to show how to access it. The AJAX filter will automatically set it to the user who's using the AJAX action.

* The final parameter is marked `[FromServices]` which tells ASP to use its dependency injection to get the value of that parameter. So in this case, we're injecting
  the [IServiceAnimalManager](xref:FarmMaster.Services.IServiceAnimalManager) as we need that to query for animals.

* The first three lines of the function are simply getting an animal by the id from the AJAX model, and throwing an exception (returned as an error message) if the animal
  doesn't exist.

* The `return` statement is wrapping an anonymous class containing the return data around the `AjaxValueResult` class, which is a hard requirement of
  `[FarmAjaxReturnsMessageAndValue]`.

After all that hard work, the following JSON is generated:

```json
{
    "message": "Success!",
    "messageType": 1,   // FarmAjaxMessageType.Information
    "messageFormat": 0, // FarmAjaxMessageFormat.Default
    "value": {
        "name": "Babayetu",
        "tag": "0284A",
        "id": 20
    }
}
```

You'll have to read the previous section if you wish to learn how to now call this AJAX action from client-side javascript.

Hopefully this is all you'll need to get started with creating new AJAX actions :)

## GraphQL

TODO: Because... dear lord how do I explain this.
