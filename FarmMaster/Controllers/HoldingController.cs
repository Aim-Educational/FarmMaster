using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Controllers
{
    public class HoldingController : Controller
    {
        readonly IServiceHoldingData _holdings;

        public HoldingController(IServiceHoldingData holdings)
        {
            this._holdings = holdings;
        }

        public IActionResult Index()
        {
            return View(new HoldingIndexViewModel
            {
                Holdings = this._holdings.QueryAllIncluded()
            });
        }
    }
}