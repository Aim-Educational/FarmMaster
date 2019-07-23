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
            AjaxModel model,
            IServiceUserManager users,
            IServiceRoleManager roles,
            string[] permsNeeded,
            Action<User> action)
        {
            var message = new AjaxModelWithMessage();

            if (!controller.ModelState.IsValid)
            {
                message.Message.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(controller.ModelState));
                return controller.Json(message);
            }

            try
            {
                var user = AjaxHelper.DoValidation(model, users, roles, permsNeeded);
                action(user);
            }
            catch (Exception ex)
            {
                message.Message.Message = ex.Message;
                message.Message.MessageType = ViewModelWithMessage.Type.Error;
                return controller.Json(message);
            }

            return controller.Json(message);
        }

        public static IActionResult DoAjaxWithValueAndMessageResponse<T>(
            this Controller controller,
            AjaxModel model,
            IServiceUserManager users,
            IServiceRoleManager roles,
            string[] permsNeeded,
            Func<User, T> action)
        where T : class
        {
            var response = new AjaxModelWithValueAndMessage<T>();

            if (!controller.ModelState.IsValid)
            {
                response.Message.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(controller.ModelState));
                return controller.Json(response);
            }

            try
            {
                var user = AjaxHelper.DoValidation(model, users, roles, permsNeeded);
                response.Value = action(user);
            }
            catch (Exception ex)
            {
                response.Message.Message = ex.Message;
                response.Message.MessageType = ViewModelWithMessage.Type.Error;
                return controller.Json(response);
            }

            return controller.Json(response);
        }

        private static User DoValidation(
            AjaxModel model,
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
