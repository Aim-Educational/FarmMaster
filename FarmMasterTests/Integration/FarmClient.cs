using DataAccess;
using FarmMaster.Models;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FarmMasterTests.Integration
{
    public class FarmClient
    {
        private CookieContainer _cookies  { get; set; }
        private TestServer _server { get; set; }

        public FarmClient(TestServer server)
        {
            this._server  = server;
            this._cookies = new CookieContainer();
        }

        public async Task LoginAsync(string username = IdentityContext.DEFAULT_USERNAME, string password = IdentityContext.DEFAULT_PASSWORD)
        {
            // Use the ViewModels to ensure our code breaks with any changes.
            var loginModel = new AccountLoginViewModel 
            { 
                Username = username,
                Password = password,
                RememberMe = true
            };

            var response = await this.PostAsync(
                "/Account/Login",
                new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "Username", loginModel.Username },
                    { "Password", loginModel.Password },
                    { "RememberMe", loginModel.RememberMe ? "true" : "false" }
                })
            );
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        }

        // https://stackoverflow.com/questions/48704331/setting-cookies-to-httpclient-of-asp-net-core-testserver
        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            using (var response = await BuildRequest(url).GetAsync())
            {
                UpdateCookies(url, response);
                return response;
            }
        }

        // https://stackoverflow.com/questions/48704331/setting-cookies-to-httpclient-of-asp-net-core-testserver
        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            var builder = BuildRequest(url);
            builder.And(request => request.Content = content);
            using (var response = await builder.PostAsync())
            {
                UpdateCookies(url, response);
                return response;
            }
        }

        // https://stackoverflow.com/questions/48704331/setting-cookies-to-httpclient-of-asp-net-core-testserver
        private RequestBuilder BuildRequest(string url)
        {
            var uri = new Uri(this._server.BaseAddress, url);
            var builder = this._server.CreateRequest(url);

            var cookieHeader = this._cookies.GetCookieHeader(uri);
            if (!string.IsNullOrWhiteSpace(cookieHeader))
            {
                builder.AddHeader(HeaderNames.Cookie, cookieHeader);
            }

            return builder;
        }

        // https://stackoverflow.com/questions/48704331/setting-cookies-to-httpclient-of-asp-net-core-testserver
        private void UpdateCookies(string url, HttpResponseMessage response)
        {
            if (response.Headers.Contains(HeaderNames.SetCookie))
            {
                var uri = new Uri(this._server.BaseAddress, url);
                var cookies = response.Headers.GetValues(HeaderNames.SetCookie);
                foreach (var cookie in cookies)
                {
                    this._cookies.SetCookies(uri, cookie);
                }
            }
        }
    }
}
