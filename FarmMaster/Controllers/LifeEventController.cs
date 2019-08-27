using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Controllers
{
    public class LifeEventController : Controller
    {
        readonly IServiceLifeEventManager _lifeEvents;

        public LifeEventController(IServiceLifeEventManager lifeEvents)
        {
            this._lifeEvents = lifeEvents;
        }

        public IActionResult Index()
        {
            return View(new LifeEventIndexViewModel
            {
                LifeEvents = this._lifeEvents.For<LifeEvent>().Query()
            });
        }
    }
}