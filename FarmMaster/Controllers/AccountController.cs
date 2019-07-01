using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Controllers
{
    public class AccountController : Controller
    {
        readonly IServiceUserManager _users;

        public AccountController(IServiceUserManager users)
        {
            this._users = users;
        }

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
        public IActionResult Signup(AccountSignupViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            this._users.CreateUser(model.LoginInfo.Username, model.LoginInfo.Password,
                                   model.Contact.FirstName,  model.Contact.MiddleNames,
                                   model.Contact.LastName,   model.Contact.Email);
            return Redirect("/");
        }
    }
}
