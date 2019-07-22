using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public abstract class ComponentTableInputBase
    {

    }

    public class ComponentTableTextInput : ComponentTableInputBase
    {

    }

    public class ComponentTableSelectInput : ComponentTableInputBase
    {
        public Dictionary<string, string> SelectValues { get; set; }
    }

    public class ComponentTableNameValue
    {
        public string Name;
        public string Value;
        public object AjaxName; // The 'Name' sent to the Ajax callback. Only used if not null.
    }

    public class ComponentTableViewModel
    {
        public string                               Name        { get; set; }
        public string                               Header      { get; set; }
        public IEnumerable<ComponentTableNameValue> NameValues  { get; set; }
        public ComponentTableInputBase              NameInput   { get; set; }
        public ComponentTableInputBase              ValueInput  { get; set; }
        public string                               AjaxAdd     { get; set; }
        public string                               AjaxRemove  { get; set; }
        public int?                                 DataId      { get; set; }
    }

    public class ComponentTableInputPartialViewModel
    {
        public string InputHeader { get; set; }
        public ComponentTableInputBase Input { get; set; }
        public string IdInput { get; set; }
    }
}
