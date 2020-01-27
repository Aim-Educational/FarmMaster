﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FarmMaster.Services;
using Microsoft.EntityFrameworkCore;
using Business.Model;

namespace FarmMaster.Filters
{
    public class FarmAuthoriseAttribute : TypeFilterAttribute
    {
        public FarmAuthoriseAttribute(string[] PermsAND = null, string[] PermsOR = null) : base(typeof(FarmAuthoriseFilter))
        {
            PermsAND = PermsAND ?? Array.Empty<string>();
            PermsOR = PermsOR ?? Array.Empty<string>();
            Arguments = new object[] { PermsAND, PermsOR };
        }
    }

    public class FarmAuthoriseFilter : IAuthorizationFilter
    {
        readonly string[] PermsAND;
        readonly string[] PermsOR;

        public FarmAuthoriseFilter(string[] PermsAND, string[] PermsOR)
        {
            this.PermsAND = PermsAND;
            this.PermsOR = PermsOR;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext
                              .RequestServices
                              .GetRequiredService<IServiceUserManager>()
                              .UserFromCookieSession(context.HttpContext);

            if(user == null)
            {
                context.Result = new RedirectResult("/Account/Login");
                return;
            }

            var perms = user.Role?.Permissions
                     ?? Array.Empty<MapRolePermissionToRole>().AsQueryable();

            if(user.Role?.IsGodRole ?? false)
                return;

            if(PermsAND.Any()
            && !PermsAND.All(p => perms.Any(up => up.EnumRolePermission.InternalName == p)))
                context.Result = new ForbidResult();

            if(PermsOR.Any()
            && !PermsOR.Any(p => perms.Any(up => up.EnumRolePermission.InternalName == p)))
                context.Result = new ForbidResult();
        }
    }
}