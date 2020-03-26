using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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

            // example match: {{ username }} -> "username"
            var normalValues = Regex.Matches(templateText, @"{{\s*([\w_]+)\s*}}");

            // example match: {{ @Account#ConfirmEmail?token=valueKey }} -> "Account", "ConfirmEmail", "token", "valueKey"
            var linkValues = Regex.Matches(templateText, @"{{\s*@(\w+)#(\w+)\?(\w+)=(\w+)\s*}}");
            this.ValueKeys = normalValues
                             .Select((Match m) => m.Groups[1].Value)
                             .Concat(linkValues
                                     .Select((Match m) => m.Groups[4].Value)
                             );
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
        /// <param name="accessor">Needed for <paramref name="generator"/>, can be null, but only recommended for testing.</param>
        /// <param name="generator">Used to generate links, can be null, but only recommended for testing.</param>
        /// <returns>A resolved version of this template, where all placeholders are replaced with their values.</returns>
        public string Resolve(EmailTemplateValues values, IHttpContextAccessor accessor, LinkGenerator generator)
        {
            if(!this.AreAllValuesDefined(values))
                throw new InvalidOperationException("Not all values have been defined for this template. TODO: List which values.");

            var output = new StringBuilder(this._rawText.Length);
            var start  = 0;

            Action<int> commit = endIndex => // endIndex is exclusive
            {
                output.Append(this._rawText.AsSpan(start, (endIndex - start)));
            };

            // I'm not the greatest fan of single-function parsers, but it's *just* on the edge of what I'd
            // consider not featureful enough to bother with a properly structured one.
            for(int i = 0; i < this._rawText.Length; i++)
            {
                var ch = this._rawText[i];

                // If we reach a placeholder, commit the current selection, then begin processing.
                if(ch == '{' && i != this._rawText.Length - 1 && this._rawText[i + 1] == '{')
                {
                    commit(i);
                    i += 2; // Skip both {{

                    if(i >= this._rawText.Length)
                        break;

                    // Skip spaces (not all whitespace, since that's invalid syntax in this case)
                    while(i < this._rawText.Length && this._rawText[i] == ' ')
                        i++;

                    if(i >= this._rawText.Length)
                        break;

                    // Decide what type of placeholder it is.
                    bool isLink = this._rawText[i] == '@';

                    if(isLink) // Link placeholder. ex: @Account#ConfirmEmail?token=valueKey }}
                    {
                        // (This doesn't validate input enough, but honestly it's more effort than it's worth for a basic single-function parser.)
                        // Also code duplication.
                        start = ++i;

                        string controller = null;
                        string action     = null;
                        string query      = null;
                        string valueKey   = null;

                        // Read until a #
                        while(i < this._rawText.Length && this._rawText[++i] != '#') { }

                        controller = this._rawText.Substring(start, i - start);
                        i++;
                        start = i;

                        // Read until a ?
                        while (i < this._rawText.Length && this._rawText[++i] != '?') { }

                        action = this._rawText.Substring(start, i - start);
                        i++;
                        start = i;

                        // Read until an =
                        while (i < this._rawText.Length && this._rawText[++i] != '=') { }

                        query = this._rawText.Substring(start, i - start);
                        i++;
                        start = i;

                        // Read until whitespace or bracket
                        while (i < this._rawText.Length && this._rawText[i] != ' ' && this._rawText[i] != '}')
                            i++;

                        valueKey = this._rawText.Substring(start, i - start);

                        // Skip spaces, read past the }}, and then we're done. (Not enough validation done here, but meh)
                        while (i < this._rawText.Length && this._rawText[i] == ' ')
                            i++;
                        i += 2; // Trusting the input too much.
                        start = i;

                        string url = null;

                        if(generator != null)
                        { 
                            url = generator.GetUriByAction(
                                accessor.HttpContext, 
                                action, 
                                controller,
                                null,
                                accessor.HttpContext.Request.Scheme
                            );
                        }
                        else
                            url = $"/{controller}/{action}";
                        output.AppendFormat("<a href='{0}?{1}={2}'>here</a>", url, query, values[valueKey]);
                    }
                    else // Normal placeholder. ex: username }}
                    {
                        // Read until space or bracket.
                        start = i;
                        while(i < this._rawText.Length && this._rawText[i] != ' ' && this._rawText[i] != '}')
                            i++;

                        var key = this._rawText.Substring(start, i - start);
                        output.Append(Convert.ToString(values[key]));

                        // Skip spaces, read past the }}, and then we're done. (Not enough validation done here, but meh)
                        while (i < this._rawText.Length && this._rawText[i] == ' ')
                            i++;
                        i += 2; // Trusting the input too much.
                        start = i;
                    }
                }
            }

            if(start < this._rawText.Length)
                commit(this._rawText.Length);

            return output.ToString();
        }
    }
}
