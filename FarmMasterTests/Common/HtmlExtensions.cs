using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FarmMasterTests.Common
{
    public static class HtmlExtensions
    {
        static readonly IConfiguration   CONFIG  = Configuration.Default.WithDefaultLoader();
        static readonly IBrowsingContext CONTEXT = BrowsingContext.New(CONFIG);

        // I thought I needed this stuff for something, but didn't, but might still need this later on, so it stays.

        public static async Task<IDocument> GetDocumentAsync(this HttpResponseMessage http)
        {
            var data = await http.Content.ReadAsStringAsync();
            return await CONTEXT.OpenAsync(r => r.Content(data));
        }
    }
}
