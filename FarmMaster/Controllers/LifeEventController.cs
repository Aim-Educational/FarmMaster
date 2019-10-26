using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Misc;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmMaster.Controllers
{
    [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.VIEW_LIFE_EVENTS })]
    public class LifeEventController : Controller
    {
        readonly IServiceLifeEventManager _lifeEvents;

        public LifeEventController(IServiceLifeEventManager lifeEvents)
        {
            this._lifeEvents = lifeEvents;
        }

        public IActionResult Index(string message)
        {
            var model = new LifeEventIndexViewModel
            {
                LifeEvents = this._lifeEvents.For<LifeEvent>().Query()
            };
            model.ParseMessageQueryString(message);

            return View(model);
        }

        #region Event Pages (GET)
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENTS })]
        public IActionResult Create()
        {
            return View();
        }

        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENTS })]
        public IActionResult Edit(string message, int id)
        {
            // The 'AllIncluded' variants include loading *every single entry and all their values*
            // While that is correct behaviour for that function variant, it's a bit... undesirable.
            // So we include excatly what we need, and no more.
            var @event = this._lifeEvents
                             .For<LifeEvent>()
                             .Query()
                             .Include(e => e.Fields)
                             .Include(e => e.Entries)
                             .FirstOrDefault(e => e.LifeEventId == id);
            if(@event == null)
            {
                return RedirectToAction(nameof(Index), new
                {
                    message = ViewModelWithMessage.CreateErrorQueryString($"No Life Event with ID #{id} was found.")
                });
            }

            if(@event.IsBuiltin)
            {
                return RedirectToAction(nameof(Index), new
                {
                    message = ViewModelWithMessage.CreateErrorQueryString(
                        $"The '{@event.Name}' Life Event is a builtin event, and cannot be modified."
                    )
                });
            }

            var model = new LifeEventEditViewModel
            {
                Name        = @event.Name,
                Description = @event.Description,
                Id          = @event.LifeEventId,
                GET_Fields  = @event.Fields,
                GET_IsInUse = @event.IsInUse
            };
            model.ParseMessageQueryString(message);

            return View(model);
        }

        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENTS })]
        public IActionResult Delete(int id)
        {
            var @event = this._lifeEvents.For<LifeEvent>().FromIdAllIncluded(id);
            if (@event == null)
            {
                return RedirectToAction(nameof(Index), new
                {
                    message = ViewModelWithMessage.CreateErrorQueryString($"No Life Event with ID #{id} was found.")
                });
            }

            if (@event.IsBuiltin)
            {
                return RedirectToAction(nameof(Index), new
                {
                    message = ViewModelWithMessage.CreateErrorQueryString($"Life Event '{@event.Name}' is builtin, so cannot be deleted.")
                });
            }

            if (@event.IsInUse)
            {
                return RedirectToAction(nameof(Index), new
                {
                    message = ViewModelWithMessage.CreateErrorQueryString(
                        $"Life Event '{@event.Name}' is in use, so cannot be deleted " +
                        $"until all uses of it have also been deleted."
                    )
                });
            }

            this._lifeEvents.FullDelete(@event);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Event Pages (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENTS })]
        public IActionResult Create(LifeEventCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ParseInvalidModelState(ModelState);
                return View(model);
            }
            
            int? id;
            try
            { 
                id = this._lifeEvents.CreateEvent(model.Name, model.Description).LifeEventId;
            }
            catch(InvalidOperationException ex)
            {
                model.Message       = ex.Message; 
                model.MessageFormat = ViewModelWithMessage.Format.Default;
                model.MessageType   = ViewModelWithMessage.Type.Error;
                return View(model);
            }

            return (id == null) ? RedirectToAction("Index") : RedirectToAction("Edit", new { id = id.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENTS })]
        public IActionResult Edit(LifeEventEditViewModel model)
        {
            var @event = this._lifeEvents
                             .For<LifeEvent>()
                             .Query()
                             .Include(e => e.Fields)
                             .Include(e => e.Entries)
                             .FirstOrDefault(e => e.LifeEventId == model.Id); ;
            if(@event == null)
            {
                return RedirectToAction(nameof(Index), new
                {
                    message = ViewModelWithMessage.CreateErrorQueryString($"No Life Event with the ID #{model.Id} was found.")
                });
            }

            // We don't want to store these in hidden inputs, as it's kind of important that
            // these values can't be modified by a malicious user to trick the server into doing stuff it shouldn't.
            model.GET_IsInUse = @event.IsInUse;
            model.GET_Fields = @event.Fields;

            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
                return View(model);
            }

            if (@event.IsBuiltin)
            {
                return RedirectToAction(nameof(Index), new
                {
                    message = ViewModelWithMessage.CreateErrorQueryString(
                        $"The '{@event.Name}' Life Event is a builtin event, and cannot be modified."
                    )
                });
            }

            @event.Name = model.Name;
            @event.Description = model.Description;

            this._lifeEvents.Update(@event);

            model.MessageType = ViewModelWithMessage.Type.Information;
            model.Message = "Success";
            return View(model);
        }
        #endregion

        #region Entry Editor (GET)
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENT_ENTRY })]
        public IActionResult CreateEntry(
            int lifeEventId, 
            string redirectController, 
            string redirectAction, 
            string breadcrumb, 
            int redirectEntityId)
        {
            var lifeEvent = this._lifeEvents
                                .For<LifeEvent>()
                                .Query()
                                .Include(e => e.Fields)
                                .FirstOrDefault(e => e.LifeEventId == lifeEventId);
            if (lifeEvent == null)
                throw new ArgumentOutOfRangeException($"There is no LifeEvent with the ID #{lifeEventId}");

            return View("EntryEditor", new LifeEventEntryEditorViewModel
            {
                GET_FieldInfo       = lifeEvent.Fields,
                LifeEventId         = lifeEventId,
                LifeEventName       = lifeEvent.Name,
                Type                = LifeEventEntryEditorType.Create,
                Values              = lifeEvent.Fields.ToDictionary(f => f.Name, _ => ""),
                RedirectAction      = redirectAction,
                RedirectController  = redirectController,
                RedirectEntityId    = redirectEntityId,
                                      // Example: abc:123/345>def:123/567
                Breadcrumb          = breadcrumb.Split('>')
                                                .ToDictionary(
                                                    s => s.Split(':')[0],
                                                    s => s.Split(':')[1]
                                                )
            });
        }

        // This function can't create entries, and might be useful for people who can edit life events, but not entries.
        // So we're using either permission here.
        [FarmAuthorise(PermsOR: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENTS, BusinessConstants.Roles.EDIT_LIFE_EVENT_ENTRY })]
        public IActionResult TestEntryEditor(int lifeEventId)
        {
            // Using AllIncluded, since in practice this will barely be used after initial setup week.
            var lifeEvent = this._lifeEvents.For<LifeEvent>().FromIdAllIncluded(lifeEventId);
            if (lifeEvent == null)
                throw new ArgumentOutOfRangeException($"There is no LifeEvent with the ID #{lifeEventId}");

            return View("EntryEditor", new LifeEventEntryEditorViewModel
            {
                GET_FieldInfo       = lifeEvent.Fields,
                LifeEventId         = lifeEventId,
                LifeEventName       = lifeEvent.Name,
                Type                = LifeEventEntryEditorType.Test,
                Values              = lifeEvent.Fields.ToDictionary(f => f.Name, _ => ""),
                RedirectAction      = "TestEntry",
                RedirectController  = "LifeEvent",
                Breadcrumb = new Dictionary<string, string>
                {
                    { "Home",        "/Home/Index" },
                    { "Life Events", "/LifeEvent/Index" },
                    { "Edit",       $"/LifeEvent/Edit?id={lifeEventId}" },
                    { "Test Editor", "#" }
                }
            });
        }

        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENT_ENTRY })]
        public IActionResult EditEntry(int lifeEventId, int lifeEventEntryId, string breadcrumb, int redirectEntityId)
        {
            var lifeEvent = this._lifeEvents
                                .For<LifeEvent>()
                                .Query()
                                .Include(e => e.Fields)
                                .Include(e => e.Entries)
                                 .ThenInclude(e => e.Values)
                                  .ThenInclude(v => v.LifeEventDynamicFieldInfo)
                                .FirstOrDefault(e => e.LifeEventId == lifeEventId);
            if (lifeEvent == null)
                throw new ArgumentOutOfRangeException($"There is no LifeEvent with the ID #{lifeEventId}");

            var entry = lifeEvent.Entries.First(e => e.LifeEventEntryId == lifeEventEntryId);
            return View("EntryEditor", new LifeEventEntryEditorViewModel
            {
                GET_FieldInfo       = lifeEvent.Fields,
                LifeEventId         = lifeEventId,
                LifeEventName       = lifeEvent.Name,
                LifeEventEntryId    = lifeEventEntryId,
                Type                = LifeEventEntryEditorType.Edit,
                RedirectAction      = "EditEntry",
                RedirectController  = "LifeEvent",
                RedirectEntityId    = redirectEntityId,
                Breadcrumb          = breadcrumb.Split('>')
                                                .ToDictionary(
                                                    s => s.Split(':')[0],
                                                    s => s.Split(':')[1]
                                                ),
                Values              = lifeEvent
                                      .Fields
                                      .ToDictionary(
                                          f => f.Name,
                                          f => entry.Values
                                                    .First(v => v.LifeEventDynamicFieldInfo.Name == f.Name)
                                                    .Value
                                                    .ToHtmlString()
                                      ),
            });
        }
        #endregion

        #region Entry Editor (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENT_ENTRY })]
        public IActionResult CreateEntry(LifeEventEntryEditorViewModel model)
        {
            // TODO: Change the code so we don't have to do 'AllIncluded', as this will *not* scale well
            //       after a decent amount of use.
            var @event = this._lifeEvents.For<LifeEvent>().FromIdAllIncluded(model.LifeEventId);
            if (@event == null)
                throw new ArgumentOutOfRangeException($"No LifeEvent with ID #{model.LifeEventId}");

            model.GET_FieldInfo = @event.Fields;

            if (!ModelState.IsValid)
            {
                model.ParseInvalidModelState(ModelState);
                return View("EntryEditor", model);
            }

            // Create the entry
            var entries = new Dictionary<string, DynamicField>();
            var fieldFactory = new DynamicFieldFactory();
            foreach (var kvp in model.Values)
            {
                var info = @event.Fields.First(f => f.Name == kvp.Key);
                var data = fieldFactory.FromTypeAndHtmlString(info.Type, kvp.Value);

                entries[kvp.Key] = data;
            }

            var entry = this._lifeEvents.CreateEventEntry(@event, entries);

            return RedirectToActionPermanent(
                model.RedirectAction,
                model.RedirectController,
                new
                {
                    lifeEventEntryId = entry.LifeEventEntryId,
                    redirectEntityId = model.RedirectEntityId
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { BusinessConstants.Roles.EDIT_LIFE_EVENT_ENTRY })]
        public IActionResult EditEntry(LifeEventEntryEditorViewModel model)
        {
            // TODO: Change the code so we don't have to do 'AllIncluded', as this will *not* scale well
            //       after a decent amount of use.
            var @event = this._lifeEvents.For<LifeEvent>().FromIdAllIncluded(model.LifeEventId);
            if (@event == null)
                throw new ArgumentOutOfRangeException($"No LifeEvent with ID #{model.LifeEventId}");

            model.GET_FieldInfo = @event.Fields;

            if (!ModelState.IsValid)
            {
                model.ParseInvalidModelState(ModelState);
                return View("EntryEditor", model);
            }

            // Update the entry
            var entry = @event.Entries.FirstOrDefault(e => e.LifeEventEntryId == model.LifeEventEntryId);
            if(entry == null)
                throw new ArgumentOutOfRangeException($"No LifeEventEntry with ID #{model.LifeEventEntryId} belongs to Life Event '{@event.Name}'");
            
            var fieldFactory = new DynamicFieldFactory();
            foreach (var kvp in model.Values)
            {
                var info = @event.Fields.First(f => f.Name == kvp.Key);
                this._lifeEvents.UpdateEventEntryFieldValueByName(entry, kvp.Key, fieldFactory.FromTypeAndHtmlString(info.Type, kvp.Value));
            }

            return View("EntryEditor", model);
        }
        #endregion
    }
}