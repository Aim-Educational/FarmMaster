using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Models
{
    public abstract class AnimalGroupScriptEditorViewModel
    {
    }

    public class AnimalGroupScriptEditorSingleUseViewModel :  AnimalGroupScriptEditorViewModel
    {
        public int GroupId { get; set; }
        public string AnimalGridId { get; set; }
    }
}
