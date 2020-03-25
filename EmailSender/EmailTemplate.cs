using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailSender
{
    /**
     * Why not Razor?
     * 
     * 1. Google is completely unhelpful on trying to find out how to use Razor as a library on its own.
     * 
     * 2. Razor is overly complex for email templates. We don't need most of its features due to emails being very static
     *    and simplistic.
     *    
     * 3. We can easily add in our own syntax to the templates as needed, e.g. for easily generating links to the website.
     *    With Razor it'd be a bit more cumbersome.
     * */

    public class EmailTemplateValues : Dictionary<string, object>
    {
    }

    public class EmailTemplate
    {
        readonly string _rawText;

        public IEnumerable<string> ValueKeys { get; private set; }

        public EmailTemplate(string templateText)
        {
            this._rawText = templateText;

            var captures = Regex.Matches(templateText, @"{{\s*([\w_]+)\s*}}"); // example match: {{ username }} -> "username"
            this.ValueKeys = captures.Select((Match m) => m.Groups[1].Value);
        }

        public bool AreAllValuesDefined(EmailTemplateValues values)
        {
            if(values == null)
                throw new ArgumentNullException(nameof(values));

            return this.ValueKeys.All(k => values.ContainsKey(k));
        }

        public string Resolve(EmailTemplateValues values)
        {
            if(!this.AreAllValuesDefined(values))
                throw new InvalidOperationException("Not all values have been defined for this template. TODO: List which values.");

            // TODO: When I'm less lazy, this could be O(n) instead of O(n*m)
            //       It'd also mean I can switch over to StringBuilder instead of doing this memory inefficient stuff.
            //       Not to mention ToString and the string interpolation also doing their thing.
            string output = this._rawText;
            foreach(var key in values.Keys)
                output = output.Replace($"{{{{ {key} }}}}", Convert.ToString(values[key]));

            return output;
        }
    }
}
