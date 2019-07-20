using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public abstract class ComponentTableInputBase
    {

    }

    public class ComponentTableTextNameTextValueInput : ComponentTableInputBase
    {

    }

    public class ComponentTableTextNameSelectValueInput : ComponentTableInputBase
    {
        public Dictionary<string, string> SelectValues { get; set; }
    }

    public class ComponentTableViewModel
    {
        public string                       Name        { get; set; }
        public string                       Header      { get; set; }
        public Dictionary<string, string>   NameValues  { get; set; }
        public ComponentTableInputBase      Input       { get; set; }
        public string                       AjaxAdd     { get; set; }
        public string                       AjaxRemove  { get; set; }
        public int?                         DataId      { get; set; }
    }
}
