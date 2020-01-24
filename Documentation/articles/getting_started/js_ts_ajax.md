# Javascript (JS), Typescript (TS), AJAX, and client-side GraphQL in FarmMaster

This section describes how FarmMaster makes use of, and organises its use of JS, TS, AJAX, and GraphQL (on the browser's side).

This section also details the usage of the other misc. Javascript libraries that FarmMaster provides.

## Typescript is used for libraries

FarmMaster needs to provide its webpages a bit of repeated functionality (such as using AJAX, more on that later), which is done
by providing its own set of libraries.

These libraries are written in Typescript, as it is essentially just a better javascript. Typescript isn't used for specific web pages
due to annoying amounts of friction during the TS -> JS process.

### Flow/pipeline/structure

Typescript usage in FarmMaster is structured in the following way:

1. Libraries are written inside of the `./FarmMaster/Scripts` folder.

2. One of the nuget packages FarmMaster uses should automatically compile any changes upon save. If not, install
   the typescript compiler and invoke the `tsc` command inside of the `./FarmMaster/Scripts` folder.

3. The compiled files are placed into the `./FarmMaster/wwwroot/js` folder as part of the compliation process.

4. For existing libraries: Any updates to the code will now take affect.

5. For newly made libraries: The file `./FarmMaster/wwwroot/js/index.js` needs to be modified to make the library visible.

6. Webpages can now import libraries via code such as `import { FarmAjax, GraphQL } from "/js/index.js"`

### index.js

FarmMaster's libraries are compiled into their own individual `.js` files, and it'd be pretty annoying to maintain code
that references half a dozen different files just to import the libraries they want.

To get around this, FarmMaster takes advantage of the fact that its Typescript libraries compile into [ES6 modules](https://www.w3schools.com/js/js_es6.asp)
so that the user code only needs to reference a singular, well-known (a.k.a it won't randomly dissapear during development) file known
as `index.js`.

Basically, `index.js`'s job is to act as a module (or library) that exports *other* modules (all our other libraries) anytime it is used
by the user code. If you give its code a read, you'll see what I mean.

One other, but unrelated, job of `index.js` is to configure [JasterValidate](https://github.com/SealabJaster/jaster-validate-js) which is a
JS form validation library I created (since the one bundled with ASP is too limited). For the most part you won't ever have to worry about that
part of `index.js`, but it was worth noting.

### Modifying an existing library

As per the pipeline above, simply edit the `.ts` file inside of the `./FarmMaster/Scripts` folder, and either save the file
(for automatic compliation) or use the typescript compiler (`tsc`) to compile your changes into JS.

Any changes made to the library will now be visible.

### Using a library

To use one of FarmMaster's libraries:

1. Open the `.cshtml` file you're modifying.

2. Create a `<script type="module"></script>` tag somewhere (usually inside of an `@section Scripts{ }` block).

3. Import any symbols (classes, enums, etc.) you need via `import { FarmAjax, GraphQL } from "/js/index.js"`

4. ???

5. Profit.

### Creating a library

To create a new library:

1. Create a new `.ts` file inside of the `./FarmMaster/Scripts` folder.

2. Write the code for the library (remember to use the `export` keyword where needed).

3. Compile your changes in some way (see one of the sections above).

4. Open the file at `./FarmMaster/wwwroot/js/index.js`.

5. Add a line similar to the following (change the path as needed): `export * from "/js/modal.js";`

## Javascript is used for page-specific logic

Since each webpage is different, they need their own specific logic, and to do this we fallback to good old JS.

Because FarmMaster's libraries are all written in the `ES6` version of Javascript, any javascript code needs to be inside of a
`module` type `<script>` tag: `<script type="module"></script>` otherwise things don't work.

One caveat of an ES6 script tag is that anything declared inside one isn't global to the webpage.

So say for example, you have multiple script tags but all of them need to access some utility function written inside of another script tag.

To do this is simple, simply add your utility function (or whatever you want to make global) into the `window` object:

```html
<script type="module">
    function someUtilFunc() { return "Andy"; }

    window.someUtilFunc = someUtilFunc;
</script>

<script type="module">
    // We can now access the util func as if it were global!
    alert("Mastermind is " + someUtilFunc()));
</script>
```

There is a downside that because most pages have their own kind of "ecosystem" for their JS code, and since virtually none of it is documented,
there is a large overhead of having to learn how a specific page works in its *current* form before you can begin making fixes/changes to it. And
you have to do this for *every* page.

I'm sure there's industry standard ways to handle this, but I'm just a mere self-taught programmer, who's only learning things as needed (and of course, it's
my first time writing JS to this sort of scale).

I do have plans to explore a framework called [Vue](https://vuejs.org/) soon (specifically for a refactor of the GroupScript editor's JS) to see if it'd be a good fit
to refactor the JS code elsewhere.

Another downside, although it's probably completely unavoidable without more effor than it is worth,
is that due to how heavily coupled a page's JS code is to the actual markup of the page, even simply restructuring the layout may sometimes also require fixes to the JS code.

## GraphQL and AJAX

A large portion of FarmMaster is supposed to be dynamic (as in, the page doesn't have to constantly refresh to show data).

There are pros and cons to this, but even just for the sake of me needing to get experience with dynamic web pages, I feel like my large usage of AJAX and
GraphQL are justified.

FarmMaster utilises both [AJAX](https://www.w3schools.com/js/js_ajax_intro.asp) and [GraphQL](https://graphql.org/), while also providing libraries to make
using these parts of FarmMaster simpler to handle.

A short version of their differences are:

* AJAX should only be used to modify data (but legacy code will still use it to retrieve data).

* GraphQL is **only** used to retrieve data.

There are exceptions (such as the aforementioned legacy code), but those will be explored in more detail below.

### GraphQL

GraphQL is used to retrieve data dynamically, whether that's because the website needs the lastest version of some data, or because
it needs access to data that'd be cumbersome to include inside of a page's ViewModel, it doesn't matter because GraphQL is here to save the day.

> [!NOTE]
> When I was integrating the GraphQL (server-side) into FarmMaster, the ability to restrict certain data to certain users (e.g. needing permission to see animal data)
> wasn't very well documented, so therefore any data exposed by GraphQL can be accessed by anyone who is simply logged in.
>
> So, until I look into the newer documentation for authorising certain data to certain users, only expose non-sensitive data through GraphQL.
>
> Or to be more blunt, if you ever have doubt about GDPR or some form of exploit that could be peformed with the data, then don't put it in GraphQL and instead
> create an AJAX endpoint which can be locked behind `[FarmAuthorise]`.

Also as this page is only going over the client-side portion of using GraphQL, this section won't cover how to expose new data through GraphQL (another article will do that).

#### Using GraphQL

First of all, if you'd like to simply test/experiment with FarmMaster's GraphQL setup, simply go to any FarmMaster instance and manually change the URL to point to the
`/graphiql` endpoint: e.g. `farm.chatha.dev/graphiql`, just make sure you're signed in.

If you're wanting to use GraphQL inside of a webpage, then you'll first need to import the `GraphQL` library:

```html
<script type="module">
    import { GraphQL } from "/js/index/js";
</script>
```

After that, you can use the `GraphQL.query(query: string, parameters: object = null): Promise<object>` function:

* query - The GraphQL query to execute. Use the `/graphiql` endpoint to make testing your query easier.

* parameters - If your query makes use of parameters, you can place them inside an object here. e.g The parameter `$someParam` could be given a value by using
  `{ someParam: 20 }` inside the object passed as the second parameter to this function.

* A normal JS [Promise](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise) is returned where the query's data can be accessed
  by using `.then(value => { ... })`.

Here is a real-life snippet of GraphQL being used in FarmMaster which shows a parameterised query being executed to load all needed
information about an animal (all within a single request!) before passing off each piece of data to a handler function:

```js
GraphQL
.query(`
query InitialLoad($animalId: ID){
    contacts {
        id
        name
    }
    species {
        id
        name
    }
    lifeEvents(target:ANIMAL) {
        id
        name
    }
    animals(id:$animalId) {
        lifeEventEntries {
            id
            dateTimeUtc
            lifeEvent {
                id
                name
            }
        }
    }
    holdings {
        id
        name
    }
    groupsAssignedTo: animalGroups(hasAnimalId:$animalId) {
        id
        name
        description
    }
    groupsNotAssignedTo: animalGroups(hasNotAnimalId:$animalId) {
        id
        name
        description
    }
}`,
{ animalId: @Model.AnimalId }
)
.then(data => {
    handleContacts(data.contacts);
    handleSpecies(data.species);
    handleLifeEventTypes(data.lifeEvents);
    handleLifeEventEntries(data.animals[0].lifeEventEntries);
    handleHoldings(data.holdings);
    handleGroups(data.groupsAssignedTo, data.groupsNotAssignedTo);
})
.catch(errors => onError(errors));
```

> [!NOTE]
> Please pay attention to the fact that the `data` object returned matches the exact layout of the query.
>
> e.g. Compare how `data.animals[0].lifeEventEntries` reflects the query itself.

### AJAX

As described, there are cases (such as modifying data) that GraphQL doesn't handle (GraphQL *can* handle modifying data, I just prefer AJAX here) so we need 
to make use of AJAX instead.

AJAX in FarmMaster basically boils down to: `Send JSON data to endpoint; Endpoint then returns JSON data in response (e.g. failed/succeeded).`

**All** AJAX endpoints in FarmMaster (barring a few legacy ones that are going to be replaced) are placed inside of the [AjaxController](xref:FarmMaster.Controllers.AjaxController). This is for a variety of reasons such as organisation reasons, as well as reasons such as "**Where the bloody hell is this AJAX endpoint stored that I need to modify the code for?**" or "**Oh god what controller was that endpoint in again?**" (true story).

AJAX endpoints in FarmMaster also follow a rather... *unconventional* naming scheme. For example, if you imagine a function/function chain that looks like
`Animal.ById(id).Characteristic.AsNameValueId().GetAll()` then the AJAX endpoint would be called `Animal_ById_Characteristic_AsNameValueId_All`.

You'll likely have to look at the existing endpoints to properly get a grasp of how things are named, but hopefully you'll at least try to see *why* I've done
things this way, an example being encoding the return value inside the name itself (`AsNameValueId` -> `{ name: string, value: whatever, id: number }` instead of just guessing or having to look at the C# side of things every time).

As with GraphQL, this article only deals with the client-side part of AJAX, so the act of creating/modifying endpoints will be placed into a server-side specific article.

#### Using AJAX

The first thing you need to do is to import the `FarmAjax` and `FarmAjaxMessageType` symbols:

```html
<script type="module">
    import { FarmAjax, FarmAjaxMessageType } from "/js/index/js";
</script>
```

The `FarmAjax` class contains functions for sending AJAX requests and collecting their results.

All responses from an AJAX endpoint contains a `.messageType` property of type `FarmAjaxMessageType` (an enum), which details
whether it was a success, failure, etc. So you'll need to import the symbol to be able to check whether your request worked or not.

Now, the next thing you need to do is determine which endpoint you want to call, what *type* of endpoint it is, and what parameters you want to pass to it.

There are two different *types* of AJAX endpoints in FarmMaster: `returns Message`, and `returns Message and Value`. The former simply returns a message (e.g. `.messageType`) 
while the latter returns both a message and a value, where the value is different between each endpoint.

So let's decide we want to use [Animal_ById_Characteristic_AsNameValueTypeInheritedId_All](xref:FarmMaster.Controllers.AjaxController#Animal_ById_Characteristic_AsNameValueTypeInheritedId_All)
which (if you can decode the name):

* Is a `returns Message and Value`-type endpoint. You can tell this either by looking into the C# code to see whether it uses [FarmAjaxReturnsMessageAndValue]
(xref:FarmMaster.Filters.FarmAjaxReturnsMessageAndValue) or [FarmAjaxReturnsMessage](xref:FarmMaster.Filters.FarmAjaxReturnsMessage), or by the fact the name contains
`AsNameValueTypeInheritedId` which is a common convention in the namings to describe what (if any) data is returned.

* It requires a parameter (because of `ById`). The convetion for this is: If there is only *one* id parameter, it is named `id`, otherwise if there are two, one is called `byId` and the other `forId`. In this case it'd be `id`.

* It returns an array (because of `All`) of objects containing a Name, Value, Type, Inherited (boolean), and Id.

Now that we have all of this figured out, we can send our AJAX request using `FarmAjax.postWithMessageAndValueResponse<T>(url: string, data: any, onDone: (response: FarmAjaxMessageAndValueResponse<T | null>) => void)`:

* url - The endpoint we want to use, so in this case `/Ajax/Animal_ById_Characteristic_AsNameValueTypeInheritedId_All`

* data - The object containing the data, so in this case we'll use `{ id: 1 }` (normally you'd get the ID somewhere else, such as the ViewModel).

* onDone - A function that is called if the AJAX request succeeded. I would've used a `Promise` like with GraphQL, but I literally didn't know they existed at that point in time
  and it's a bit too late now.

> [!NOTE]
> There is another version called `postWithMessageResponse` for endpoints that don't return a value.

I know this is a lot to take in, but it's simpler to use in practice than it is to explain here. Anyway, let's use this endpoint:

```html
<script type="module">
    import { FarmAjax, FarmAjaxMessageType } from "/js/index/js";

    FarmAjax.postWithMessageAndValueResponse(
        // url
        "/Ajax/Animal_ById_Characteristic_AsNameValueTypeInheritedId_All",

        // data
        { id: 1 },

        // onDone
        messageAndValue => {
            // We'll handle errors (.messageType) in a moment.

            // Example usage: Turn the value into JSON and show it on screen.
            alert(JSON.stringify(messageAndValue.value));
        }
    );
</script>
```

And if everything goes well (such as the animal with an ID of `1` existing), then it should be a success! Otherwise... well, we have
to check to see if something went wrong.

I admit this is a bit clunky, and if I ever get the energy to rewrite things then I'd make use of `Promise` (e.g. for `.catch`) instead, but to check if there's an error we 
basically check if the `.messageType` isn't `FarmAjaxMessageType.Information`. After that we call the `.populateMessageBox` function with a `div` that I'll show you
how to make in a second:

```html
<script type="module">
    import { FarmAjax, FarmAjaxMessageType } from "/js/index/js";

    FarmAjax.postWithMessageAndValueResponse(
        // url
        "/Ajax/Animal_ById_Characteristic_AsNameValueTypeInheritedId_All",

        // data
        { id: 1 },

        // onDone
        messageAndValue => {
            if(messageAndValue.messageType !== FarmAjaxMessageType.Information) {
                messageAndValue.populateMessageBox(document.getElementById("errorBox"));
                return;
            }

            // Example usage: Turn the value into JSON and show it on screen.
            alert(JSON.stringify(messageAndValue.value));
        }
    );
</script>
```

Basically all `populateMessageBox` does is show (if it's hidden) a [Semantic UI message box](https://semantic-ui.com/collections/message.html) with the appropriate
colouring and text based on what `.messageType`, `.messageFormat`, and `.message`'s values are. In other words, if something go wrong, show pretty message box.

In terms of the HTML for the error box, simply place this HTML *somewhere* that it makes sense and change the id to something that also makes sense,
the usual convention for FarmMaster is to prefix it with "error":

```html
<div id="errorBox" class="ui error message transition hidden"></div>
```

Again, it's not great, but I was still really *really* early into my learning with ASP, JS, Semantic UI, etc. back when I made that decision, so forgive me as I want
to change it as much as you do. :P

But anyway, that's how to use AJAX.

## Other libraries

FarmMaster also provides a few other libraries, which I may as well document a bit here.

### dranges

Another library I made that's not exclusive to FarmMaster, see [here](https://www.npmjs.com/package/dranges).

### JasterValidate

Again, see [here](https://github.com/SealabJaster/jaster-validate-js).

I will document specific usage in FarmMaster in it's own article though, as it's important.

### component_select.js

Legacy, will be replaced.

### Cookies

It exposes 3 free-standing functions called `setCookie(name, value)`, `getCookie(name)`, and `deleteCookie(name)`. They all kind of explain themselves.

### Modal

A wrapper around [Semantic UI modals](https://fomantic-ui.com/modules/modal.html).

Basically, it exists for predefined modals such as the modals contained in the partial views: `_ModalAreYouSure_NoUndo`, and `_ModalReason_ContactAction`.

For example, if you wanted a modal that asked a user "Do you want to do this, you can't undo it?" and then to execute an action *only* if they press "Yes", then you'd
do something like this:

```html
<partial name="_ModalAreYouSure_NoUndo" />

<script type="module">
    import { Modal } from "/js/index.js";

    // The partial we included already has the HTML expected for this function to work:
    Modal.askAreYouSure() // Returns a promise, so we use .then()
        .then(() => {
            alert("This will only show if the user presses yes!");
        });
</script>
```

If you have your own modal that you want to use, simply use the `Modal.show(modalId: string): Promise` function. The `modalId` being a HTML
`<div id="lala">` kind of thing.

### characteristic_helper.js

You really don't need to concern yourself with this too much unless you're messing with the `_ComponentCharacteristics` partial.

This file basically contains functions that can create strings that the server can interpret as a characteristic value. It's nice to have these functions
in its own library in case they need to be used elsewhere.

### component_table.js

Legacy, will be replaced.
