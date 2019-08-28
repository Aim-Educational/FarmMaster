using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;
using FarmMaster.Misc;
using FarmMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmMaster.Controllers
{
    public interface IPagingController<T> where T : class
    {
        [HttpPost]
        [AllowAnonymous]
        IActionResult AjaxGetTablePageCount([FromBody] AjaxPagingControllerRequestModel model, User user);

        [HttpPost]
        [AllowAnonymous]
        IActionResult AjaxRenderTablePage([FromBody] AjaxPagingControllerRenderRequestModel model, User user);
    }

    public static class PagingHelper
    {
        public static int CalculatePageCount(int itemCount, int? itemsPerPage)
        {
            var pages = itemCount / itemsPerPage.GetValueOrDefault(GlobalConstants.DefaultPageItemCount);
            return (pages == 0) ? 1 : pages;
        }

        public static IEnumerable<T> GetPage<T>(IEnumerable<T> items, int page, int? itemsPerPage)
        where T : class
        {
            itemsPerPage = itemsPerPage.GetValueOrDefault(GlobalConstants.DefaultPageItemCount);

            var pages = PagingHelper.CalculatePageCount(items.Count(), itemsPerPage) - 1; // PageCount always returns 1 or more, so take off 1.
            return items.Skip((pages - page) * itemsPerPage.Value).Take(itemsPerPage.Value);
        }
    }

    public class AjaxPagingControllerRequestModel : AjaxRequestModel
    {
        public int? ItemsPerPage { get; set; }
        public string EntityType { get; set; }
    }

    public class AjaxPagingControllerRenderRequestModel : AjaxPagingControllerRequestModel
    {
        [Required]
        public int PageToRender { get; set; }
    }
}
