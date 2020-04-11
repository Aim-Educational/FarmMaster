using DataAccess;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FarmMasterTests.Integration
{
    public abstract class IntegrationTest : IDisposable
    {
        protected TestServer Host { private set; get; }
        protected FarmMasterContext Context { private set; get; }
        protected HttpClient Client { get; set; }

        public IntegrationTest()
        {
            this.Host    = Common.TestHost; // Creates a new one each time.
            this.Client  = this.Host.CreateClient();
            this.Context = this.Host.Services.GetRequiredService<FarmMasterContext>();
        }

        public void Dispose()
        {
            try
            {
                this.Context.Database.EnsureDeleted();
            }
            catch(Exception ex) { }
            this.Client.Dispose();
            this.Host.Dispose();
        }
    }
}
