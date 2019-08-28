using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Filters;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;

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
            var @event = this._lifeEvents.For<LifeEvent>().FromId(id);
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
                Id = @event.LifeEventId
            };
            model.ParseMessageQueryString(message);

            return View(model);
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
            if (!ModelState.IsValid)
            {
                model.ParseMessageQueryString(ViewModelWithMessage.CreateQueryString(ModelState));
                return View(model);
            }

            var @event = this._lifeEvents.For<LifeEvent>().FromId(model.Id);
            if(@event == null)
            {
                return RedirectToAction(nameof(Index), new
                {
                    message = ViewModelWithMessage.CreateErrorQueryString($"No Life Event with the ID #{model.Id} was found.")
                });
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
    }
}