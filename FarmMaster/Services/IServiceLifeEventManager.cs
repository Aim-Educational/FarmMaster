using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Services
{
    // Might have to split this up
    // While these classes are all tightly coupled to eachother, it's a bit over the top
    public interface IServiceLifeEventManager : IServiceEntityManager<LifeEvent>,
                                                IServiceEntityManager<LifeEventDynamicFieldInfo>,
                                                IServiceEntityManager<LifeEventDynamicFieldValue>,
                                                IServiceEntityManager<LifeEventEntry>
    {
        LifeEvent CreateEvent(string name, string description);
        LifeEventDynamicFieldInfo CreateEventField(LifeEvent @event, string name, string description, DynamicField.Type type);
        LifeEventEntry CreateEventEntry(LifeEvent @event, string name, IDictionary<string, DynamicField> values);
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

        public LifeEventEntry CreateEventEntry(LifeEvent @event, string name, IDictionary<string, DynamicField> values)
        {
            // Make sure we have what we need included.
            @event = this.For<LifeEvent>()
                         .Query()
                         .Include(e => e.Fields)
                         .First(e => e.LifeEventId == @event.LifeEventId);

            var entry = new LifeEventEntry
            {
                LifeEvent = @event
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

        #region Impl IServiceEntityManager (If there's a god, please spare my soul)
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
        }

        public void Update(LifeEventDynamicFieldInfo entity)
        {
            this._context.Update(entity);
        }

        public void Update(LifeEventDynamicFieldValue entity)
        {
            this._context.Update(entity);
        }

        public void Update(LifeEventEntry entity)
        {
            this._context.Update(entity);
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
        #endregion
    }
}
