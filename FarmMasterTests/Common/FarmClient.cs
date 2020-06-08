using AccountModule.Models;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace FarmMasterTests.Common
{
    /// <summary>
    /// A HttpClient used to interface with the <see cref="TestServer"/>.
    /// </summary>
    /// <remarks>
    /// The HttpClient returned by <see cref="TestServer.CreateClient"/> is... *shit*.
    /// 
    /// We can't configure it in anyway, so it won't store cookies, doesn't redirect, etc.
    /// 
    /// So this class is used to solve all our issues.
    /// </remarks>
    public partial class FarmClient
    {
        private CookieContainer _cookies { get; set; }
        private TestServer _server { get; set; }

        public FarmClient(TestServer server)
        {
            this._server = server;
            this._cookies = new CookieContainer();
        }

        #region COMMON ACTIONS
        public void ClearCookies()
        {
            this._cookies = new CookieContainer();
        }
        #endregion

        #region GET/POST
        public async Task<HttpResponseMessage> GetEnsureStatusAsync(string url, HttpStatusCode status, Regex urlRegexTest = null)
        {
            var response = await this.GetAsync(url);
            Assert.Equal(status, response.StatusCode);

            if (urlRegexTest != null)
                Assert.Matches(urlRegexTest, response.Headers.Location.ToString());

            return response;
        }

        public async Task<HttpResponseMessage> PostEnsureStatusAsync(
            string         url,
            HttpContent    content, 
            HttpStatusCode status, 
            Regex          urlRegexTest = null
        )
        {
            var response = await this.PostAsync(url, content);
            Assert.Equal(status, response.StatusCode);
            
            if(urlRegexTest != null)
                Assert.Matches(urlRegexTest, response.Headers.Location.ToString());

            return response;
        }

        // https://stackoverflow.com/questions/48704331/setting-cookies-to-httpclient-of-asp-net-core-testserver
        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            // NOTE: We don't dispose of the responses, since our testing code may need access to the response content,
            //       which can only be accessed on a non-disposed object.
            //
            //       This does mean that the memory lingers about until the GC kicks in, but this is testing code, and is
            //       very short lived, so this shouldn't matter at all.
            var response = await this.BuildRequest(url).GetAsync();
            this.UpdateCookies(url, response);
            return response;
        }

        // https://stackoverflow.com/questions/48704331/setting-cookies-to-httpclient-of-asp-net-core-testserver
        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            var builder = this.BuildRequest(url);
            builder.And(request => request.Content = content);
            var response = await builder.PostAsync();

            this.UpdateCookies(url, response);
            return response;
        }
        #endregion

        #region HELPERS
        private async Task GetVerificationTokenAsync(Uri uri, RequestBuilder parentBuilder)
        {
            /**
             * ASP's Antiforgery has two parts:
             * 
             *  - 1. It stores half of the token inside of the __RequestVerificationToken input
             *  - 2. It stores the other half inside of the .AspNetCore.Antiforgery.XXXXXXX cookie
             *  
             * So we GET the Account/Login page, which we should always have anon access to as well as it always providing
             * these two values for us.
             * 
             * We then extract these values, and use them to build up the parentBuilder's request further.
             * 
             * This allows us to satisfy [ValidateAntiforgeryToken] :)
             * **/

            // Expire the forgery token, to ensure we're sent a fresh pair of tokens.
            var loginUri = new Uri(this._server.BaseAddress, "Account/Login");
            foreach (Cookie cookie in this.GetAllCookies().Where(c => c.Name.StartsWith(".AspNetCore.Antiforgery.")))
                cookie.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            var builder = this.BuildRequest("Account/Login", true);
            using (var response = await builder.GetAsync())
            {
                var content = await response.Content.ReadAsStringAsync();
                var match = Regex.Match(content, @"<input\s+name=""__RequestVerificationToken""\s+type=""hidden""\s+value=""([^""]+)""\s+/>");

                if (!match.Success)
                    throw new InvalidOperationException("Could not retrieve verification token.");

                parentBuilder.AddHeader("X-CSRF-TOKEN", match.Groups[1].Value);
                var forgeryCookie = response
                                    .Headers
                                    .GetValues(HeaderNames.SetCookie)
                                    .SelectMany(c => c.Split(";"))
                                    .Where(c => c.StartsWith(".AspNetCore.Antiforgery."))
                                    .Select(c => new Cookie(c.Split("=")[0], c.Split("=")[1]))
                                    .First();
                this._cookies.Add(uri, forgeryCookie);
            }
        }

        // https://stackoverflow.com/questions/48704331/setting-cookies-to-httpclient-of-asp-net-core-testserver
        private RequestBuilder BuildRequest(string url, bool noVerficiationToken = false)
        {
            var uri = new Uri(this._server.BaseAddress, url);
            var builder = this._server.CreateRequest(url);

            if (!noVerficiationToken)
                this.GetVerificationTokenAsync(uri, builder).Wait();

            var cookieHeader = this._cookies.GetCookieHeader(uri);
            if (!string.IsNullOrWhiteSpace(cookieHeader))
                builder.AddHeader(HeaderNames.Cookie, cookieHeader);

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

        // https://stackoverflow.com/a/31900670
        // One would imagine this would be easier to do, but #JustMicrosoftThings
        private IEnumerable<Cookie> GetAllCookies()
        {
            var c = this._cookies;
            Hashtable k = (Hashtable)c.GetType().GetField("m_domainTable", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(c);
            foreach (DictionaryEntry element in k)
            {
                SortedList l = (SortedList)element.Value.GetType().GetField("m_list", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(element.Value);
                foreach (var e in l)
                {
                    var cl = (CookieCollection)((DictionaryEntry)e).Value;
                    foreach (Cookie fc in cl)
                    {
                        yield return fc;
                    }
                }
            }
        }
        #endregion
    }
}
