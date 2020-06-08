using AccountModule.Models;
using DataAccess;
using FarmMaster.Module.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace FarmMasterTests.Common
{
    public partial class FarmClient
    {
        public Task<HttpResponseMessage> CrudCreateAsync<T>(string url, T entity)
        where T : class
        {
            return this.CrudCreateEditAsync(url, entity, true);
        }

        public Task<HttpResponseMessage> CrudEditAsync<T>(string url, T entity)
        where T : class
        {
            return this.CrudCreateEditAsync(url, entity, false);
        }

        private async Task<HttpResponseMessage> CrudCreateEditAsync<T>(string url, T entity, bool isCreate)
        where T : class
        {
            var model = new CrudCreateEditViewModel<T> 
            {
                IsCreate = isCreate,
                Entity   = entity
            };

            
            var codeToCheck = (isCreate) 
                ? HttpStatusCode.Redirect // Test for redirect, since it should be going to an edit page, instead of erroring to the create page.
                : HttpStatusCode.OK;      // Test for going back to the same page.

            return await this.PostEnsureStatusAsync(url, model.ToFormEncodedContent(), codeToCheck);
        }
    }
}
