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
    [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.VIEW_LIFE_EVENTS })]
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

        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_LIFE_EVENTS })]
        public IActionResult Create()
        {
            return View();
        }

        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_LIFE_EVENTS })]
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
                Name = @event.Name,
                Description = @event.Description,
                Id = @event.LifeEventId,
                GET_Fields = @event.Fields,
                GET_IsInUse = @event.IsInUse
            };
            model.ParseMessageQueryString(message);

            return View(model);
        }

        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_LIFE_EVENT_ENTRY })]
        public IActionResult CreateEntry(int lifeEventId, string redirectController, string redirectAction)
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
                Type                = LifeEventEntryEditorType.Create,
                Values              = lifeEvent.Fields.ToDictionary(f => f.Name, _ => ""),
                RedirectAction      = redirectAction,
                RedirectController  = redirectController
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_LIFE_EVENTS })]
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
                model.Message = ex.Message; 
                model.MessageFormat = ViewModelWithMessage.Format.Default;
                model.MessageType = ViewModelWithMessage.Type.Error;
                return View(model);
            }

            return (id == null) ? RedirectToAction("Index") : RedirectToAction("Edit", new { id = id.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_LIFE_EVENTS })]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FarmAuthorise(PermsAND: new[] { EnumRolePermission.Names.EDIT_LIFE_EVENT_ENTRY })]
        public IActionResult CreateEntry(LifeEventEntryEditorViewModel model)
        {
            // TODO: Change the code so we don't have to do 'AllIncluded', as this will *not* scale well
            //       after a decent amount of use.
            var @event = this._lifeEvents.For<LifeEvent>().FromIdAllIncluded(model.LifeEventId);
            if(@event == null)
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
                    lifeEventEntryId = entry.LifeEventEntryId
                }
            );
        }

        #region AJAX
        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessage(EnumRolePermission.Names.EDIT_LIFE_EVENTS)]
        public IActionResult AjaxAddField([FromBody] AjaxLifeEventAddFieldRequest model, User _)
        {
            var @event = this._lifeEvents.For<LifeEvent>().FromIdAllIncluded(model.LifeEventId);
            if(@event == null)
                throw new ArgumentOutOfRangeException($"No event with ID #{model.LifeEventId} was found.");

            if (@event.IsInUse)
                throw new InvalidOperationException($"Cannot modify the fields of event '{@event.Name}' as it is currently in use.");

            this._lifeEvents.CreateEventField(@event, model.Name, model.Description, model.Type);
            return new EmptyResult();
        }

        [HttpPost]
        [AllowAnonymous]
        [FarmAjaxReturnsMessage(EnumRolePermission.Names.EDIT_LIFE_EVENTS)]
        public IActionResult AjaxDeleteField([FromBody] AjaxLifeEventDeleteFieldRequest model, User _)
        {
            var @event = this._lifeEvents.For<LifeEvent>().FromIdAllIncluded(model.LifeEventId);
            if (@event == null)
                throw new ArgumentOutOfRangeException($"No event with ID #{model.LifeEventId} was found.");

            if(@event.IsInUse)
                throw new InvalidOperationException($"Cannot modify the fields of event '{@event.Name}' as it is currently in use.");
            
            var result = this._lifeEvents.RemoveEventFieldByName(@event, model.Name);
            if(result == CouldDelete.No)
                throw new ArgumentOutOfRangeException($"Could not delete field '{model.Name}', does it even exist?");

            return new EmptyResult();
        }
        #endregion
    }
}