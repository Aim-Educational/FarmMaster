using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FarmMasterTests.Common
{
    public static class HtmlExtensions
    {
        static readonly IConfiguration   CONFIG  = Configuration.Default.WithDefaultLoader();
        static readonly IBrowsingContext CONTEXT = BrowsingContext.New(CONFIG);

        public static async Task<IDocument> GetDocumentAsync(this HttpResponseMessage http)
        {
            var data = await http.Content.ReadAsStringAsync();
            return await CONTEXT.OpenAsync(r => r.Content(data));
        }

        public static FormUrlEncodedContent ToFormEncodedContent<T>(this T value)
        where T : class
        {
            var json = JsonDocument.Parse(JsonSerializer.Serialize(value));
            var dict = new Dictionary<string, string>();

            // When. Did. C#. Support. Closures. Natively?!
            // No longer do I have to use the ugly Action/Func way <333
            void addToDict(string prefix, JsonElement element)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Object:
                        // Special support for TimeSpan
                        var totalDays = element.EnumerateObject().FirstOrDefault(p => p.NameEquals("TotalDays"));
                        if (totalDays.Value.ValueKind != JsonValueKind.Undefined)
                            addToDict((prefix == null) ? "" : prefix, totalDays.Value);

                        foreach (var child in element.EnumerateObject())
                            addToDict((prefix == null) ? child.Name : prefix + "." + child.Name, child.Value);
                        break;

                    case JsonValueKind.Array:
                        int i = 0;
                        foreach (var child in element.EnumerateArray())
                            addToDict($"{prefix}[{i++}]", child);
                        break;

                    case JsonValueKind.False:  dict[prefix] = "false";                               break;
                    case JsonValueKind.True:   dict[prefix] = "true";                                break;
                    case JsonValueKind.Null:   dict[prefix] = null;                                  break;
                    case JsonValueKind.Number: dict[prefix] = Convert.ToString(element.GetDouble()); break;
                    case JsonValueKind.String: dict[prefix] = element.GetString();                   break;

                    default: throw new Exception($"Unhandled type: {element.ValueKind} -> {element}");
                }
            }           

            addToDict(null, json.RootElement);
            return new FormUrlEncodedContent(dict);
        }
    }
}
