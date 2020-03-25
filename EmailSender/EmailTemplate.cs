using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
namespace EmailSender
{
    /// <summary>
    /// Contains the values to pass to an <see cref="EmailTemplate"/>
    /// </summary>
    public class EmailTemplateValues : Dictionary<string, object>
    {
    }

    /// <summary>
    /// Contains a template of an email.
    /// </summary>
    /// <remarks>
    /// The syntax of a template is very, very simple.
    /// 
    /// For the most part, the template is any ordinary text file, except placeholders defined like `{{ value_key }}`
    /// are replaced by values provided at runtime.
    /// 
    /// For example, the template `Hello, {{ username }}!` with the values `{ username: "Sealab" }` would produce
    /// the output of `Hello, Sealab!`
    /// </remarks>
    public class EmailTemplate
    {
        readonly string _rawText;

        /// <summary>
        /// An enumeration of all keys/placeholders found inside the template.
        /// </summary>
        public IEnumerable<string> ValueKeys { get; private set; }

        /// <summary>
        /// Creates a new email template with the provided text.
        /// </summary>
        /// <param name="templateText">The text that the template consists of.</param>
        public EmailTemplate(string templateText)
        {
            this._rawText = templateText;

            var captures = Regex.Matches(templateText, @"{{\s*([\w_]+)\s*}}"); // example match: {{ username }} -> "username"
            this.ValueKeys = captures.Select((Match m) => m.Groups[1].Value);
        }

        /// <summary>
        /// Determines if the provided template values contains all required values for this template.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="values"/> is null.</exception>
        /// <param name="values">The values to check over.</param>
        /// <returns>True if <paramref name="values"/> contains all the values that this template needs.</returns>
        public bool AreAllValuesDefined(EmailTemplateValues values)
        {
            if(values == null)
                throw new ArgumentNullException(nameof(values));

            return this.ValueKeys.All(k => values.ContainsKey(k));
        }

        /// <summary>
        /// Resolves this template using the given values.
        /// </summary>
        /// <exception cref="InvalidOperationException">If <paramref name="values"/> does not contain all required values.</exception>
        /// <param name="values">The values to use.</param>
        /// <returns>A resolved version of this template, where all placeholders are replaced with their values.</returns>
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
