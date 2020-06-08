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
        public async Task<HttpResponseMessage> CrudCreateAsync<T>(string url, T entity)
        where T : class
        {
            var model = new CrudCreateEditViewModel<T> 
            {
                IsCreate = true,
                Entity   = entity
            };

            // Test for redirect, since it should be going to an edit page, instead of erroring to the create page.
            return await this.PostEnsureStatusAsync(url, model.ToFormEncodedContent(), HttpStatusCode.Redirect);
        }
    }
}
