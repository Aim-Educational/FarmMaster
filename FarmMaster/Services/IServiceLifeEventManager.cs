﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Misc;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Services
{
    // Might have to split this up
    // While these classes are all tightly coupled to eachother, it's a bit over the top
    public interface IServiceLifeEventManager : IServiceEntityManager<LifeEvent>,
                                                IServiceEntityManager<LifeEventDynamicFieldInfo>,
                                                IServiceEntityManager<LifeEventDynamicFieldValue>,
                                                IServiceEntityManager<LifeEventEntry>,
                                                IServiceEntityManagerFullDeletion<LifeEvent>
    {
        LifeEvent CreateEvent(string name, string description);
        LifeEventDynamicFieldInfo CreateEventField(LifeEvent @event, string name, string description, DynamicField.Type type);
        LifeEventEntry CreateEventEntry(LifeEvent @event, IDictionary<string, DynamicField> values);

        CouldDelete RemoveEventFieldByName(LifeEvent @event, string fieldName);

        void UpdateEventEntryFieldValueByName(LifeEventEntry entry, string fieldName, DynamicField value);

        // Certain life events are built-in, so get special 'easy'-to-use support.
        LifeEventEntry CreateBornEventEntry(DateTimeOffset dateTimeBorn);
        DateTimeOffset GetDateTimeBorn(LifeEventEntry bornEntry);
        LifeEventEntry FindBornEventEntryOrNull(IEnumerable<LifeEventEntry> entries);
    }
    
    public class ServiceLifeEventManager : IServiceLifeEventManager
    {
        readonly FarmMasterContext _context;

        public ServiceLifeEventManager(FarmMasterContext context)
        {
            this._context = context;
        }

        public LifeEvent CreateEvent(string name, string description)
        {
            if(this._context.LifeEvents.Any(e => e.Name.ToLower() == name.ToLower()))
                throw new InvalidOperationException($"A Life Event called '{name}' already exists.");

            var @event = new LifeEvent
            {
                Description = description,
                Name = name
            };

            this._context.Add(@event);
            this._context.SaveChanges();
            return @event;
        }

        public LifeEventDynamicFieldInfo CreateEventField(LifeEvent @event, string name, string description, DynamicField.Type type)
        {
            var field = new LifeEventDynamicFieldInfo
            {
                Description = description,
                LifeEvent = @event,
                Name = name,
                Type = type
            };

            this._context.Add(field);
            this._context.SaveChanges();
            return field;
        }

        public LifeEventEntry CreateEventEntry(LifeEvent @event, IDictionary<string, DynamicField> values)
        {
            // Make sure we have what we need included.
            @event = this.For<LifeEvent>()
                         .Query()
                         .Include(e => e.Fields)
                         .First(e => e.LifeEventId == @event.LifeEventId);

            var entry = new LifeEventEntry
            {
                LifeEvent = @event,
                DateTimeCreated = DateTimeOffset.UtcNow
            };

            // Create the values, ensuring that the name and type exist/match
            var valuesToAdd = new List<LifeEventDynamicFieldValue>();            
            foreach(var kvp in values)
            {
                var info = @event.Fields.FirstOrDefault(f => f.Name == kvp.Key);
                if(info == null)
                    throw new ArgumentOutOfRangeException($"LifeEvent '{@event.Name}' does not define a field named '{kvp.Key}'");

                if(info.Type != kvp.Value.FieldType)
                    throw new InvalidOperationException($"Field '{info.Name}' of type '{info.Type}' cannot be set to value '{kvp.Value.ToHtmlString()}' of type '{kvp.Value.FieldType}'");

                var value = new LifeEventDynamicFieldValue
                {
                    LifeEventDynamicFieldInfo = info,
                    LifeEventEntry = entry,
                    Value = kvp.Value
                };
                valuesToAdd.Add(value);
            }
            
            // Check we have values for all fields.
            var valuesToAddNames = valuesToAdd.Select(v => v.LifeEventDynamicFieldInfo.Name);
            var allFieldNames = @event.Fields.Select(f => f.Name);
            var missingNames = new List<string>();
            foreach(var fieldName in allFieldNames)
            {
                if(!valuesToAddNames.Contains(fieldName))
                    missingNames.Add(fieldName);
            }

            if(missingNames.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Cannot create entry as not all fields have been given values:\n" +
                    $"\tAll fields:     {allFieldNames}\n" +
                    $"\tFields given:   {valuesToAddNames}\n" +
                    $"\tFields missing: {missingNames}"
                );
            }

            // Finally, add all the values to the context
            this._context.Add(entry);
            this._context.AddRange(valuesToAdd);
            this._context.SaveChanges();
            return entry;
        }

        public CouldDelete RemoveEventFieldByName(LifeEvent @event, string fieldName)
        {
            // Make sure we have what we need included.
            @event = this.For<LifeEvent>()
                         .Query()
                         .Include(e => e.Fields)
                         .First(e => e.LifeEventId == @event.LifeEventId);

            // Check that it exists, then delete it.
            var field = @event.Fields.FirstOrDefault(f => f.Name == fieldName);
            if(field == null)
                return CouldDelete.No;

            this._context.Remove(field);
            this._context.SaveChanges();
            return CouldDelete.Yes;
        }

        public void UpdateEventEntryFieldValueByName(LifeEventEntry entry, string fieldName, DynamicField value)
        {
            entry = this.For<LifeEventEntry>()
                        .Query()
                        .Include(e => e.LifeEvent)
                        .Include(e => e.Values)
                         .ThenInclude(v => v.LifeEventDynamicFieldInfo)
                        .First(e => e.LifeEventEntryId == entry.LifeEventEntryId);

            var dbValue = entry.Values.FirstOrDefault(v => v.LifeEventDynamicFieldInfo.Name == fieldName);
            if(dbValue == null)
                throw new Exception($"Could not find value or field called '{fieldName}' for Life Event '{entry.LifeEvent.Name}'");

            if (dbValue.LifeEventDynamicFieldInfo.Type != value.FieldType)
                throw new InvalidOperationException($"Field '{fieldName}' of type '{dbValue.LifeEventDynamicFieldInfo.Type}' cannot be set to value '{value.ToHtmlString()}' of type '{value.FieldType}'");

            dbValue.Value = value;
            this._context.Update(dbValue);
            this._context.SaveChanges();
        }

        #region Built in Life Events
        public LifeEventEntry CreateBornEventEntry(DateTimeOffset dateTimeBorn)
        {
            return this.CreateEventEntry(
                this._context.LifeEvents.First(e => e.Name == BusinessConstants.BuiltinLifeEvents.BORN),
                new Dictionary<string, DynamicField>
                {
                    {
                        BusinessConstants.BuiltinLifeEventFields.BORN_DATE,
                        new DynamicFieldDateTime
                        {
                            DateTime = dateTimeBorn
                        }
                    }
                }
            );
        }

        public DateTimeOffset GetDateTimeBorn(LifeEventEntry bornEntry)
        {
            // Make sure we have all the data we want.
            bornEntry = this.For<LifeEventEntry>()
                            .Query()
                            .Include(e => e.LifeEvent)
                            .Include(e => e.Values)
                             .ThenInclude(v => v.LifeEventDynamicFieldInfo)
                            .First(e => e.LifeEventEntryId == bornEntry.LifeEventEntryId);
            
            if(bornEntry.LifeEvent.Name != BusinessConstants.BuiltinLifeEvents.BORN)
                throw new ArgumentException($"Entry is for event '{bornEntry.LifeEvent.Name}', not '{BusinessConstants.BuiltinLifeEvents.BORN}'.");
            if(!bornEntry.LifeEvent.IsBuiltin)
                throw new InvalidOperationException($"While the event is the right name, it's not marked as builtin, so cannot assume that it is structured correctly.");

            var value = bornEntry.Values.First(v => v.LifeEventDynamicFieldInfo.Name == BusinessConstants.BuiltinLifeEventFields.BORN_DATE);
            var castedField = value.Value as DynamicFieldDateTime;
            if(castedField == null)
                throw new InvalidCastException("Internal error: castedField should not be null. Did the structure of the event change?");

            return castedField.DateTime;
        }

        public LifeEventEntry FindBornEventEntryOrNull(IEnumerable<LifeEventEntry> entries)
        {
            return entries.FirstOrDefault(e => e.LifeEvent.Name == BusinessConstants.BuiltinLifeEvents.BORN);
        }
        #endregion

        #region Impl IServiceEntityManager and IServiceEntityManagerFullDeletion (If there's a god, please spare my soul)
        public int GetIdFor(LifeEvent entity)
        {
            return entity.LifeEventId;
        }

        public int GetIdFor(LifeEventDynamicFieldInfo entity)
        {
            return entity.LifeEventDynamicFieldInfoId;
        }

        public int GetIdFor(LifeEventDynamicFieldValue entity)
        {
            return entity.LifeEventDynamicFieldValueId;
        }

        public int GetIdFor(LifeEventEntry entity)
        {
            return entity.LifeEventEntryId;
        }

        public IQueryable<LifeEvent> Query()
        {
            return this._context.LifeEvents;
        }

        public IQueryable<LifeEvent> QueryAllIncluded()
        {
            return this._context.LifeEvents
                                .Include(e => e.Entries)
                                 .ThenInclude(e => e.Values)
                                .Include(e => e.Fields);
        }

        public void Update(LifeEvent entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }

        public void Update(LifeEventDynamicFieldInfo entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }

        public void Update(LifeEventDynamicFieldValue entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }

        public void Update(LifeEventEntry entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }

        IQueryable<LifeEventDynamicFieldInfo> IServiceEntityManager<LifeEventDynamicFieldInfo>.Query()
        {
            return this._context.LifeEventDynamicFieldInfo;
        }

        IQueryable<LifeEventDynamicFieldValue> IServiceEntityManager<LifeEventDynamicFieldValue>.Query()
        {
            return this._context.LifeEventDynamicFieldValues;
        }

        IQueryable<LifeEventEntry> IServiceEntityManager<LifeEventEntry>.Query()
        {
            return this._context.LifeEventEntries;
        }

        IQueryable<LifeEventDynamicFieldInfo> IServiceEntityManager<LifeEventDynamicFieldInfo>.QueryAllIncluded()
        {
            // I can already taste the memory usage.
            return this._context.LifeEventDynamicFieldInfo
                                .Include(i => i.LifeEvent)
                                 .ThenInclude(e => e.Fields)
                                .Include(i => i.LifeEvent)
                                 .ThenInclude(e => e.Entries)
                                  .ThenInclude(e => e.Values);
        }

        IQueryable<LifeEventDynamicFieldValue> IServiceEntityManager<LifeEventDynamicFieldValue>.QueryAllIncluded()
        {
            return this._context.LifeEventDynamicFieldValues
                                .Include(v => v.LifeEventEntry)
                                 .ThenInclude(e => e.LifeEvent)
                                .Include(v => v.LifeEventDynamicFieldInfo)
                                 .ThenInclude(i => i.LifeEvent);
        }

        IQueryable<LifeEventEntry> IServiceEntityManager<LifeEventEntry>.QueryAllIncluded()
        {
            return this._context.LifeEventEntries
                                .Include(e => e.LifeEvent)
                                .Include(e => e.Values);
        }

        public void FullDelete(LifeEvent entity)
        {
            // Make sure we're using a tracked entity.
            entity = this.For<LifeEvent>().FromId(entity.LifeEventId);

            this._context.Remove(entity);
            this._context.SaveChanges();
        }
        #endregion
    }
}
