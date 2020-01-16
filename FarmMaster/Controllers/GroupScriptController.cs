using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Controllers
{
    public class GroupScriptController : Controller
    {
        public IActionResult Editor()
        {
            return View();
        }
    }
}