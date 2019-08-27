using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;

/*
 * A DynamicField, as the name supplies, is a field that can hold dynamically changing data.
 * 
 * The main use of a DynamicField is to have a field that the user can customise the type of data for.
 * 
 * For example, with the AnimalCharacteristic entity, the user can decide what type of data the characteristic stores. Instead
 * of having 2000 different columns for every possible type, we instead use a DynamicField which has proper support both in the backend and frontend
 * for easily allowing dynamic fields to be used.
 */
namespace Business.Model
{
    public abstract class DynamicField
    {
        public enum Type
        {
            Error_Unknown,
            TimeSpan,
            Text,
            DateTime // Stored in UTC.
        }

        public const string TYPE_KEY = "__TYPE";
        public DynamicField.Type FieldType { private set; get; }

        public DynamicField(DynamicField.Type type)
        {
            this.FieldType = type;
        }

        protected abstract JObject ToJsonImpl();
        public abstract void FromJson(JObject json);
        public abstract string ToHtmlString();
        public abstract void FromHtmlString(string html);

        public JObject ToJson()
        {
            var json = this.ToJsonImpl();
            json[TYPE_KEY] = Enum.GetName(typeof(DynamicField.Type), this.FieldType);

            return json;
        }
    }

    public static class DynamicFieldExtentions
    {
        public static PropertyBuilder<DynamicField> IsDynamicField(this PropertyBuilder<DynamicField> builder)
        {
            builder.HasConversion(
                c => c.ToJson().ToString(),
                s => (new DynamicFieldFactory()).FromJson(JObject.Parse(s))
            );
            return builder;
        }
    }

    public class DynamicFieldFactory
    {
        public DynamicField FromJson(JObject json)
        {
            var typeKey = json.GetValue(DynamicField.TYPE_KEY).Value<string>();
            var type = Enum.Parse<DynamicField.Type>(typeKey);
            var data = this.FromType(type);

            data.FromJson(json);
            return data;
        }

        public DynamicField FromTypeAndHtmlString(DynamicField.Type type, string htmlString)
        {
            var data = this.FromType(type);
            data.FromHtmlString(htmlString);
            return data;
        }

        private DynamicField FromType(DynamicField.Type type)
        {
            switch (type)
            {
                case DynamicField.Type.TimeSpan:
                    return new DynamicFieldTimeSpan();

                case DynamicField.Type.Text:
                    return new DynamicFieldText();

                case DynamicField.Type.DateTime:
                    return new DynamicFieldDateTime();

                default: throw new InvalidOperationException($"The type key '{type}' does not exist.");
            }
        }
    }

    public class DynamicFieldTimeSpan : DynamicField
    {
        public TimeSpan TimeSpan { get; set; }

        public DynamicFieldTimeSpan() : base(DynamicField.Type.TimeSpan)
        {
        }

        public override void FromJson(JObject json)
        {
            this.TimeSpan = TimeSpan.Parse(json["v"].Value<string>());
        }

        protected override JObject ToJsonImpl()
        {
            var json = new JObject();
            json["v"] = this.TimeSpan.ToString();

            return json;
        }

        public override string ToHtmlString()
        {
            if(this.TimeSpan.TotalSeconds == 0)
                return "0d 0m 0s";

            var builder = new StringBuilder(30);

            if (this.TimeSpan.Days > 0) builder.Append($"{this.TimeSpan.Days}d ");
            if (this.TimeSpan.Minutes > 0) builder.Append($"{this.TimeSpan.Minutes}m ");
            if (this.TimeSpan.Seconds > 0) builder.Append($"{this.TimeSpan.Seconds}s");

            return builder.ToString();
        }

        public override void FromHtmlString(string html)
        {
            var match = Regex.Match(html, @"^(\d+)d?\s*(\d+)m?\s*(\d+)s?$");
            if (!match.Success)
                throw new ArgumentOutOfRangeException($"The html string '{html}' does not meet the pattern of '#d #m #s'");

            this.TimeSpan = new TimeSpan();
            if (!String.IsNullOrWhiteSpace(match.Groups[1].Value))
                this.TimeSpan = this.TimeSpan.Add(TimeSpan.FromDays(Convert.ToDouble(match.Groups[1].Value)));
            if (!String.IsNullOrWhiteSpace(match.Groups[2].Value))
                this.TimeSpan = this.TimeSpan.Add(TimeSpan.FromMinutes(Convert.ToDouble(match.Groups[2].Value)));
            if (!String.IsNullOrWhiteSpace(match.Groups[3].Value))
                this.TimeSpan = this.TimeSpan.Add(TimeSpan.FromSeconds(Convert.ToDouble(match.Groups[3].Value)));
        }
    }

    public class DynamicFieldText : DynamicField
    {
        public string Text { set; get; }

        public DynamicFieldText() : base(DynamicField.Type.Text)
        {

        }

        public override void FromHtmlString(string html)
        {
            this.Text = html;
        }

        public override string ToHtmlString()
        {
            return this.Text;
        }

        public override void FromJson(JObject json)
        {
            this.Text = json.GetValue("v").Value<string>();
        }

        protected override JObject ToJsonImpl()
        {
            var json = new JObject();
            json["v"] = this.Text;

            return json;
        }
    }

    public class DynamicFieldDateTime : DynamicField
    {
        public DateTimeOffset DateTime { set; get; }

        public DynamicFieldDateTime() : base(DynamicField.Type.DateTime)
        {

        }

        public override void FromHtmlString(string html)
        {
            // "O" is full datetime with timezone info
            this.DateTime = DateTimeOffset.ParseExact(html, "O", CultureInfo.InvariantCulture);
        }

        public override string ToHtmlString()
        {
            return this.DateTime.ToString("O", CultureInfo.InvariantCulture);
        }

        public override void FromJson(JObject json)
        {
            this.FromHtmlString(json.GetValue("v").Value<string>());
        }

        protected override JObject ToJsonImpl()
        {
            var json = new JObject();
            json["v"] = this.ToHtmlString();

            return json;
        }
    }
}
