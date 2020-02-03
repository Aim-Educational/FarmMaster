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
    // TODO: Move this into Misc, since IPagingController doesn't exist anymore.
    public static class PagingHelper
    {
        public static int CalculatePageCount(int itemCount, int? itemsPerPage)
        {
            var pages = itemCount / itemsPerPage.GetValueOrDefault(FarmConstants.Paging.ItemsPerPage);
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
}
