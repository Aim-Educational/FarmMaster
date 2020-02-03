using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.ViewComponents
{
    /// <summary>
    /// A ViewComponent used to easily create a paged table.
    /// </summary>
    public class PagedTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string headers,
            string tableId,
            string pageName,
            string controller,
            string actionEdit,
            string actionDelete
        )
        {
            return View(new Model 
            {
                Headers         = headers.Split(','),
                TableId         = tableId,
                PageCountName   = pageName,
                Controller      = controller,
                ActionEdit      = actionEdit,
                ActionDelete    = actionDelete
            });
        }

        public class Model
        {
            public IEnumerable<string> Headers { get; set; }
            public string TableId { get; set; }
            public string PageCountName { get; set; }
            public string Controller { get; set; }
            public string ActionEdit { get; set; }
            public string ActionDelete { get; set; }
        }
    }
}
