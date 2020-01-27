using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.ViewComponents
{
    /// <summary>
    /// A ViewComponent used to easily create more featureful dropdowns, instead of just a plain HTML select tag.
    /// 
    /// The generated HTML is compatible for use with FarmMaster's <code>Dropdown</code> library.
    /// </summary>
    public class DropdownViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            ModelExpression aspFor, 
            string          dropdownId, 
            string          defaultValue,
            string          gotoController,
            string          gotoAction
        )
        {
            return View(new Model 
            {
                AspFor          = aspFor,
                DropdownId      = dropdownId,
                DefaultValue    = defaultValue,
                GotoAction      = gotoAction,
                GotoController  = gotoController
            });
        }

        public class Model
        {
            public ModelExpression AspFor { get; set; }
            public string DropdownId { get; set; } // For use with FarmMaster's Dropdown library.
            public string DefaultValue { get; set; }
            public string GotoController { get; set; }
            public string GotoAction { get; set; }
        }
    }
}
