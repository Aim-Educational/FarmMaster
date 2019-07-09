using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Models;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Controllers
{
    public class ContactController : Controller
    {
        readonly FarmMasterContext _context;

        public ContactController(FarmMasterContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            return View(new ContactIndexViewModel
            {
                Contacts = this._context.Contacts
            });
        }
    }
}