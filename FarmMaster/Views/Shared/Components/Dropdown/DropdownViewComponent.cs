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
        /// <param name="aspFor">Same as attaching an asp-for onto an input tag. Can be null.</param>
        /// <param name="dropdownId">An ID given to the dropdown. This ID can be passed to the Dropdown library if JS is needed.</param>
        /// <param name="defaultValue">The default value of the dropdown. This will determine what value is selected by default during a data load.</param>
        /// <param name="gotoController">The name of the controller for use with the 'Goto' button. Can be left empty for `null`.</param>
        /// <param name="gotoAction">The name of the action for use with the 'Goto' button. Can be left empty for `null`.</param>
        public IViewComponentResult Invoke(
            ModelExpression aspFor, 
            string          dropdownId, 
            string          defaultValue,
            string          gotoController,
            string          gotoAction
        )
        {
            if(gotoController.Length == 0)
                gotoController = null;

            if(gotoAction.Length == 0)
                gotoAction = null;

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
