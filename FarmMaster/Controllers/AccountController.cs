using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(AccountSignupViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            return Redirect("/");
        }
    }
}
