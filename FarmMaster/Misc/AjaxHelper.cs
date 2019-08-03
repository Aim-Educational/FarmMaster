using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Models;
using FarmMaster.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Misc
{
    public static class AjaxHelper
    {
        public static IActionResult DoAjaxWithMessageResponse(
            this Controller controller,
            AjaxRequestModel model,
            IServiceUserManager users,
            IServiceRoleManager roles,
            string[] permsNeeded,
            Action<User> action)
        {
            var message = new AjaxResponseWithMessageModel();

            if (!controller.ModelState.IsValid)
            {
                message.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(controller.ModelState));
                return controller.Json(message);
            }

            try
            {
                var user = AjaxHelper.DoValidation(model, users, roles, permsNeeded);
                action(user);
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.MessageType = ViewModelWithMessage.Type.Error;
                return controller.Json(message);
            }

            return controller.Json(message);
        }

        public static IActionResult DoAjaxWithValueAndMessageResponse<T>(
            this Controller controller,
            AjaxRequestModel model,
            IServiceUserManager users,
            IServiceRoleManager roles,
            string[] permsNeeded,
            Func<User, T> action)
        where T : class
        {
            var response = new AjaxResponseWithMessageAndValueModel<T>();

            if (!controller.ModelState.IsValid)
            {
                response.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(controller.ModelState));
                return controller.Json(response);
            }

            try
            {
                var user = AjaxHelper.DoValidation(model, users, roles, permsNeeded);
                response.Value = action(user);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.MessageType = ViewModelWithMessage.Type.Error;
                return controller.Json(response);
            }

            return controller.Json(response);
        }

        private static User DoValidation(
            AjaxRequestModel model,
            IServiceUserManager users,
            IServiceRoleManager roles,
            string[] permsNeeded)
        {
            var user = users.UserFromCookieSession(model.SessionToken);
            if (user == null)
                throw new Exception("You are not logged in.");

            if (permsNeeded != null && !permsNeeded.All(p => roles.HasPermission(user.Role, p)))
                throw new Exception("You do not have permission to do that.");
            
            return user;
        }
    }
}
