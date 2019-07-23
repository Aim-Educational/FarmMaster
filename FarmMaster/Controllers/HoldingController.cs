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
    public class HoldingController : Controller
    {
        readonly FarmMasterContext _context;
        readonly IServiceHoldingData _holdings;

        public HoldingController(FarmMasterContext context, IServiceHoldingData holdings)
        {
            this._holdings = holdings;
            this._context = context;
        }

        public IActionResult Index()
        {
            return View(new HoldingIndexViewModel
            {
                Holdings = this._holdings.QueryAllIncluded()
            });
        }

        public IActionResult Create()
        {
            return View("CreateEdit", new HoldingCreateEditViewModel
            {
                AllRegistrations = this._context.EnumHoldingRegistrations,
                IsCreate = true
            });
        }
    }
}