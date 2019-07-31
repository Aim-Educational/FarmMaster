using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Business.Model
{
    public abstract class AnimalCharacteristicBase
    {
        public const string TYPE_KEY = "__TYPE";
        public AnimalCharacteristic.Type Type { private set; get; }

        public AnimalCharacteristicBase(AnimalCharacteristic.Type type)
        {
            this.Type = type;
        }

        protected abstract JObject ToJsonImpl();
        public abstract void FromJson(JObject json);
        public abstract string ToHtmlString();
        public abstract void FromHtmlString(string html);

        public JObject ToJson()
        {
            var json = this.ToJsonImpl();
            json[TYPE_KEY] = Enum.GetName(typeof(AnimalCharacteristic.Type), this.Type);

            return json;
        }
    }

    public class AnimalCharacteristic
    {
        public enum Type
        {
            Error_Unknown,
            TimeSpan
        }

        [Key]
        public int AnimalCharacteristicId { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [Required]
        [StringLength(ushort.MaxValue)]
        public AnimalCharacteristicBase Data { get; set; }

        [Required]
        public int ListId { get; set; }
        public AnimalCharacteristicList List { get; set; }

        // CALCULATED FIELD
        public Type CalculatedType { get; set; }
    }

    public class AnimalCharacteristicFactory
    {
        public AnimalCharacteristicBase FromJson(JObject json)
        {
            var typeKey = json.GetValue(AnimalCharacteristicBase.TYPE_KEY).Value<string>();

            switch(typeKey)
            {
                case nameof(AnimalCharacteristic.Type.TimeSpan):
                    var c = new AnimalCharacteristicTimeSpan();
                    c.FromJson(json);
                    return c;

                default: throw new InvalidOperationException($"The type key '{typeKey}' does not exist.");
            }
        }
    }

    public class AnimalCharacteristicTimeSpan : AnimalCharacteristicBase
    {
        public TimeSpan TimeSpan { get; set; }

        public AnimalCharacteristicTimeSpan() : base(AnimalCharacteristic.Type.TimeSpan)
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
            var builder = new StringBuilder(30);

            if(this.TimeSpan.Days > 0)    builder.Append($"{this.TimeSpan.Days}d ");
            if(this.TimeSpan.Minutes > 0) builder.Append($"{this.TimeSpan.Minutes}m ");
            if(this.TimeSpan.Seconds > 0) builder.Append($"{this.TimeSpan.Seconds}s");

            return builder.ToString();
        }

        public override void FromHtmlString(string html)
        {
            var match = Regex.Match(html, @"^(\d+d)?\s*(\d+m)?\s*(\d+s)?$");
            if(!match.Success)
                throw new ArgumentOutOfRangeException($"The html string '{html}' does not meet the pattern of '#d #m #s'");

            this.TimeSpan = new TimeSpan();
            if (!String.IsNullOrWhiteSpace(match.Groups[1].Value))
                this.TimeSpan.Add(TimeSpan.FromDays(Convert.ToDouble(match.Groups[1].Value)));
            if (!String.IsNullOrWhiteSpace(match.Groups[2].Value))
                this.TimeSpan.Add(TimeSpan.FromMinutes(Convert.ToDouble(match.Groups[2].Value)));
            if (!String.IsNullOrWhiteSpace(match.Groups[3].Value))
                this.TimeSpan.Add(TimeSpan.FromSeconds(Convert.ToDouble(match.Groups[3].Value)));
        }
    }
}
