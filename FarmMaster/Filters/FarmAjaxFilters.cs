using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmMaster.Services;
using FarmMaster.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Business.Model;
using System.Diagnostics.CodeAnalysis;

namespace FarmMaster.Filters
{
    #region With Message
    public class FarmAjaxReturnsMessageAttribute : TypeFilterAttribute
    {
        public FarmAjaxReturnsMessageAttribute(params string[] permsAND) : base(typeof(FarmAjaxReturnsMessageFilter))
        {
            Arguments = new object[] { permsAND ?? Array.Empty<string>() };
        }
    }

    public class FarmAjaxReturnsMessageFilter : FarmAjaxFilter
    {
        public FarmAjaxReturnsMessageFilter(
            IServiceUserManager users,
            IServiceRoleManager roles,
            string[] permsAND
        ) : base(users, roles, permsAND)
        {
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                context.Result = new JsonResult(new AjaxResponseWithMessageModel
                {
                    Message = "Success",
                    MessageFormat = ViewModelWithMessage.Format.Default,
                    MessageType = ViewModelWithMessage.Type.Information
                });
            }
            else
            {
                context.Result = new JsonResult(new AjaxResponseWithMessageModel
                {
                    Message = context.Exception.Message,
                    MessageFormat = ViewModelWithMessage.Format.Default,
                    MessageType = ViewModelWithMessage.Type.Error
                });

                context.Exception = null;
            }

            base.OnActionExecuted(context);
        }
    }
    #endregion

    #region With Message And Value
    public class FarmAjaxReturnsMessageAndValueAttribute : TypeFilterAttribute
    {
        public FarmAjaxReturnsMessageAndValueAttribute(params string[] permsAND) : base(typeof(FarmAjaxReturnsMessageAndValueFilter))
        {
            Arguments = new object[] { permsAND ?? Array.Empty<string>() };
        }
    }

    public class FarmAjaxReturnsMessageAndValueFilter : FarmAjaxFilter
    {
        public FarmAjaxReturnsMessageAndValueFilter(
            IServiceUserManager users,
            IServiceRoleManager roles,
            string[] permsAND
        ) : base(users, roles, permsAND)
        {
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var result = context.Result as AjaxValueResult;
            if (result == null && context.Result != null) // If both are null, then an exception bubbled up.
                throw new InvalidOperationException($"The result *must* be of type AjaxValueResult, not {context.Result?.GetType()}.");

            if (context.Exception == null)
            {
                context.Result = new JsonResult(new AjaxResponseWithMessageAndValueModel<Object>
                {
                    Message       = "Success",
                    MessageFormat = ViewModelWithMessage.Format.Default,
                    MessageType   = ViewModelWithMessage.Type.Information,
                    Value         = result.Value
                });
            }
            else
            {
                context.Result = new JsonResult(new AjaxResponseWithMessageAndValueModel<Object>
                {
                    Message       = context.Exception.Message,
                    MessageFormat = ViewModelWithMessage.Format.Default,
                    MessageType   = ViewModelWithMessage.Type.Error,
                    Value         = null
                });

                context.Exception = null;
            }

            base.OnActionExecuted(context);
        }
    }
    #endregion

    public abstract class FarmAjaxFilter : IActionFilter, IResultFilter
    {
        protected readonly IServiceUserManager users;
        protected readonly IServiceRoleManager roles;
        protected readonly string[] permsAND;

        public FarmAjaxFilter(
            IServiceUserManager users,
            IServiceRoleManager roles,
            string[] permsAND
        )
        {
            this.users = users;
            this.roles = roles;
            this.permsAND = permsAND;
        }

        public virtual void OnActionExecuted(ActionExecutedContext context){}
        public virtual void OnResultExecuted(ResultExecutedContext context){}
        public virtual void OnResultExecuting(ResultExecutingContext context){}

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The entire point *is* to catch any exception.")]
        public virtual void OnActionExecuting(ActionExecutingContext context)
        {
            if(!context.ModelState.IsValid)
            {
                var response = new AjaxResponseWithMessageModel();
                response.ParseInvalidModelState(context.ModelState);

                this.OnException(context, response);
                return;
            }

            var model = this.FindModelParameter(context);

            User user;
            try
            { 
                user = this.EnforceUserHasPermission(model);
            }
            catch(Exception ex)
            {
                this.OnException(context, ex);
                return;
            }

            if(user == null)
                throw new Exception("Internal error");

            this.SetUserParameter(context, user);
        }

        #region Helpers
        private AjaxRequestModel FindModelParameter(ActionExecutingContext context)
        {
            var modelKvp = context.ActionArguments.FirstOrDefault(kvp => kvp.Value as AjaxRequestModel != null);
            if (modelKvp.Key == null && modelKvp.Value == null)
                throw new InvalidOperationException("No parameter that inherits from AjaxRequestModel was found.");

            return (AjaxRequestModel)modelKvp.Value;
        }

        private void SetUserParameter(ActionExecutingContext context, User user)
        {
            var userKvp = context.ActionArguments.FirstOrDefault(kvp => kvp.Value as User != null);
            if (userKvp.Key == null)
                return;

            context.ActionArguments[userKvp.Key] = user;
        }

        private User EnforceUserHasPermission(AjaxRequestModel model)
        {
            var user = this.users.UserFromCookieSession(model.SessionToken);
            if (user == null)
                throw new Exception("You are not logged in.");

            if (this.permsAND != null && !this.permsAND.All(p => this.roles.HasPermission(user.Role, p)))
                throw new Exception("You do not have permission to do that.");

            return user;
        }
        
        protected void OnException(ActionExecutingContext context, Exception ex)
        {
            this.OnException(context, new AjaxResponseWithMessageModel
            {
                Message = ex.Message,
                MessageFormat = ViewModelWithMessage.Format.Default,
                MessageType = ViewModelWithMessage.Type.Error
            });
        }

        protected void OnException(ActionExecutingContext context, AjaxResponseWithMessageModel response)
        {
            context.Result = new JsonResult(response);
        }
        #endregion
    }

    public class AjaxValueResult : IActionResult
    {
        public Object Value { get; }

        public AjaxValueResult(Object value)
        {
            this.Value = value;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new NotImplementedException("This shouldn't actually ever be called.");
        }
    }
}
