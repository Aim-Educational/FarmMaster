using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmMaster.Models;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Misc
{
    public static class AjaxHelper
    {
        public static IActionResult DoAjaxWithMessageResponse(this Controller controller, Action action)
        {
            var message = new AjaxModelWithMessage();

            if (!controller.ModelState.IsValid)
            {
                message.ParseMessageQueryString(ViewModelWithMessage.CreateMessageQueryString(controller.ModelState));
                return controller.Json(message);
            }

            try
            {
                action();
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.MessageType = ViewModelWithMessage.Type.Error;
                return controller.Json(message);
            }

            return controller.Json(message);
        }
    }
}
