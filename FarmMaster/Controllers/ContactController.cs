using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Constants;
using DataAccessLogic;
using FarmMaster.Models;
using FarmMaster.Module.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FarmMaster.Controllers
{
    public class ContactController : CrudController<Contact, IContactManager>
    {
        // So we don't constantly recreate instances.
        static readonly CrudControllerConfig CONFIG = new CrudControllerConfig 
        {
            DeletePolicy = Permissions.Contact.Delete,
            ManagePolicy = Permissions.Contact.ManageUI,
            ReadPolicy   = Permissions.Contact.Read,
            WritePolicy  = Permissions.Contact.Write
        };

        protected override CrudControllerConfig Config => CONFIG;

        public ContactController(
            IContactManager contacts, 
            IUnitOfWork unitOfWork, 
            ILogger<ContactController> logger
        )
        : base(contacts, unitOfWork, logger)
        {
        }

        protected override Contact CreateEntityFromModel(CrudCreateEditViewModel<Contact> model)
        {
            var contact = new Contact();
            this.UpdateEntityFromModel(model, ref contact);

            return contact;
        }

        protected override void UpdateEntityFromModel(CrudCreateEditViewModel<Contact> model, ref Contact entity)
        {
            // Whitelisting values that the user can provide
            entity.Name  = model.Entity.Name;
            entity.Email = model.Entity.Email;
            entity.Phone = model.Entity.Phone;
            entity.Type  = model.Entity.Type;
        }
    }
}