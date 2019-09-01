# Life Events

The following entities make up most of the Life Event portion of the model:

* LifeEvent

* LifeEventEntry

* LifeEventDynamicFieldInfo

* LifeEventDynamicFieldValue

## LifeEvent

This entity is responsible for containing the general information about an event (name, description, etc.) but
does not contain any info about the data the event holds.

## LifeEventDynamicFieldInfo

This entity is responsible for describing a singular field for a `LifeEvent`. There can be many field infos for a single life event.

This only *describes* the field (name, data type, description, etc.) and **does not** store any actual *values*.

## LifeEventEntry and LifeEventDynamicFieldValue

While `LifeEvent` and `LifeEventDynamicFieldInfo` are merely for *describing* the life event, we still need a way to
make 'instances' of this event.

The `LifeEventEntry` entity is esentially an 'instance' of a specific `LifeEvent`. The `LifeEventEntry` has many `LifeEventDynamicFieldValue` entities,
which store the actual values.

`LifeEventDynamicFieldValues` entities will point to the `LifeEventDynamicFieldInfo` that it's supposed to be storing a value for. This is used
for type checking, and ensuring that all field values, field infos, and field entries, are the correct type, and all point back to the same root `LifeEvent`.

## So in short

`LifeEvent` is the 'root' entity that all the other entity types refer to. It stores general information about an event.

`LifeEventDynamicFieldInfo` is the 'field *description*' entity that points to a `LifeEvent`. It stores information about a field for the event.

`LifeEventEntry` is the  'instance' entity for a certain `LifeEvent`. It doesn't actually store any data on its own (aside from a foreign key to a `LifeEvent`),
but it is used as a way to group `LifeEventDynamicFieldValue`s together.

`LifeEventDynamicFieldValue` is the 'field *value*' entity for a certain `LifeEventEntry`. It stores the actual instance data for an entry, and it has
a foreign key to the appropriate `LifeEventDynamicFieldInfo` that describes it.

## "This sounds complicated to code :("

Fret not, as you shouldn't even be manually manipulating these relationships.

Instead, use the `IServiceLifeEventManager` service, found in the file `/{root}/FarmMaster/Services/IServiceLifeEventManager.cs`.

It provides an easy way to create life events, create field info for events, create entries for an event which also includes setting/editing the values
for an entry.

## "How do we 'connect' life events to other entities, such as animals?"

Via the use of a mapping table. You map a `LifeEventEntry` to an `Animal` for example.
